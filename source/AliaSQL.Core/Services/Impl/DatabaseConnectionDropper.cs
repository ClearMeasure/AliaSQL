using System;
using AliaSQL.Core.Model;
using AliaSQL.Core;
using AliaSQL.Infrastructure.DatabaseManager.DataAccess;

namespace AliaSQL.Core.Services.Impl
{
	
	public class DatabaseConnectionDropper : IDatabaseConnectionDropper
	{
		private IResourceFileLocator _fileLocator;
		private ITokenReplacer _replacer;
		private IQueryExecutor _executor;

		public DatabaseConnectionDropper(IResourceFileLocator fileLocator, ITokenReplacer replacer, IQueryExecutor executor)
		{
			_fileLocator = fileLocator;
			_replacer = replacer;
			_executor = executor;
		}

	    public DatabaseConnectionDropper():this(new ResourceFileLocator(), new TokenReplacer(), new QueryExecutor())
	    {
	        
	    }

	    public void Drop(ConnectionSettings settings, ITaskObserver taskObserver)
		{
			string message = string.Format("Dropping connections for database {0}\n", settings.Database);
			taskObserver.Log(message);

			string assembly = SqlDatabaseManager.SQL_FILE_ASSEMBLY;
			string sqlFile = string.Format(SqlDatabaseManager.SQL_FILE_TEMPLATE, "DropConnections");

			string sql = _fileLocator.ReadTextFile(assembly, sqlFile);

			_replacer.Text = sql;
			_replacer.Replace("DatabaseName", settings.Database);
			sql = _replacer.Text;

			_executor.ExecuteNonQuery(settings, sql, false);
		}
	}
}