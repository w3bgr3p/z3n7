using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net.Http;
using System.Text;
using System.Linq;

using System.Collections.Generic;
using System.Security.Cryptography;
using ZennoLab.InterfacesLibrary.ProjectModel;

using Newtonsoft.Json.Linq;

namespace z3n7.Utilities
{
    /// <summary>
    /// Unified GitHub manager combining git operations and REST API calls
    /// </summary>
    public class Git
    {
        #region Fields
        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _log;
        private readonly string _username;
        private readonly string _organization;
        private readonly string _branch;
        private readonly Db _db;
        private string _gitTable = "___git";
        private readonly GitCli _gitCli;
        private readonly GitHubApi _githubApi;
        #endregion

        #region Constructor
        public Git(IZennoPosterProjectModel project, string token, string username, string branch = "master", string organization = null, Logger log = null)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _log = log;
            if (_log != null) 
            {
                _log.Emoji = "Git";
            }
            
            if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("ghp_"))
                throw new ArgumentException("Invalid GitHub token. Must start with 'ghp_'", nameof(token));
                
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            _username = username;
            _organization = organization;
            _branch = branch;
            

            _db = new Db(project, dbMode: "PostgreSQL", 
                pgHost: "localhost", pgPort: "5432", pgDbName: "postgres", 
                pgUser: "postgres", pgPass: project.Var("DBpstgrPass"), 
                defaultTable: _gitTable , log: false
            );
            
