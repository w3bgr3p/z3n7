using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.Enums.Log;


namespace z3n7
{
    /// <summary>
    /// Picking the next account to work on from the database by condition, range priorities and
    /// social-account filters.
    /// </summary>
    public static class AccountRunner
    {
        private static List<string> ParseRangeGroups(IZennoPosterProjectModel project, string cfgAccRange)
        {
            var groups = new List<string>();
    
            // Сплитим по : (группы приоритета)
            foreach (var groupRange in cfgAccRange.Split(':'))
            {
                // Каждую группу обрабатываем через оригинальный Range()
                var rangeIds = project.Range(groupRange); // вернет List<string> ID
                // Склеиваем обратно в строку для SQL IN (...)
                groups.Add(string.Join(",", rangeIds));
            }
    
            return groups;
        }
        
        private static string ParseSingleRange(string rangeStr)
        {
            if (rangeStr.Contains(","))
            {
                // Already comma-separated list
                return rangeStr;
            }
            else if (rangeStr.Contains("-"))
            {
                // Range format like "1-100"
                var parts = rangeStr.Split('-').Select(int.Parse).ToArray();
                int start = parts[0];
                int end = parts[1];
                return string.Join(",", Enumerable.Range(start, end - start + 1));
            }
            else
            {
                // Single number
                return rangeStr;
            }
        }

        private static void GetListFromDb(this IZennoPosterProjectModel project,
            string condition,
            string sortByTaskAge = null,
            bool useRange = true,
            bool filterTwitter = false,
            bool filterDiscord = false,
            bool filterGithub = false,
            string tableName = null,
            bool debugLog = false,
            bool sqlNow = true)
        {
            if (!string.IsNullOrEmpty(project.Var("acc0")))
            {
                return;
            }
            if (string.IsNullOrEmpty(tableName)) 
                tableName = project.ProjectTable();
            if (sqlNow)
            {
                condition = ApplySqlNow(condition, project.DbMode()); 
            }
            // Parse priority groups from cfgAccRange
            List<string> rangeGroups = new List<string>();
            if (useRange)
            {
                var cfgAccRange = project.Var("cfgAccRange");
                rangeGroups = ParseRangeGroups(project, cfgAccRange);
            }
            else
            {
                rangeGroups.Add(null); // Single iteration without range
            }
            
            // Try each priority group in order
            foreach (var rangeGroup in rangeGroups)
            {
                var fullCondition = useRange
                    ? $"{condition} AND id in ({rangeGroup})"
                    : condition;

                List<string> accounts;

                if (!string.IsNullOrEmpty(sortByTaskAge))
                {
                    var selectColumns = $"\"id\", \"{sortByTaskAge}\"";
                    var orderBy = $"CASE WHEN \"{sortByTaskAge}\" = '' OR \"{sortByTaskAge}\" IS NULL THEN '9999-12-31' ELSE \"{sortByTaskAge}\" END ASC";
                    var query = $"SELECT {selectColumns} FROM \"{tableName}\" WHERE {fullCondition} ORDER BY {orderBy}";
                    
                    var rawData = project.DbQ(query, log: debugLog);

                    var accountsWithDates = rawData.Split('·')
                        .Where(row => !string.IsNullOrWhiteSpace(row))
                        .Select(row =>
                        {
                            var parts = row.Split('¦');
                            var id = parts[0];
                            var dateStr = parts.Length > 1 ? parts[1] : "";

                            DateTime date;
                            if (!DateTime.TryParseExact(dateStr,
                                    "yyyy-MM-ddTHH:mm:ss.fffZ",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                                    out date))
                            {
                                date = DateTime.MaxValue;
                            }

                            return new { Id = id, Date = date };
                        })
                        .OrderBy(x => x.Date)
                        .Select(x => x.Id)
                        .ToList();

                    accounts = accountsWithDates;
                }
                else
                {
                    accounts = project.DbGetLines("id",tableName: tableName, log: debugLog, where: fullCondition)
                        .Where(acc => !string.IsNullOrWhiteSpace(acc))
                        .ToList();
                }

                if (!accounts.Any())
                {
                    // No accounts found in this group, try next priority group
                    continue;
                }

                project.ListSync("accs", accounts);

                if (filterTwitter)
                {
                    if (!project.FilterBySocial("twitter", condition))
                        continue; // Try next group
                }

                if (filterDiscord)
                {
                    if (!project.FilterBySocial("discord", condition))
                        continue; // Try next group
                }
                
                if (filterGithub)
                {
                    if (!project.FilterBySocial("github", condition))
                        continue; // Try next group
                }

                accounts = project.Lists["accs"].ToList();

                if (!accounts.Any())
                {
                    // After filtering, no accounts left, try next group
                    continue;
                }
                
                // Successfully found accounts, exit
                return;
            }
            
            project.warn($"No accounts found by condition\n{condition.Replace("\n", " ")} in range {project.Var("cfgAccRange")}");
        }

