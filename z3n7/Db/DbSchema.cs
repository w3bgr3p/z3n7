using System.Collections.Generic;

namespace z3n7
{
    /// <summary>
    /// Централизованное хранилище имён таблиц с дефолтными значениями.
    ///
    /// Источники:
    ///   DbExtencions.cs — _wlt (хардкод на строке 150, в SqlGet цепочки кошелька)
    /// </summary>
    public class TableSchema
    {
        public string Name   { get; set; }
        public Dictionary<string, string> Columns { get; set; }
    }
    
    
    
    public static partial class DbSchema
    {
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
        // ── DbExtencions ──────────────────────────────────────────────────────

        /// <summary>
        /// Таблица кошельков — используется в SqlGet при chainType-запросах.
        /// DbExtencions.cs:150 — хардкод "_wlt"
        /// </summary>
        public static string Wlt { get; set; } = "_wlt";
        
        public static string Instance  { get; set; } = "_instance";
        
      

        
    }
}