            _gitCli = new GitCli(_log, token, username, organization, branch);
            _githubApi = new GitHubApi(_project, token, username, organization);        
        }
        #endregion

        #region Public API - Repository Management
        /// <summary>
        /// Create a new repository (uses REST API)
        /// </summary>
        public string CreateRepository(string repoName, bool isPrivate = true)
        {
            _log?.Send($"Creating repository: {repoName} (private: {isPrivate})");
            return _githubApi.CreateRepository(repoName, isPrivate);
        }

        /// <summary>
        /// Change repository visibility (uses REST API)
        /// </summary>
        public string ChangeVisibility(string repoName, bool makePrivate)
        {
            _log?.Send($"Changing visibility for {repoName} to {(makePrivate ? "private" : "public")}");
            return _githubApi.ChangeVisibility(repoName, makePrivate);
        }

        /// <summary>
        /// Get repository info (uses REST API)
        /// </summary>
        public string GetRepositoryInfo(string repoName)
        {
            return _githubApi.GetRepositoryInfo(repoName);
        }

        /// <summary>
        /// Delete repository (uses REST API)
        /// </summary>
        public string DeleteRepository(string repoName)
        {
            _log?.Send($"Deleting repository: {repoName}");
            return _githubApi.DeleteRepository(repoName);
        }
        #endregion

        #region Public API - Collaborators
        /// <summary>
        /// Get list of collaborators (uses REST API)
        /// </summary>
        public string GetCollaborators(string repoName)
        {
            return _githubApi.GetCollaborators(repoName);
        }

        /// <summary>
        /// Add collaborator with permission (uses REST API)
        /// </summary>
        public string AddCollaborator(string repoName, string collaboratorUsername, string permission = "pull")
        {
            _log?.Send($"Adding collaborator {collaboratorUsername} to {repoName} with {permission} permission");
            return _githubApi.AddCollaborator(repoName, collaboratorUsername, permission);
        }

        /// <summary>
        /// Remove collaborator (uses REST API)
        /// </summary>
        public string RemoveCollaborator(string repoName, string collaboratorUsername)
        {
            _log?.Send($"Removing collaborator {collaboratorUsername} from {repoName}");
            return _githubApi.RemoveCollaborator(repoName, collaboratorUsername);
        }

        /// <summary>
        /// Change collaborator permission (uses REST API)
        /// </summary>
        public string ChangeCollaboratorPermission(string repoName, string collaboratorUsername, string permission = "pull")
        {
            _log?.Send($"Changing {collaboratorUsername} permission in {repoName} to {permission}");
            return _githubApi.ChangeCollaboratorPermission(repoName, collaboratorUsername, permission);
        }
        #endregion
        
        #region Public API - Organization Members

        public string InviteToOrganization(string username, string role = "member")
        {
            _log?.Send($"Inviting {username} to organization with role: {role}");
            return _githubApi.InviteToOrganization(username, role);
        }

        public string RemoveFromOrganization(string username)
        {
            _log?.Send($"Removing {username} from organization");
            return _githubApi.RemoveFromOrganization(username);
        }

        public string GetOrganizationMembers()
        {
            return _githubApi.GetOrganizationMembers();
        }
        public void SyncFarmWithUnlock(bool dryRun = false, string tableName = "___z3nFarm")
        {
            var activeUsers = new Dictionary<string, string>(); 
            
            try
            {
                var activeRecords = _db.GetLines(
                    "owner, github",
                    tableName,
                    where: "expired = 'False' AND github != ''",
                    log: true
                );
                
                foreach (var record in activeRecords)
                {
                    var parts = record.Split('¦');
                    if (parts.Length < 2) continue;
                    
                    string owner = parts[0].Trim().ToLower();
                    string github = parts[1].Trim();
                    
                    if (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(github))
                        activeUsers[owner] = github;
                }
                
            }
            catch (Exception ex)
            {
                _log.Warn($"!W Failed to read DB: {ex.Message}");
                return;
            }
            
            if (activeUsers.Count == 0)
            {
                _log?.Send("No active users in DB, nothing to sync");
                return;
            }
            
            var currentMembers = new HashSet<string>();
            try
            {
                var membersJson = _githubApi.GetOrganizationMembers();
                var members = JArray.Parse(membersJson);
                
                foreach (var member in members)
                {
                    string login = member["login"]?.ToString();
                    if (!string.IsNullOrEmpty(login))
                        currentMembers.Add(login);
                }
            }
            catch (Exception ex)
            {
                _log.Warn(ex.Message);
                return;
            }
            var activeUsernames = new HashSet<string>(activeUsers.Values);
            var toAdd = activeUsernames.Where(u => !currentMembers.Contains(u)).ToList();
            var toRemove = currentMembers.Where(m => !activeUsernames.Contains(m)).ToList();
            
            if (toAdd.Count == 0 && toRemove.Count == 0)
            {
                return;
            }
            
            if (dryRun)
            {
                _log?.Send("⚠️ DRY RUN MODE - no actual changes");
                _log?.Send($"Would add: {string.Join(", ", toAdd)}");
                _log?.Send($"Would remove: {string.Join(", ", toRemove)}");
                return;
            }
            
            foreach (var username in toAdd)
            {
                _log?.Send($"[ADD] {username}");
                
                var result = _githubApi.InviteToOrganization(username, role: "member");
                if (result.Contains("Error") || result.Contains("error"))
                {
                    _log.Warn(result);
                }
                else
                {
                    _log?.Send(result);
                }
                
                Thread.Sleep(1000); // Rate limit protection
            }
            

            foreach (var username in toRemove)
            {
                _log?.Send($"[REMOVE] {username}");
                var result = _githubApi.RemoveFromOrganization(username);
                if (result.Contains("Error") || result.Contains("error"))
                {
                    _log.Warn(result);
                }
                else
                {
                    _log?.Send(result);
                }
                
                Thread.Sleep(1000);
            }
            
        }

        #endregion

        #region Public API - Code Sync (Git Operations)
        /// <summary>
        /// Sync local repositories to GitHub (uses git CLI)
        /// Reads configuration from database and processes all enabled projects
        /// </summary>
        public void SyncRepositories(string baseDir, string commitMessage = "ts")
        {
            if (!Directory.Exists(baseDir))
            {
                _log?.Send($"[!W]: Directory '{baseDir}' does not exist!");
                Thread.Sleep(5000);
                return;
            }

            var projectsList = LoadProjectsConfiguration(baseDir);
            if (projectsList.Count == 0)
            {
                _log?.Send("[!W]: No projects found in database!");
                return;
            }

            var stats = new GitCli.SyncStatistics();
            stats.TotalFolders = projectsList.Count;
    
            _log?.Send($"Processing base directory: {baseDir}");
            _log?.Send($"Target: {GetGitHubPath()}");
            _log?.Send($"Syncing {stats.TotalFolders} folders");

            foreach (string projectToSync in projectsList)
            {
                ProcessSingleProject(baseDir, projectToSync, commitMessage, stats);
            }

            LogSummary(stats);
        }

        /// <summary>
        /// Push single directory to GitHub (uses git CLI)
        /// </summary>
        public void PushDirectory(string localPath, string repoName, string commitMessage = "ts", bool createIfNotExists = true)
        {
            if (!Directory.Exists(localPath))
            {
                _log?.Send($"[!W]: Directory '{localPath}' does not exist!");
                return;
            }

            // Create repo if needed
            if (createIfNotExists)
            {
                var repoInfo = GetRepositoryInfo(repoName);
                if (repoInfo.Contains("\"message\":\"Not Found\""))
                {
                    _log?.Send($"Repository {repoName} not found, creating...");
                    CreateRepository(repoName);
                    Thread.Sleep(2000);
                }
            }

            var stats = new GitCli.SyncStatistics { TotalFolders = 1 };
            string repoUrl = _gitCli.BuildRepositoryUrl(repoName);
            var sizeCheck = _gitCli.CheckRepositorySize(localPath);
            
            if (!sizeCheck.IsValid)
            {
                _log?.Send($"[!W]: {sizeCheck.Reason}");
                return;
            }

            _gitCli.PerformGitOperations(localPath, repoUrl, commitMessage, sizeCheck, stats);
            LogSummary(stats);
        }

        /// <summary>
        /// Clone repository to local directory (uses git CLI)
        /// </summary>
        public void CloneRepository(string repoName, string targetPath)
        {
            _log?.Send($"Cloning {repoName} to {targetPath}");
            string repoUrl = _gitCli.BuildRepositoryUrl(repoName);
            _gitCli.Clone(repoUrl, targetPath);
        }

        /// <summary>
        /// Pull latest changes (uses git CLI)
        /// </summary>
        public void PullChanges(string localPath)
        {
            _log?.Send($"Pulling changes in {localPath}");
            _gitCli.Pull(localPath, _branch);
        }
        #endregion

        #region Public API - Utilities
        /// <summary>
        /// Calculate MD5 hash of a file
        /// </summary>
        public static string GetFileHash(string filePath)
        {
            return GitCli.GetFileHash(filePath);
        }

        /// <summary>
        /// Get current GitHub path (owner/repo pattern)
        /// </summary>
        public string GetGitHubPath()
        {
            return string.IsNullOrWhiteSpace(_organization) 
                ? $"github.com/{_username}" 
                : $"github.com/{_organization} (user: {_username})";
        }
        #endregion

        #region Private Helpers
        private List<string> LoadProjectsConfiguration(string baseDir)
        {
            var projectsList = new List<string>();
            try
            {
                var allProjects = _db.GetLines("name", _gitTable, where: "\"name\" != ''", log: true);
                foreach (var projectName in allProjects)
                {
                    var ghSynced = _db.Get("gh_synced", _gitTable, where: $"\"name\" = '{projectName}'", log: true);
                    projectsList.Add($"{projectName} : {ghSynced}");
                }
            }
            catch (Exception ex)
            {
                _project.SendWarningToLog(ex.Message);
            }
            return projectsList;
        }

        private void ProcessSingleProject(string baseDir, string projectToSync, string commitMessage, GitCli.SyncStatistics stats)
        {
            try
            {
                if (projectToSync.Contains("false"))
                {
                    _log?.Send($"[SKIP] (sync is off for: {projectToSync})");
                    stats.FoldersSkipped++;
                    return;
                }

                string subDir = Path.Combine(baseDir, projectToSync.Split(':')[0].Trim());
                string projectName = Path.GetFileName(subDir);
        
                string currentHash = CalculateDirectoryHash(subDir);
                string lastLocalHash = _db.Get("last_commit_hash", _gitTable, where: $"\"name\" = '{projectName}'", log: false);
                string remoteHash = _githubApi.GetLastCommitHash(projectName, _branch);

                bool localChanged = string.IsNullOrEmpty(lastLocalHash) || currentHash != lastLocalHash;
                bool needsSync = remoteHash == null || localChanged;

                if (!needsSync)
                {
                    _log?.Send($"[SKIP] (no changes): {projectName}");
                    stats.FoldersSkipped++;
                    return;
                }

                string repoUrl = _gitCli.BuildRepositoryUrl(projectName);
                var sizeCheck = _gitCli.CheckRepositorySize(subDir);
        
                if (!sizeCheck.IsValid)
                {
                    _log?.Send($"Skipped {subDir}: {sizeCheck.Reason}");
                    stats.FoldersSkippedBySize++;
                    return;
                }

                bool repoJustCreated = EnsureRepositoryExists(projectName);
                _gitCli.PerformGitOperations(subDir, repoUrl, commitMessage, sizeCheck, stats, forceSync: repoJustCreated || remoteHash == null);
        
                _db.Upd($"last_commit_hash = '{currentHash}'", _gitTable, 
                    where: $"\"name\" = '{projectName}'", log: false);
            }
            catch (Exception ex)
            {
                _log?.Send($"[!W]: {ex.Message}");
                stats.ErrorCount++;
                Thread.Sleep(5000);
            }
        }
        private bool EnsureRepositoryExists(string repoName)
        {
            var repoInfo = _githubApi.GetRepositoryInfo(repoName);
            _log?.Send($"[DEBUG] GetRepositoryInfo({repoName}): {repoInfo}");
            
            bool notFound = repoInfo.Contains("\"message\":\"Not Found\"") 
                            || repoInfo.StartsWith("404") 
                            || repoInfo.Contains("404 Not Found")
                            || repoInfo.Contains("Connection Failed");

            if (notFound)
            {
                _log?.Send($"[CREATE] Repository {repoName} not found, creating...");
                _githubApi.CreateRepository(repoName, isPrivate: true);
        
                // Ждём, пока репозиторий станет доступен
                int retries = 10;
                for (int i = 0; i < retries; i++)
                {
                    Thread.Sleep(1000);
                    var check = _githubApi.GetRepositoryInfo(repoName);
                    if (!check.Contains("\"message\":\"Not Found\""))
                    {
                        _log?.Send($"[CREATE] Repository {repoName} ready after {i+1}s");
                        return true;
                    }
                }
                _log?.Send($"[!W] Repository {repoName} created but not accessible");
                return true;
            }
            return false;
        }
        private string CalculateDirectoryHash(string directory)
        {
            var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                .Where(f => !f.Contains("\\.git\\"))
                .OrderBy(f => f)
                .ToArray();

            using (var md5 = MD5.Create())
            {
                foreach (string file in files)
                {
                    try
                    {
                        byte[] pathBytes = Encoding.UTF8.GetBytes(file.Substring(directory.Length));
                        md5.TransformBlock(pathBytes, 0, pathBytes.Length, null, 0);

                        byte[] contentBytes = File.ReadAllBytes(file);
                        md5.TransformBlock(contentBytes, 0, contentBytes.Length, null, 0);
                    }
                    catch { }
                }

                md5.TransformFinalBlock(new byte[0], 0, 0);
                return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
            }
        }
        private void LogSummary(GitCli.SyncStatistics stats)
        {
            _log?.Send($"=======================Summary======================= \n" +
                     $"Total={stats.TotalFolders}, Changes={stats.FoldersWithChanges}, " +
                     $"Skipped={stats.FoldersSkipped}, SizeSkipped={stats.FoldersSkippedBySize}, " +
                     $"Committed={stats.SuccessfullyCommitted}, Failed={stats.ErrorCount}");
        }
        #endregion
        
        #region Nested Classes
        /// <summary>
        /// Handles git CLI operations (push, pull, commit, etc.)
        /// </summary>
        private class GitCli
        {
            private readonly Logger _log;
            private readonly string _token;
            private readonly string _username;
            private readonly string _organization;
            private readonly string _branch;

            private const long MAX_FILE_SIZE_MB = 100;
            private const long MAX_TOTAL_SIZE_MB = 1000;
            private const int DELAY_BETWEEN_REPOS_MS = 2000;
            private const int MAX_FILES_COUNT = 10000;

            private readonly string[] EXCLUDED_EXTENSIONS = {
                ".exe", ".so", ".dylib", ".bin", ".obj", ".lib", ".a",
                ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2",
                ".iso", ".img", ".dmg", ".msi", ".deb", ".rpm",
                ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv",
                ".mp3", ".wav", ".flac", ".ogg", ".m4a",
                ".psd", ".ai", ".sketch", ".fig",
                ".db", ".sqlite", ".mdb", ".accdb"
            };

            public class SyncStatistics
            {
                public int TotalFolders { get; set; }
                public int FoldersWithChanges { get; set; }
                public int FoldersSkipped { get; set; }
                public int SuccessfullyCommitted { get; set; }
                public int ErrorCount { get; set; }
                public int FoldersSkippedBySize { get; set; }
            }

            public class SizeCheckResult
            {
                public bool IsValid { get; set; }
                public string Reason { get; set; }
                public double TotalSizeMB { get; set; }
                public int FilesCount { get; set; }
            }

            public GitCli(Logger log, string token, string username, string organization, string branch)
            {
                _log = log;
                _token = token;
                _username = username;
                _organization = organization;
                _branch = branch;
            }

            public string BuildRepositoryUrl(string projectName)
            {
                string owner = string.IsNullOrWhiteSpace(_organization) ? _username : _organization;
                return $"https://{_token}@github.com/{owner}/{projectName}.git";
            }

            public void PerformGitOperations(string subDir, string repoUrl, string commitMessage, SizeCheckResult sizeCheck, SyncStatistics stats, bool forceSync = false)
            {
                ConfigureSafeDirectory(subDir);

                if (!Directory.Exists(Path.Combine(subDir, ".git")))
                {
                    RunGit("init", subDir);
                    RunGit($"checkout -b {_branch}", subDir);
                }

                RunGit($"config user.name \"{_username}\"", subDir);
                RunGit($"config user.email \"{_username}@users.noreply.github.com\"", subDir);

                if (!RunGit("remote -v", subDir).Contains("origin"))
                {
                    RunGit($"remote add origin {repoUrl}", subDir);
                }
                else
                {
                    RunGit($"remote set-url origin {repoUrl}", subDir);
                }

                CreateOrUpdateGitignore(subDir);
                RunGit("add .", subDir);

                string status = RunGit("status --porcelain", subDir);
                if (string.IsNullOrWhiteSpace(status))
                {
                    if (forceSync)
                    {
                        // Есть коммиты но нет изменений — просто пушим
                        _log?.Send($"[PUSH] (force, no local changes): {subDir}");
                        RunGit($"push origin {_branch} --force", subDir);
                        stats.SuccessfullyCommitted++;
                    }
                    else
                    {
                        _log?.Send($"[SKIP] (No changes): {subDir}");
                        stats.FoldersSkipped++;
                    }
                    return;
                }

                if (commitMessage == "ts")
                    commitMessage = DateTime.UtcNow.ToString("o");

                stats.FoldersWithChanges++;
                RunGit($"commit -m \"{commitMessage}\"", subDir);
                RunGit($"push origin {_branch} --force", subDir);

                _log?.Send($"[COMMIT] {subDir} ({sizeCheck.TotalSizeMB:F1}MB, {sizeCheck.FilesCount} files)");
                stats.SuccessfullyCommitted++;
                Thread.Sleep(DELAY_BETWEEN_REPOS_MS);
            }
            public void Clone(string repoUrl, string targetPath)
            {
                if (Directory.Exists(targetPath))
                {
                    _log?.Send($"[!W]: Directory {targetPath} already exists!");
                    return;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                RunGitGlobal($"clone {repoUrl} \"{targetPath}\"");
            }

            public void Pull(string localPath, string branch)
            {
                if (!Directory.Exists(Path.Combine(localPath, ".git")))
                {
                    _log?.Send($"[!W]: {localPath} is not a git repository!");
                    return;
                }

                RunGit($"pull origin {branch}", localPath);
            }

            public SizeCheckResult CheckRepositorySize(string directory)
            {
                var result = new SizeCheckResult { IsValid = true };

                try
                {
                    var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                        .Where(f => !f.Contains("\\.git\\")).ToArray();

                    result.FilesCount = files.Length;

                    if (result.FilesCount > MAX_FILES_COUNT)
                    {
                        result.IsValid = false;
                        result.Reason = $"Too many files ({result.FilesCount} > {MAX_FILES_COUNT})";
                        return result;
                    }

                    double totalSizeMB = 0;
                    foreach (string file in files)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            double fileSizeMB = fileInfo.Length / (1024.0 * 1024.0);
                            totalSizeMB += fileSizeMB;

                            string extension = Path.GetExtension(file).ToLower();
                            if (fileSizeMB > MAX_FILE_SIZE_MB || (EXCLUDED_EXTENSIONS.Contains(extension) && fileSizeMB > 1))
                            {
                                result.IsValid = false;
                                result.Reason = $"Contains large or binary files";
                                return result;
                            }
                        }
                        catch { continue; }
                    }

                    result.TotalSizeMB = totalSizeMB;
                    if (totalSizeMB > MAX_TOTAL_SIZE_MB)
                    {
                        result.IsValid = false;
                        result.Reason = $"Repository too large ({totalSizeMB:F1}MB > {MAX_TOTAL_SIZE_MB}MB)";
                    }
                }
                catch (Exception ex)
                {
                    result.IsValid = false;
                    result.Reason = $"Size check error: {ex.Message}";
                }

                return result;
            }

            public static string GetFileHash(string filePath)
            {
                try
                {
                    using (var md5 = MD5.Create())
                    using (var stream = File.OpenRead(filePath))
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }

            private void CreateOrUpdateGitignore(string directory)
            {
                try
                {
                    string gitignorePath = Path.Combine(directory, ".gitignore");
                    var rules = new[] {
                        "*.exe", "*.so", "*.dylib", "*.bin", "*.obj", "*.lib", "*.a",
                        "*.zip", "*.rar", "*.7z", "*.tar", "*.gz", "*.bz2",
                        "*.mp4", "*.avi", "*.mkv", "*.mov", "*.mp3", "*.wav",
                        "*.db", "*.sqlite", "*.mdb",
                        ".vs/", ".vscode/", "*.user", "*.suo", "Thumbs.db", ".DS_Store",
                        "node_modules/", "packages/", "bin/", "obj/", "build/", "dist/"
                    };

                    if (!File.Exists(gitignorePath))
                    {
                        File.WriteAllLines(gitignorePath, rules);
                    }
                    else
                    {
                        var existing = File.ReadAllText(gitignorePath);
                        var missing = rules.Where(r => !existing.Contains(r)).ToArray();
                        if (missing.Any())
                        {
                            File.AppendAllLines(gitignorePath, missing);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log?.Send($"Warning: Failed to update .gitignore in {directory}: {ex.Message}");
                }
            }

            private void ConfigureSafeDirectory(string directory)
            {
                try
                {
                    string normalizedPath = Path.GetFullPath(directory).Replace('\\', '/');
                    if (!RunGitGlobal("config --global --get-all safe.directory").Contains(normalizedPath))
                    {
                        RunGitGlobal($"config --global --add safe.directory \"{normalizedPath}\"");
                    }
                }
                catch (Exception ex)
                {
                    _log?.Send($"Warning: Failed to configure safe directory {directory}: {ex.Message}");
                }
            }

            private string RunGit(string args, string workingDir)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = args,
                    WorkingDirectory = workingDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Git failed: {args}. Error: {error}");
                    }

                    return output;
                }
            }

            private string RunGitGlobal(string args)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Git global failed: {args}. Error: {error}");
                    }

                    return output;
                }
            }
        }

        /// <summary>
        /// Handles GitHub REST API calls
        /// </summary>
       private class GitHubApi
{
    private readonly IZennoPosterProjectModel _project;
    private readonly string _token;
    private readonly string _username;
    private readonly string _organization;
    private const string BASE_URL = "https://api.github.com/";

