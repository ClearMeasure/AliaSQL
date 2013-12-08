using System;
using AliaSQL.Core.Model;
using AliaSQL.Core;

namespace AliaSQL.Core.Services.Impl
{
	
	public class DatabaseVersioner : IDatabaseVersioner
	{
		private IQueryExecutor _executor;
		private IResourceFileLocator _fileLocator;
		private string _databaseVersionPropertyName = "usdDatabaseVersion";

		public DatabaseVersioner(IResourceFileLocator fileLocator, IQueryExecutor executor)
		{
			_fileLocator = fileLocator;
			_executor = executor;
		}

	    public DatabaseVersioner():this(new ResourceFileLocator(), new QueryExecutor())
	    {
	        
	    }

	    public void VersionDatabase(ConnectionSettings settings, ITaskObserver taskObserver)
		{
			string assembly = SqlDatabaseManager.SQL_FILE_ASSEMBLY;
			string sqlFile = string.Format(SqlDatabaseManager.SQL_FILE_TEMPLATE, "VersionDatabase");

			string sql = _fileLocator.ReadTextFile(assembly, sqlFile);
			string version = _executor.ExecuteScalarInteger(settings, sql).ToString();
			taskObserver.SetVariable(_databaseVersionPropertyName, version);
		}
	}
}