        /// <summary>
        /// Picks an account and stores it in <c>acc0</c>; the candidates stay in the <c>accs</c> list and its
        /// row gets <c>status = 'working...'</c>.
        /// When <c>acc0Forced</c> is set, it is used as is. When <c>acc0</c> is already set, the candidate
        /// search is skipped. Throws when no account matches.
        /// </summary>
        /// <param name="condition">
        /// SQL condition on the account table. The word <c>NOW</c> is replaced by the current time as
        /// <c>yyyy-MM-ddTHH:mm:ss</c> when <c>sqlNow</c> is true.
        /// </param>
        /// <param name="useRange">
        /// Limit to <c>cfgAccRange</c>. Groups separated by <c>:</c> are priorities: the first group with
        /// matching accounts is used.
        /// </param>
        /// <param name="filterTwitter">Keep only accounts whose row in the <c>_twitter</c> table has <c>status = 'ok'</c>.</param>
        /// <param name="filterDiscord">Keep only accounts whose row in the <c>_discord</c> table has <c>status = 'ok'</c>.</param>
        /// <param name="filterGithub">Keep only accounts whose row in the <c>_github</c> table has <c>status = 'ok'</c>.</param>
        /// <param name="tableName">Account table; default <c>__</c> + project name.</param>
        /// <param name="debugLog">Write the queries to the log.</param>
        /// <param name="sqlNow">Replace <c>NOW</c> in the condition.</param>
        /// <param name="sortByTaskAge">
        /// Column with ISO timestamps: take the account with the oldest value (empty first). When empty, a
        /// random account is taken.
        /// </param>
        public static void ChooseAccountByCondition(this IZennoPosterProjectModel project,
            string condition,
            string sortByTaskAge = null,
            bool useRange = true,
            bool filterTwitter = false,
            bool filterDiscord = false,
            bool filterGithub = false,
            string tableName = null,
            bool debugLog = false,
            bool sqlNow = true)
        {
            if (!string.IsNullOrEmpty(project.Var("acc0Forced")))
            {
                project.Var("acc0", project.Var("acc0Forced"));
                return;
            }

            // Get filtered list from DB
            project.GetListFromDb(condition, sortByTaskAge, useRange, filterTwitter, filterDiscord, filterGithub, tableName ,debugLog, sqlNow);

            var accounts = project.Lists["accs"].ToList();

            if (!accounts.Any())
            {
                project.warn($"Account selection failed: condition={condition}, found=0 in all priority groups", thrw: true);
                return;
            }

            var acc0 = !string.IsNullOrEmpty(sortByTaskAge)
                ? accounts.First()
                : project.RndFromList("accs", true);

            project.Var("acc0", acc0);

            if (!string.IsNullOrEmpty(sortByTaskAge))
                project.Lists["accs"].Remove(acc0);

            var left = project.Lists["accs"].Count;
            project.DbUpd($"status = 'working...'");
            project.SendToLog($"Account selected: acc={acc0}, remaining={left}, condition={condition}, range={project.Var("cfgAccRange")}", LogType.Info, true, LogColor.Gray);
        }