    public GitHubApi(IZennoPosterProjectModel project, string token, string username, string organization)
    {
        _project = project;
        _token = token;
        _username = username;
        _organization = organization;
    }

    private string[] GetHeaders()
    {
        return new[]
        {
            "Authorization: token " + _token,
            "User-Agent: z3n7-GitHubManager",
            "Accept: application/vnd.github.v3+json"
        };
    }

    public string GetRepositoryInfo(string repoName)
    {
        string url = $"{BASE_URL}repos/{GetOwner()}/{repoName}";
        return _project.GET(url, headers: GetHeaders(), cookies: "-", log: true, useNetHttp:true);
    }

    public string CreateRepository(string repoName, bool isPrivate = true)
    {
        string endpoint = string.IsNullOrWhiteSpace(_organization)
            ? $"{BASE_URL}user/repos"
            : $"{BASE_URL}orgs/{_organization}/repos";

        string body = "{\"name\":\"" + repoName + "\",\"private\":" + isPrivate.ToString().ToLower() + "}";
        return _project.POST(endpoint, body, headers: GetHeaders(), cookies: "-", log: true);
    }

    public string ChangeVisibility(string repoName, bool makePrivate)
    {
        string url = $"{BASE_URL}repos/{GetOwner()}/{repoName}";
        string body = "{\"private\":" + makePrivate.ToString().ToLower() + "}";
        return _project.PUT(url, body, headers: GetHeaders(), cookies: "-", log: true);
    }

