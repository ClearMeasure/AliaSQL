using System;
using System.Linq;
using System.Threading.Tasks;
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services.Impl
{
    public class DatabaseDropper : IDatabaseActionExecutor
    {
        private readonly IDatabaseConnectionDropper _connectionDropper;
        private readonly IQueryExecutor _queryExecutor;

        public DatabaseDropper(IDatabaseConnectionDropper connectionDropper, IQueryExecutor queryExecutor)
        {
            _connectionDropper = connectionDropper;
            _queryExecutor = queryExecutor;
        }

        public DatabaseDropper()
            : this(new DatabaseConnectionDropper(), new QueryExecutor())
        {

        }

        public void Execute(TaskAttributes taskAttributes, ITaskObserver taskObserver)
        {
            var version = _queryExecutor.ReadFirstColumnAsStringArray(taskAttributes.ConnectionSettings, "select @@version")[0];
             taskObserver.Log("Running against: " + version);

            //can't kill connections or enter single user mode in Azure
            var sql = string.Format("drop database [{0}]", taskAttributes.ConnectionSettings.Database);
            if (!version.Contains("SQL Azure"))
            {
                _connectionDropper.Drop(taskAttributes.ConnectionSettings, taskObserver);
                sql = string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE drop database [{0}]", taskAttributes.ConnectionSettings.Database);
            }

            string message = string.Format("Dropping database: {0}\n", taskAttributes.ConnectionSettings.Database);
            taskObserver.Log(message);
            try
            {
                _queryExecutor.ExecuteNonQuery(taskAttributes.ConnectionSettings, sql);
            }
            catch (Exception)
            {
                taskObserver.Log(string.Format("Database '{0}' could not be dropped.", taskAttributes.ConnectionSettings.Database));
            }
        }
    }
}