        /// <summary>
        /// Like the single-condition overload, but takes the accounts that match every condition in its own
        /// table (intersection).
        /// </summary>
        /// <param name="conditionsByTable">Condition → table.</param>
        /// <param name="sortByTaskAge">When set, the first remaining account is taken instead of a random one; no sorting is done.</param>
        /// <param name="useRange">
        /// Limit to <c>cfgAccRange</c>. Groups separated by <c>:</c> are priorities: the first group with
        /// matching accounts is used.
        /// </param>
        /// <param name="filterTwitter">Keep only accounts whose row in the <c>_twitter</c> table has <c>status = 'ok'</c>.</param>
        /// <param name="filterDiscord">Keep only accounts whose row in the <c>_discord</c> table has <c>status = 'ok'</c>.</param>
        /// <param name="filterGithub">Keep only accounts whose row in the <c>_github</c> table has <c>status = 'ok'</c>.</param>
        /// <param name="debugLog">Write the queries to the log; also makes an empty intersection throw.</param>
        /// <param name="sqlNow">Replace <c>NOW</c> in the conditions.</param>
        public static void ChooseAccountByCondition(this IZennoPosterProjectModel project,
            Dictionary<string, string> conditionsByTable,
            string sortByTaskAge = null,
            bool useRange = true,
            bool filterTwitter = false,
            bool filterDiscord = false,
            bool filterGithub = false,
            bool debugLog = false,
            bool sqlNow = true)
        {
            if (!string.IsNullOrEmpty(project.Var("acc0Forced")))
            {
                project.Var("acc0", project.Var("acc0Forced"));
                return;
            }

            if (conditionsByTable == null || !conditionsByTable.Any())
            {
                project.warn("ChooseAccountByCondition: conditionsByTable is empty", thrw: true);
                return;
            }

            HashSet<string> resultSet = null;

            foreach (var kvp in conditionsByTable)
            {
                var condition = kvp.Key;
                var tableName = kvp.Value;

                if (sqlNow)
                {
                    condition = ApplySqlNow(condition, project.DbMode());
                }

                List<string> rangeGroups = new List<string>();
                if (useRange)
                {
                    var cfgAccRange = project.Var("cfgAccRange");
                    rangeGroups = ParseRangeGroups(project, cfgAccRange);
                }
                else
                {
                    rangeGroups.Add(null);
                }

                HashSet<string> tableAccounts = new HashSet<string>();

                foreach (var rangeGroup in rangeGroups)
                {
                    var fullCondition = useRange
                        ? $"{condition} AND id in ({rangeGroup})"
                        : condition;

                    var accounts = project.DbGetLines("id", tableName: tableName, log: debugLog, where: fullCondition)
                        .Where(acc => !string.IsNullOrWhiteSpace(acc))
                        .ToList();

                    foreach (var acc in accounts)
                    {
                        tableAccounts.Add(acc);
                    }
                }

                if (resultSet == null)
                {
                    resultSet = tableAccounts;
                }
                else
                {
                    resultSet.IntersectWith(tableAccounts);
                }

                if (!resultSet.Any())
                {
                    project.warn($"No intersection found after processing table={tableName}, condition={condition}", thrw: debugLog);
                    return;
                }
            }

            if (resultSet == null || !resultSet.Any())
            {
                var conditionsStr = string.Join("; ", conditionsByTable.Select(kvp => $"{kvp.Value}:{kvp.Key}"));
                project.warn($"Account selection failed: no accounts match ALL conditions: {conditionsStr}", thrw: true);
                return;
            }

            var finalAccounts = resultSet.ToList();
            project.ListSync("accs", finalAccounts);

            if (filterTwitter)
            {
                if (!project.FilterBySocial("twitter"))
                {
                    project.warn("No accounts left after Twitter filter", thrw: true);
                    return;
                }
            }

            if (filterDiscord)
            {
                if (!project.FilterBySocial("discord"))
                {
                    project.warn("No accounts left after Discord filter", thrw: true);
                    return;
                }
            }

            if (filterGithub)
            {
                if (!project.FilterBySocial("github"))
                {
                    project.warn("No accounts left after Github filter", thrw: true);
                    return;
                }
            }

            finalAccounts = project.Lists["accs"].ToList();

            if (!finalAccounts.Any())
            {
                project.warn("No accounts left after social filters", thrw: true);
                return;
            }

            var acc0 = !string.IsNullOrEmpty(sortByTaskAge)
                ? finalAccounts.First()
                : project.RndFromList("accs", true);

            project.Var("acc0", acc0);

            if (!string.IsNullOrEmpty(sortByTaskAge))
                project.Lists["accs"].Remove(acc0);

            var left = project.Lists["accs"].Count;
            project.DbUpd($"status = 'working...'");

            var conditionsLog = string.Join("; ", conditionsByTable.Select(kvp => $"{kvp.Value}:{kvp.Key}"));
            project.SendToLog($"Account selected: acc={acc0}, remaining={left}, conditions={conditionsLog}, range={project.Var("cfgAccRange")}", LogType.Info, true, LogColor.Gray);
        }