    public string DeleteRepository(string repoName)
    {
        string url = $"{BASE_URL}repos/{GetOwner()}/{repoName}";
        return _project.DELETE(url, headers: GetHeaders(), cookies: "-", log: true);
    }

    public string GetCollaborators(string repoName)
    {
        string url = $"{BASE_URL}repos/{GetOwner()}/{repoName}/collaborators";
        return _project.GET(url, headers: GetHeaders(), cookies: "-", log: true);
    }

    public string AddCollaborator(string repoName, string collaboratorUsername, string permission = "pull")
    {
        string url = $"{BASE_URL}repos/{GetOwner()}/{repoName}/collaborators/{collaboratorUsername}";
        string body = "{\"permission\":\"" + permission + "\"}";
        return _project.PUT(url, body, headers: GetHeaders(), cookies: "-", log: true);
    }

    public string RemoveCollaborator(string repoName, string collaboratorUsername)
    {
        string url = $"{BASE_URL}repos/{GetOwner()}/{repoName}/collaborators/{collaboratorUsername}";
        return _project.DELETE(url, headers: GetHeaders(), cookies: "-", log: true);
    }

    public string ChangeCollaboratorPermission(string repoName, string collaboratorUsername, string permission = "pull")
    {
        return AddCollaborator(repoName, collaboratorUsername, permission);
    }

