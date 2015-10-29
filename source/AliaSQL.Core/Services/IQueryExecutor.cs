using System.Collections.Generic;
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	public interface IQueryExecutor
	{
		void ExecuteNonQuery(ConnectionSettings settings, string sql, bool includeDatabaseName = false);
        void ExecuteNonQueryTransactional(ConnectionSettings settings, string sql);
		int ExecuteScalarInteger(ConnectionSettings settings, string sql);
		string[] ReadFirstColumnAsStringArray(ConnectionSettings settings, string sql);
	    bool CheckDatabaseExists(ConnectionSettings settings);
	    List<string> GetExecutedScripts(ConnectionSettings settings);
        List<string> GetExecutedTestDataScripts(ConnectionSettings settings);
	}
}