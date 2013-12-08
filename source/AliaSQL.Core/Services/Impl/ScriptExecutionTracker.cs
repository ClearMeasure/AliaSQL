using System;
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services.Impl
{
	
	public class ScriptExecutionTracker : IScriptExecutionTracker
	{
		private string[] _appliedScripts;
		private readonly IQueryExecutor _executor;

		public ScriptExecutionTracker(IQueryExecutor executor)
		{
			_executor = executor;
		}

	    public ScriptExecutionTracker():this(new QueryExecutor())
	    {
	        
	    }

	    public void MarkScriptAsExecuted(ConnectionSettings settings, string scriptFilename, ITaskObserver task)
		{
			string insertTemplate = 
				"insert into usd_AppliedDatabaseScript (ScriptFile, DateApplied) values ('{0}', getdate())";

			string sql = string.Format(insertTemplate, scriptFilename);
			_executor.ExecuteNonQueryTransactional(settings, sql);
		}

        public void MarkTestDataScriptAsExecuted(ConnectionSettings settings, string scriptFilename, ITaskObserver task)
        {
            string insertTemplate =
                "insert into usd_AppliedDatabaseTestDataScript (ScriptFile, DateApplied) values ('{0}', getdate())";

            string sql = string.Format(insertTemplate, scriptFilename);
            _executor.ExecuteNonQueryTransactional(settings, sql);
        }

		public bool ScriptAlreadyExecuted(ConnectionSettings settings, string scriptFilename)
		{
			if (_appliedScripts == null)
			{
				_appliedScripts =
					_executor.ReadFirstColumnAsStringArray(settings, "select ScriptFile from usd_AppliedDatabaseScript");
			}

			bool alreadyExecuted = Array.IndexOf(_appliedScripts, scriptFilename) >= 0;

			return alreadyExecuted;
		}

        public bool TestDataScriptAlreadyExecuted(ConnectionSettings settings, string scriptFilename)
        {
            if (_appliedScripts == null)
            {
                _appliedScripts =
                    _executor.ReadFirstColumnAsStringArray(settings, "select ScriptFile from usd_AppliedDatabaseTestDataScript");
            }

            bool alreadyExecuted = Array.IndexOf(_appliedScripts, scriptFilename) >= 0;

            return alreadyExecuted;
        }
	}
}