    public string InviteToOrganization(string username, string role = "member")
    {
        if (string.IsNullOrWhiteSpace(_organization))
            return "Error: No organization configured";

        string url = $"{BASE_URL}orgs/{_organization}/memberships/{username}";
        string body = "{\"role\":\"" + role + "\"}";
        return _project.PUT(url, body, headers: GetHeaders(), cookies: "-", log: true);
    }

    public string RemoveFromOrganization(string username)
    {
        if (string.IsNullOrWhiteSpace(_organization))
            return "Error: No organization configured";

        string url = $"{BASE_URL}orgs/{_organization}/members/{username}";
        return _project.DELETE(url, headers: GetHeaders(), cookies: "-", log: true);
    }

    public string GetOrganizationMembers()
    {
        if (string.IsNullOrWhiteSpace(_organization))
            return "Error: No organization configured";

        string url = $"{BASE_URL}orgs/{_organization}/members";
        return _project.GET(url, headers: GetHeaders(), cookies: "-", log: true);
    }

    private string GetOwner()
    {
        return string.IsNullOrWhiteSpace(_organization) ? _username : _organization;
    }
    
    public string GetLastCommitHash(string repoName, string branch = "master")
    {
        string url = $"{BASE_URL}repos/{GetOwner()}/{repoName}/commits/{branch}";
        string response = _project.GET(url, headers: GetHeaders(), cookies: "-", log: false);
    
        if (response.StartsWith("404") 
            || response.Contains("Repository is empty")
            || response.Contains("Not Found") 
            || response.Contains("is empty")
            || response.Contains("Connection Failed"))
            return null;
    
        try
        {
            var json = JObject.Parse(response);
            return json["sha"]?.ToString();
        }
        catch
        {
            return null;
        }
    }
}
        #endregion
        
        public string GenerateSyncReport()
        {
            string owner = string.IsNullOrWhiteSpace(_organization) ? _username : _organization;
    
            var allSynced = _db.GetLines("name, last_upd, public", _gitTable, 
                where: "\"gh_synced\" = 'True'", log: false);
    
            var sb = new StringBuilder();
            sb.AppendLine($"📦 *current projects* : {allSynced.Count}");
            sb.AppendLine($"`{DateTime.Now:yyyy-MM-dd HH:mm:ss}`");
            sb.AppendLine();
            if (allSynced.Count > 0)
            {
                foreach (var line in allSynced)
                {
                    
                    var parts = line.Split('¦');
                    if (parts.Length < 3) continue;
            
                    string name = parts[0].Trim();
                    string lastUpd = parts[1].Trim();
                    bool isPublic = parts[2].Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
            
                    string repoUrl = $"https://github.com/{owner}/{name}";
                    string visibility = isPublic ? "🌐" : "🔒";
                    sb.AppendLine($"{visibility} [{name}]({repoUrl}) `Updated: {lastUpd}`");
                }
        
                
            }
    
            sb.AppendLine();
            
    
            return sb.ToString();
        }


        
    }
}

