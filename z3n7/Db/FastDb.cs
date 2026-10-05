
using System.Collections.Generic;
using System.Linq;

using System.IO;

using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;


namespace z3n7
{
    /// <summary>Shared lock object for code that must not access the database concurrently.</summary>
    public static class DbLock
    {
        /// <summary>The lock object.</summary>
        public static readonly object lockObj = new object();
    }
    
    /// <summary>
    /// SQLite access through ZennoPoster's built-in ODBC query runner, without opening own connections.
    /// </summary>
    public class FastDb
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly string _connection;
		private readonly bool _log;
        
        /// <summary>Uses the file <c>{project.Path}{dbName}.sql</c>.</summary>
        /// <param name="dbName">File name without extension; default is the <c>dbName</c> variable, or <c>db</c>.</param>
        /// <param name="log">Write queries and <c>SELECT</c> answers to the log.</param>
        public FastDb(IZennoPosterProjectModel project,string dbName = null, bool log =  false)
        {
            _project = project;
            if (string.IsNullOrEmpty(dbName))
                dbName = (string.IsNullOrEmpty(project.Var("dbName")) ? "db" : project.Var("dbName"));
            _connection = ConnectionString(dbName);
			_log = log;
        }
        private string ConnectionString(string dbName)
        {
            string pathToDb = _project.Path + dbName +".sql";
            return $"Dsn=SQLite3 Datasource;database={pathToDb}";
        }
        private string rawQ(string query)
        {
			
            return ZennoPoster.Db.ExecuteQuery(query, null,    ZennoLab.InterfacesLibrary.Enums.Db.DbProvider.Odbc,  _connection, "|", "\n", false);
        }

        /// <summary>Executes a query.</summary>
        /// <returns>Rows joined by line breaks, columns by <c>|</c>.</returns>
        public string dbString(string query)
        {
			
			if (_log) _project.SendInfoToLog($"-> {query}", true);
			var resp = rawQ(query);
			if (_log && resp.StartsWith("SELECT")) _project.SendInfoToLog($"<- {resp}",true);
            return resp;
        }
		

        /// <summary>Executes a query and returns its rows.</summary>
        public List<string> dbList(string query)
        {
            var resp = rawQ(query);
            var respList = resp.Split('\n').ToList();
            return respList;
        }
        
        /// <summary>
        /// Writes the whole table to <c>{project.Path}{fileName}</c> as CSV with a header row (UTF-8).
        /// </summary>
        public void ExportToCsv(string tableName, string fileName)
        {
            // Получаем заголовки столбцов через PRAGMA (специфично для SQLite)
            var columnsRaw = rawQ($"PRAGMA table_info({tableName})");
            var columnNames = columnsRaw.Split('\n')
                .Select(line => line.Split('|')[1]) // 1 — это индекс имени колонки в ответе PRAGMA
                .ToList();

            // Получаем все данные из таблицы, используя запятую как разделитель для CSV
            string csvData = ZennoPoster.Db.ExecuteQuery($"SELECT * FROM {tableName}", null, 
                ZennoLab.InterfacesLibrary.Enums.Db.DbProvider.Odbc, _connection, ",", "\n", false);

            // Соединяем заголовки и данные
            string fullContent = string.Join(",", columnNames) + "\n" + csvData;

            // Сохраняем файл в директорию проекта
            File.WriteAllText(_project.Path + fileName, fullContent, System.Text.Encoding.UTF8);
        }
        
        /// <summary>
        /// Writes the selected columns to <c>{project.Path}{fileName}</c> as CSV, UTF-8 with BOM so that Excel
        /// opens it correctly.
        /// </summary>
        /// <param name="columns"><c>*</c> for all columns, or a comma-separated list that is also used as the header.</param>
        public void ExportToCsv(string tableName, string fileName, string columns = "*")
        {
            // 1. Определяем заголовки для CSV
            string header;
            if (columns == "*" || string.IsNullOrEmpty(columns))
            {
                var columnsRaw = rawQ($"PRAGMA table_info({tableName})");
                header = string.Join(",", columnsRaw.Split('\n')
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Split('|')[1]));
            }
            else
            {
                header = columns; // Если колонки переданы строкой "proxy, used", они станут заголовком
            }

            // 2. Получаем данные (используем запятую как разделитель для CSV)
            string csvData = ZennoPoster.Db.ExecuteQuery($"SELECT {columns} FROM {tableName}", null, 
                ZennoLab.InterfacesLibrary.Enums.Db.DbProvider.Odbc, _connection, ",", "\n", false);

            // 3. Формируем и сохраняем файл
            // Добавляем BOM (Byte Order Mark), чтобы Excel сразу открывал UTF-8 без иероглифов
            byte[] bom = { 0xEF, 0xBB, 0xBF };
            string fullContent = header + "\n" + csvData;
            byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(fullContent);
    
            var finalBytes = bom.Concat(contentBytes).ToArray();
            File.WriteAllBytes(_project.Path + fileName, finalBytes);
        }
    }
}