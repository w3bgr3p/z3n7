using System.Collections.Generic;

namespace z3n7
{
    /// <summary>Name and column definitions of a table.</summary>
    public class TableSchema
    {
        /// <summary>Table name.</summary>
        public string Name   { get; set; }
        /// <summary>Column name → SQL type.</summary>
        public Dictionary<string, string> Columns { get; set; }
    }
    
    
    
    /// <summary>Names and layouts of the library's own tables.</summary>
    public static partial class DbSchema
    {
        /// <summary>
        /// Table <c>_processes</c>: one row per process with machine, name, RAM, uptime and command line.
        /// </summary>
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

        /// <summary>Wallet table, default <c>_wlt</c>.</summary>
        public static string Wlt { get; set; } = "_wlt";
        
        /// <summary>Instance table, default <c>_instance</c>.</summary>
        public static string Instance  { get; set; } = "_instance";
        
      

        
    }
}