namespace z3n7.Git
{
    /// <summary>
    /// Утилита для версионирования проектов ZennoPoster и управления сборками z3n7.
    /// </summary>
    public class Snapper
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Db _db;
        private string _gitTable = "___git";
        
        public Snapper(IZennoPosterProjectModel project, string pgPass = null, string tableName = null)
        {
            _project = project;
            if (!string.IsNullOrEmpty(tableName)) 
                _gitTable = tableName;
            _db = new Db(project, dbMode: "PostgreSQL", 
                pgHost: "localhost", pgPort: "5432", pgDbName: "postgres", pgUser: "postgres", pgPass: pgPass, 
                defaultTable: _gitTable
            );
        }
        
        /// <summary>
        /// Выполняет резервное копирование файлов текущего проекта на основе хеш-сумм.
        /// </summary>
        public void SnapDir(string pathProjects = null, string pathSnaps = null, string pathCs = null)
        {
            if (string.IsNullOrEmpty(pathSnaps)) 
                pathSnaps = _project.Var("snapsDir");
            if (string.IsNullOrEmpty(pathProjects)) 
                pathProjects = _project.Path;

            EnsureProjectsTable();
    
            // ИЗМЕНЕНИЕ: только .zp файлы
            var files = Directory.GetFiles(pathProjects, "*.zp", SearchOption.TopDirectoryOnly);
    
            CleanupDeletedProjects(files, pathProjects);
            ProcessProjectFiles(files, pathSnaps, pathCs);
        }
        private void CleanupDeletedProjects(string[] currentFiles, string pathProjects)
        {
            // Получить все проекты из БД
            var dbProjects = _db.GetLines("name", _gitTable, where: "\"name\" != ''", log: false);
    
            // Создать HashSet из текущих файлов (без расширения)
            var currentProjects = new HashSet<string>(
                currentFiles.Select(f => Path.GetFileNameWithoutExtension(f))
            );
    
            int deleted = 0;
            foreach (var projectName in dbProjects)
            {
                if (!currentProjects.Contains(projectName))
                {
                    _db.Del(_gitTable, where: $"\"name\" = '{projectName}'", log: true);
                    _project.log($"[CLEANUP] Removed from DB: {projectName}");
                    deleted++;
                }
            }
    
            if (deleted > 0)
                _project.SendInfoToLog($"Cleaned up {deleted} deleted projects from DB");
        }
        
        

