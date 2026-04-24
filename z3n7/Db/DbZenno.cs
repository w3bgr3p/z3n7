
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO
{
    
    public partial class Db
    {
        private readonly IZennoPosterProjectModel _project;
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