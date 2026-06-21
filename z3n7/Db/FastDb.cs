using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using z3nIO;
using System.Net.Http;

namespace z3nIO
{
    public static class DbLock
    {
        public static readonly object lockObj = new object();
    }
    
    public class FastDb
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly string _connection;
		private readonly bool _log;
        
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

        public string dbString(string query)
        {
			
			if (_log) _project.SendInfoToLog($"-> {query}");
			var resp = rawQ(query);
			if (_log && resp.StartsWith("SELECT")) _project.SendInfoToLog($"<- {resp}");
            return resp;
        }
		

        public List<string> dbList(string query)
        {
            var resp = rawQ(query);
            var respList = resp.Split('\n').ToList();
            return respList;
        }
        
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