        private void EnsureProjectsTable()
        {
            var tableStructure = new Dictionary<string, string>
            {
                { "id", "SERIAL PRIMARY KEY" },
                { "name", "TEXT" },
                { "creation", "TEXT" },
                { "last_upd", "TEXT" },
                { "dir_size", "TEXT" },
                { "files_count", "TEXT" },
                { "have_cs", "TEXT" },
                { "gh_synced", "TEXT" },
                { "public", "TEXT" },
                { "last_commit_hash", "TEXT" }
            };

            _db.CreateTable(tableStructure, _gitTable, log: false);
        }


        private void ProcessProjectFiles(string[] files, string pathSnaps, string pathCs)
        {
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string projectName = fileName.Split('.')[0];
                string projectDir = Path.Combine(pathSnaps, projectName);
                string snapDir = Path.Combine(projectDir, "snaphots");

                if (!Directory.Exists(snapDir)) 
                    Directory.CreateDirectory(snapDir);

                // Создать снапшот основного файла
                CreateSnapshotIfChanged(file, fileName, projectDir, snapDir);

                // Проверить CS файл
                bool hasCs = false;
                if (!string.IsNullOrEmpty(pathCs))
                {
                    string csFileName = $"{projectName.ToLower()}.cs";
                    string csFilePath = Path.Combine(pathCs, csFileName);
                    hasCs = File.Exists(csFilePath);
                    
                    if (hasCs)
                    {
                        CreateSnapshotIfChanged(csFilePath, csFileName, projectDir, snapDir);
                    }
                }

                // Посчитать размер и файлы ПОСЛЕ создания снапшотов
                long dirSize = 0;
                int filesCount = 0;
                if (Directory.Exists(projectDir))
                {
                    var allFiles = Directory.GetFiles(projectDir, "*", SearchOption.AllDirectories);
                    dirSize = allFiles.Sum(f => new FileInfo(f).Length);
                    filesCount = allFiles.Length;
                }

                // Даты создания/изменения директории
                string creationDate = Directory.GetCreationTime(projectDir).ToString("yyyy-MM-dd HH:mm:ss");
                string lastModified = Directory.GetLastWriteTime(projectDir).ToString("yyyy-MM-dd HH:mm:ss");

                // Создать/обновить запись в БД
                EnsureProjectInDb(projectName, hasCs, dirSize, filesCount, creationDate, lastModified);
            }
        }

        private void EnsureProjectInDb(string projectName, bool hasCs, long dirSize, int filesCount, string creationDate, string lastModified)
        {
            var exists = _db.Get("name", where: $"\"name\" = '{projectName}'");
            string sizeKb = (dirSize / 1024).ToString();

            _project.log($"[DB] {projectName}: hasCs={hasCs}, size={sizeKb}KB, files={filesCount}, creation={creationDate}, modified={lastModified}");

            if (string.IsNullOrEmpty(exists))
            {
                var columns = "\"name\", \"creation\", \"last_upd\", \"dir_size\", \"files_count\", \"have_cs\", \"gh_synced\", \"public\", \"last_commit_hash\"";
                var values = $"'{projectName}', '{creationDate}', '{lastModified}', '{sizeKb}', '{filesCount}', '{hasCs}', 'false', 'false', ''";
                _db.Query($"INSERT INTO \"{_gitTable}\" ({columns}) VALUES ({values})", log: true);
            }
            else
            {
                _db.Upd(
                    $"creation = '{creationDate}', last_upd = '{lastModified}', dir_size = '{sizeKb}', files_count = '{filesCount}', have_cs = '{hasCs}'",
                    where: $"\"name\" = '{projectName}'",
                    log: true
                );
            }
        }

        public void UpdateProjectAccess(string projectName, bool ghSynced, bool isPublic, bool personalAccess)
        {
            var updates = new List<string>();
            
            if (ghSynced) updates.Add("gh_synced = 'true'");
            if (isPublic) updates.Add("public = 'true'");
            
            if (updates.Count > 0)
            {
                _db.Upd(
                    string.Join(", ", updates), 
                    _gitTable, 
                    log: true, 
                    where: $"\"name\" = '{projectName}'"
                );
            }
        }

        private void CreateSnapshotIfChanged(string filePath, string fileName, string projectDir, string snapDir)
        {
            string fileHash = Utilities .Git.GetFileHash(filePath);
            
            if (HashExistsInSnaps(snapDir, fileHash))
                return;

            // Update main copy
            string mainCopy = Path.Combine(projectDir, fileName);
            File.Copy(filePath, mainCopy, overwrite: true);
            _project.log($"[UPDATED]: {mainCopy}");

            // Create timestamped snapshot
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
            string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string snapPath = Path.Combine(snapDir, $"{timestamp}.{nameWithoutExt}{extension}");
            File.Copy(filePath, snapPath, overwrite: false);
            _project.log($"[SNAP]: {snapPath}");
        }

        private bool HashExistsInSnaps(string snapDir, string targetHash)
        {
            var snaps = Directory.GetFiles(snapDir, "*", SearchOption.TopDirectoryOnly);
            foreach (string snapFile in snaps)
            {
                if (Utilities.Git.GetFileHash(snapFile) == targetHash)
                    return true;
            }
            return false;
        }
        
        private readonly object LockObject = new object();
        public void CopyDir(string sourceDir, string destDir)
        {
            if (!Directory.Exists(sourceDir)) throw new DirectoryNotFoundException("Source directory does not exist: " + sourceDir);
            lock (LockObject)
            {
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

                DirectoryInfo source = new DirectoryInfo(sourceDir);
                DirectoryInfo target = new DirectoryInfo(destDir);


                foreach (FileInfo file in source.GetFiles())
                {
                    string targetFilePath = Path.Combine(target.FullName, file.Name);
                    file.CopyTo(targetFilePath, true);
                }

                foreach (DirectoryInfo subDir in source.GetDirectories())
                {
                    string targetSubDirPath = Path.Combine(target.FullName, subDir.Name);
                    CopyDir(subDir.FullName, targetSubDirPath);
                }
            }
        }

        /// <summary>
        /// Архивирует текущую версию z3n7.dll, фиксирует зависимости и обновляет рабочие проекты на "ферме".
        /// </summary>
        public void SnapCoreDll()
        {
            var paths = GetCorePaths();
            var (dllVersion, zpVersion) = GetVersions(paths.DllPath, paths.ProcessDir);

            _project.SendInfoToLog($"ZP: v{zpVersion}, z3n7: v{dllVersion}");
            _project.Var("vZP", zpVersion);
            _project.Var("vDll", dllVersion);

            CopyAssemblies(paths);
            ArchiveVersion(paths, dllVersion);
            UpdateProjects(paths);
        }
        
        #region SnapCoreDll Methods

        private CorePaths GetCorePaths()
        {
            string currentProcessPath = Process.GetCurrentProcess().MainModule.FileName;
            string processDir = Path.GetDirectoryName(currentProcessPath);
            string externalAssemblies = Path.Combine(processDir, "ExternalAssemblies");

            return new CorePaths
            {
                ProcessDir = processDir,
                ExternalAssemblies = externalAssemblies,
                DllPath = Path.Combine(externalAssemblies, "z3n7.dll"),
                z3n7Repo = @"w:\code_hard\.net\z3n7\ExternalAssemblies\",
                Z3nFarmRepo = @"w:\work_hard\zenoposter\CURRENT_JOBS\.snaps\z3nFarm\",
                SnapsBase = @"w:\work_hard\zenoposter\CURRENT_JOBS\.snaps\",
                VersionsBase = @"w:\code_hard\.net\z3n7\verions\"
            };
        }

        private (string dllVersion, string zpVersion) GetVersions(string dllPath, string processDir)
        {
            string dllVersion = FileVersionInfo.GetVersionInfo(dllPath).FileVersion;
            string zpVersion = processDir.Split('\\')[5];
            return (dllVersion, zpVersion);
        }

        private void CopyAssemblies(CorePaths paths)
        {
            var sourceFiles = Directory.GetFiles(paths.ExternalAssemblies, "*", SearchOption.TopDirectoryOnly);
            _project.log($"Copying {sourceFiles.Length} files from ExternalAssemblies");

            CopyDir(paths.ExternalAssemblies, Path.Combine(paths.Z3nFarmRepo, "ExternalAssemblies"));
            CopyDir(paths.ExternalAssemblies, paths.z3n7Repo);
        }

        private void ArchiveVersion(CorePaths paths, string dllVersion)
        {
            string versionDir = Path.Combine(paths.VersionsBase, $"v{dllVersion}");
            string versionDll = Path.Combine(versionDir, "z3n7.dll");

            if (!Directory.Exists(versionDir))
            {
                Directory.CreateDirectory(versionDir);
                _project.log($"Created: {versionDir}");
            }

            File.Copy(paths.DllPath, versionDll, true);

            // Create dependencies file
            string depsFile = Path.Combine(versionDir, "dependencies.txt");
            var dllFiles = Directory.GetFiles(paths.z3n7Repo, "*.dll", SearchOption.TopDirectoryOnly);
            using (var writer = new StreamWriter(depsFile, false))
            {
                foreach (var file in dllFiles)
                {
                    var info = FileVersionInfo.GetVersionInfo(file);
                    writer.WriteLine($"{Path.GetFileName(file)} : {info.FileVersion}");
                }
            }
            _project.log($"Archived v{dllVersion} + {dllFiles.Length} deps");
        }

        private void UpdateProjects(CorePaths paths)
        {
            var projectUpdates = new[]
            {
                //new { SourceDir = "_z3nLnch", SourceFile = "_z3nLnch.zp", TargetFile = "z3nLauncher.zp" },
                new { SourceDir = "SAFU", SourceFile = "SAFU.zp", TargetFile = "SAFU.zp" },
                new { SourceDir = "DbBuilder", SourceFile = "DbBuilder.zp", TargetFile = "DbBuilder.zp" }
            };

            // Clean old files
            foreach (var proj in projectUpdates)
            {
                string targetPath = Path.Combine(paths.Z3nFarmRepo, proj.TargetFile);
                if (File.Exists(targetPath)) 
                    File.Delete(targetPath);
            }

            // Copy new versions
            int updated = 0, missing = 0;
            foreach (var proj in projectUpdates)
            {
                string sourcePath = Path.Combine(paths.SnapsBase, proj.SourceDir, proj.SourceFile);
                string targetPath = Path.Combine(paths.Z3nFarmRepo, proj.TargetFile);

                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath, true);
                    updated++;
                }
                else
                {
                    missing++;
                }
            }
            _project.log($"Projects: {updated} updated, {missing} missing");
        }

        #endregion

        private class CorePaths
        {
            public string ProcessDir { get; set; }
            public string ExternalAssemblies { get; set; }
            public string DllPath { get; set; }
            public string z3n7Repo { get; set; }
            public string Z3nFarmRepo { get; set; }
            public string SnapsBase { get; set; }
            public string VersionsBase { get; set; }
        }
    }
}