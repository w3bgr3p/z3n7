
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    
    public partial class Db
    {
        private readonly IZennoPosterProjectModel _project;
        /// <summary>
        /// Creates a database helper from project settings. Each argument left <c>null</c> is read from:
        /// <c>dbMode</c> — variable <c>DBmode</c>; <c>sqLitePath</c> — variable <c>DBsqltPath</c>; PostgreSQL
        /// host, port, database, user and password — global variables <c>sqlPgHost</c>, <c>sqlPgPort</c>,
        /// <c>sqlPgName</c>, <c>sqlPgUser</c>, <c>sqlPgPass</c>; <c>defaultTable</c> —
        /// <c>project.ProjectTable()</c> (<c>__</c> + project name).
        /// </summary>
        /// <param name="log">Log queries and results at <c>Info</c> level.</param>
        public Db(
            IZennoPosterProjectModel project,
            string dbMode = null,
            string sqLitePath = null,
            string pgHost = null,
            string pgPort = null,
            string pgDbName = null,
            string pgUser = null,
            string pgPass = null,
            string defaultTable = null, bool log = false)
        {
            _project = project;
            _dbMode = dbMode ?? project.Var("DBmode");
            _sqLitePath = sqLitePath ?? project.Var("DBsqltPath");
            _pgHost = pgHost ?? project.GVar("sqlPgHost");
            _pgPort = pgPort ?? project.GVar("sqlPgPort");
            _pgDbName = pgDbName ?? project.GVar("sqlPgName");
            _pgUser = pgUser ?? project.GVar("sqlPgUser");
            _pgPass = pgPass ?? project.GVar("sqlPgPass");
            _defaultTable = defaultTable ?? project.ProjectTable();
            
            _log =  new Logger(_project,null, logLevel: (log) ? LogLevel.Info : LogLevel.Off);
        }
        
        
    }
    
    
    
}