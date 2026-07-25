using System.Collections.Generic;

namespace z3n7
{
    /// <summary>
    /// Централизованное хранилище имён таблиц с дефолтными значениями.
    ///
    /// Источники:
    ///   TaskManager.cs  — _settings, _tasks, _commands
    ///   DbExtencions.cs — _wlt (хардкод на строке 150, в SqlGet цепочки кошелька)
    /// </summary>
    public class TableSchema
    {
        public string Name   { get; set; }
        public Dictionary<string, string> Columns { get; set; }
    }
    
    
    
    public static partial class DbSchema
    {
        // ── TaskManager ───────────────────────────────────────────────────────

        /// <summary>
        /// InputSettings каждой задачи (переменные + _xml в base64).
        /// TaskManager._settingsTable
        /// </summary>
        public static readonly TableSchema Settings = new()
        {
            Name = "_settings",
            Columns = new()
            {
                { "id",        "TEXT PRIMARY KEY" },
                { "name",      "TEXT DEFAULT ''"  },
                { "_xml_b64",  "TEXT DEFAULT ''"  },
                { "_json_b64", "TEXT DEFAULT ''"  },
            }
        };


        /// <summary>
        /// Список задач ZennoPoster (JsonToDb из ZennoPoster.TasksList).
        /// TaskManager._tasksTable
        /// </summary>
        public static readonly TableSchema Tasks = new()
        {
            Name = "_tasks",
            Columns = new()
            {
                { "id",        "TEXT PRIMARY KEY" },
                { "name",      "TEXT DEFAULT ''"  },
                { "_json_b64", "TEXT DEFAULT ''"  },
            }
        };

        /// <summary>
        /// Очередь команд для выполнения (status: pending → done/error).
        /// TaskManager._commandsTable
        /// </summary>
        public static readonly TableSchema Commands = new()
        {
            Name = "_commands",
            Columns = new()
            {
                { "id",         "TEXT PRIMARY KEY"       },
                { "task_id",    "TEXT DEFAULT ''"        },
                { "action",     "TEXT DEFAULT ''"        },
                { "payload",    "TEXT DEFAULT ''"        },
                { "status",     "TEXT DEFAULT 'pending'" },
                { "result",     "TEXT DEFAULT ''"        },
                { "created_at", "TEXT DEFAULT ''"        },
            }
        };
        
        
        public static readonly TableSchema Process = new()
        {
            Name = "_processes",
            Columns = new()
            {
                { "id",           "TEXT PRIMARY KEY" },
                { "machine",      "TEXT DEFAULT ''"  },
                { "name",         "TEXT DEFAULT ''"  },
                { "ram",          "TEXT DEFAULT ''"  },
                { "uptime",       "TEXT DEFAULT ''"  },
                { "command_line", "TEXT DEFAULT ''"  },
                { "updated_at",   "TEXT DEFAULT ''"  },
            }
        };
        
        public static readonly TableSchema ZpNodes = new()
        {
            Name = "_zp_nodes",
            Columns = new()
            {
                { "machine",    "TEXT PRIMARY KEY" },
                { "host",       "TEXT DEFAULT ''"  },
                { "port",       "TEXT DEFAULT ''"  },
                { "updated_at", "TEXT DEFAULT ''"  },
            }
        };
        // ── DbExtencions ──────────────────────────────────────────────────────

        /// <summary>
        /// Таблица кошельков — используется в SqlGet при chainType-запросах.
        /// DbExtencions.cs:150 — хардкод "_wlt"
        /// </summary>
        public static string Wlt { get; set; } = "_wlt";
        
        public static string Instance  { get; set; } = "_instance";
        
      

        
    }
}