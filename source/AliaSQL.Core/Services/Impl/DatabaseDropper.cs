using System;
using AliaSQL.Core.Model;
using AliaSQL.Infrastructure.DatabaseManager.DataAccess;

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

	    public DatabaseDropper():this(new DatabaseConnectionDropper(),new QueryExecutor())
	    {
	        
	    }

	    public void Execute(TaskAttributes taskAttributes, ITaskObserver taskObserver)
		{
	               _connectionDropper.Drop(taskAttributes.ConnectionSettings, taskObserver);
			var sql = string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE drop database [{0}]", taskAttributes.ConnectionSettings.Database);

			try
			{
                _queryExecutor.ExecuteNonQuery(taskAttributes.ConnectionSettings, sql);
			}
			catch(Exception)
			{
				taskObserver.Log(string.Format("Database '{0}' could not be dropped.", taskAttributes.ConnectionSettings.Database));
			}
		}
	}
}