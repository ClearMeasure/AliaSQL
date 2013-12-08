using System;
using AliaSQL.Core.Model;
using AliaSQL.Core;

namespace AliaSQL.Core.Services.Impl
{
	
	public class SchemaInitializer : ISchemaInitializer
	{
		private readonly IQueryExecutor _executor;
		private readonly IResourceFileLocator _locator;

		public SchemaInitializer(IResourceFileLocator locator, IQueryExecutor executor)
		{
			_locator = locator;
			_executor = executor;
		}

	    public SchemaInitializer():this(new ResourceFileLocator(), new QueryExecutor())
	    {
	        
	    }

	    public void EnsureSchemaCreated(ConnectionSettings settings)
		{
			string assembly = SqlDatabaseManager.SQL_FILE_ASSEMBLY;
			string sqlFile = string.Format(SqlDatabaseManager.SQL_FILE_TEMPLATE, "CreateSchema");

			string sql = _locator.ReadTextFile(assembly, sqlFile);

			_executor.ExecuteNonQueryTransactional(settings, sql);
		}
        public void EnsureTestDataSchemaCreated(ConnectionSettings settings)
        {
            string assembly = SqlDatabaseManager.SQL_FILE_ASSEMBLY;
            string sqlFile = string.Format(SqlDatabaseManager.SQL_FILE_TEMPLATE, "CreateTestDataSchema");

            string sql = _locator.ReadTextFile(assembly, sqlFile);

            _executor.ExecuteNonQueryTransactional(settings, sql);
        }
	}
}