        /// <summary>
        /// Picks an account (<c>ChooseAccountByCondition</c>) and starts the browser for it
        /// (<c>RunBrowser</c>). When the browser cannot be started in the profile folder, the next account is
        /// tried; any other error is thrown.
        /// </summary>
        /// <param name="condition">
        /// SQL condition on the account table. The word <c>NOW</c> is replaced by the current time as
        /// <c>yyyy-MM-ddTHH:mm:ss</c> when <c>sqlNow</c> is true.
        /// </param>
        /// <param name="useRange">
        /// Limit to <c>cfgAccRange</c>. Groups separated by <c>:</c> are priorities: the first group with
        /// matching accounts is used.
        /// </param>
        /// <param name="filterTwitter">Keep only accounts whose row in the <c>_twitter</c> table has <c>status = 'ok'</c>.</param>
        /// <param name="filterDiscord">Keep only accounts whose row in the <c>_discord</c> table has <c>status = 'ok'</c>.</param>
        /// <param name="filterGithub">Keep only accounts whose row in the <c>_github</c> table has <c>status = 'ok'</c>.</param>
        /// <param name="tableName">Account table; default <c>__</c> + project name.</param>
        /// <param name="debugLog">Write the queries to the log.</param>
        /// <param name="sqlNow">Replace <c>NOW</c> in the condition.</param>
        /// <param name="instance">Browser instance.</param>
        /// <param name="browser">Start Chromium; otherwise run without a browser.</param>
        /// <param name="sortByTaskAge">
        /// Column with ISO timestamps: take the account with the oldest value (empty first). When empty, a
        /// random account is taken.
        /// </param>
        /// <param name="useLegacy">Passed to <c>RunBrowser</c>.</param>
        /// <param name="useZpprofile">Passed to <c>RunBrowser</c>.</param>
        /// <param name="useFolder">Passed to <c>RunBrowser</c>.</param>
        public static void ChooseAndRunByCondition(this IZennoPosterProjectModel project,Instance instance, 
            string condition, 
            bool browser = false,
            string sortByTaskAge = null, 
            bool useRange = true, 
            bool filterTwitter = false, 
            bool filterDiscord = false, 
            bool filterGithub = false,
            string tableName = null, 
            bool debugLog = false,
            bool sqlNow = true,
            bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
        {
            
            while (true)
            {
                try
                {
                    project.ChooseAccountByCondition(condition, sortByTaskAge, useRange, filterTwitter, filterDiscord, filterGithub,
                        tableName, debugLog, sqlNow);  

                    if (string.IsNullOrEmpty(project.Var("acc0"))) 
                        throw new Exception("");
                    
                    var browserMode = browser ? "Chromium" : "WithoutBrowser";
                    //run
                    project.RunBrowser(instance,browserMode, debug:debugLog, useLegacy:useLegacy, useZpprofile:useZpprofile, useFolder:useFolder);
                    return;
		
                }
                catch (Exception ex)
                {

                    bool thrw = !ex.Message.Contains("Браузер не может быть запущен в указанной папке");
                    
                    project.warn(ex,thrw);
                    continue;
                }

            }


        }
        
       /// <summary>
       /// Counts the accounts matching the condition over all range groups, after the social filters.
       /// </summary>
       /// <param name="condition">
       /// SQL condition on the account table. The word <c>NOW</c> is replaced by the current time as
       /// <c>yyyy-MM-ddTHH:mm:ss</c> when <c>sqlNow</c> is true.
       /// </param>
       /// <param name="useRange">
       /// Limit to <c>cfgAccRange</c>. Groups separated by <c>:</c> are priorities: the first group with
       /// matching accounts is used.
       /// </param>
       /// <param name="filterTwitter">Keep only accounts whose row in the <c>_twitter</c> table has <c>status = 'ok'</c>.</param>
       /// <param name="filterDiscord">Keep only accounts whose row in the <c>_discord</c> table has <c>status = 'ok'</c>.</param>
       /// <param name="filterGithub">Keep only accounts whose row in the <c>_github</c> table has <c>status = 'ok'</c>.</param>
       /// <param name="tableName">Account table; default <c>__</c> + project name.</param>
       /// <param name="debugLog">Write the queries to the log.</param>
       /// <param name="sqlNow">Replace <c>NOW</c> in the condition.</param>
       public static int QuantityByCondition(this IZennoPosterProjectModel project,
            string condition,
            bool useRange = true,
            bool filterTwitter = false,
            bool filterDiscord = false,
            bool filterGithub = false,
            string tableName = null,
            bool debugLog = false,
            bool sqlNow = true)
        {
            if (string.IsNullOrEmpty(tableName)) 
                tableName = project.ProjectTable();
            
            
            if (sqlNow)
            {
                condition = ApplySqlNow(condition, project.DbMode()); 
            }

            
            
            // Parse priority groups from cfgAccRange
            List<string> rangeGroups = new List<string>();
            if (useRange)
            {
                var cfgAccRange = project.Var("cfgAccRange");
                rangeGroups = ParseRangeGroups(project, cfgAccRange);
            }
            else
            {
                rangeGroups.Add(null); // Single iteration without range
            }
            
            int totalCount = 0;
            
            // Iterate through each priority group
            foreach (var rangeGroup in rangeGroups)
            {
                var fullCondition = useRange
                    ? $"{condition} AND id in ({rangeGroup})"
                    : condition;

                var accounts = project.DbGetLines("id", tableName: tableName, log: debugLog, where: fullCondition)
                    .Where(acc => !string.IsNullOrWhiteSpace(acc))
                    .ToList();

                if (!accounts.Any())
                    continue;

                // Filter by Twitter if needed
                if (filterTwitter)
                {
                    var twitterFiltered = new HashSet<string>(
                        project.DbGetLines("id", $"_twitter", where: @"status = 'ok'")
                    );
                    accounts = accounts.Where(acc => twitterFiltered.Contains(acc)).ToList();
                    
                    if (!accounts.Any())
                        continue;
                }

                // Filter by Discord if needed
                if (filterDiscord)
                {
                    var discordFiltered = new HashSet<string>(
                        project.DbGetLines("id", $"_discord", where: @"status = 'ok'")
                    );
                    accounts = accounts.Where(acc => discordFiltered.Contains(acc)).ToList();
                    
                    if (!accounts.Any())
                        continue;
                }
                
                if (filterGithub)
                {
                    var discordFiltered = new HashSet<string>(
                        project.DbGetLines("id", $"_github", where: @"status = 'ok'")
                    );
                    accounts = accounts.Where(acc => discordFiltered.Contains(acc)).ToList();
                    
                    if (!accounts.Any())
                        continue;
                }

                totalCount += accounts.Count;
            }
            
            return totalCount;
        }

       
       
        // Helper method without logging
        
        private static string ApplySqlNow(string condition, string dbMode)
        {
            if (dbMode == "pgSQL")
                return condition.Replace("NOW", "to_char(NOW(), 'YYYY-MM-DD\"T\"HH24:MI:SS')");
            else if (dbMode == "SQLite") 
                return condition.Replace("NOW", "strftime('%Y-%m-%dT%H:%M:%S', 'now')");
            else 
                throw new NotImplementedException(dbMode);
        }
        private static bool FilterBySocialSilent(IZennoPosterProjectModel project, string socialName)
        {
            var accs = project.ListSync("accs");
            var filtered = new HashSet<string>(
                project.DbGetLines("id", $"_{socialName.ToLower()}", where: @"status = 'ok'")
            );

            var combined = accs
                .Where(acc => filtered.Contains(acc))
                .ToList();
            
            project.ListSync("accs", combined);
            
            return combined.Any();
        }

        private static bool FilterBySocial(this IZennoPosterProjectModel project, string socialName, string originalCondition = "")
        {
            var accs = project.ListSync("accs");
            var filtered = new HashSet<string>(
                project.DbGetLines("id", $"_{socialName.ToLower()}", where: @"status = 'ok'")
            );

            var combined = accs
                .Where(acc => filtered.Contains(acc))
                .ToList();
            
            project.ListSync("accs", combined);
            
            if (!combined.Any())
            {
                var conditionInfo = string.IsNullOrEmpty(originalCondition) ? "" : $", condition={originalCondition}";
                project.warn($"Social filter failed: social={socialName}, found=0{conditionInfo}");
                return false;
            }
            return true;
        }
    }

}
