using System;
using System.IO;
using System.Linq;
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

	    public void MarkScriptAsExecuted(ConnectionSettings settings, string scriptFilename, ITaskObserver task, string hash = "")
		{
            //for everytime scripts just delete the row. We could update it but either way has the same result
	        if (!string.IsNullOrEmpty(hash))
	        {
	            string deleteTemplate = "delete from usd_AppliedDatabaseScript where ScriptFile = '{0}'";
	            string deletesql = string.Format(deleteTemplate, scriptFilename);
	            _executor.ExecuteNonQueryTransactional(settings, deletesql);
	        }

	        string insertTemplate = "insert into usd_AppliedDatabaseScript (ScriptFile, DateApplied, hash) values ('{0}', getdate(), '{1}')";
			string sql = string.Format(insertTemplate, scriptFilename, hash);
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

        public bool EverytimeScriptShouldBeExecuted(ConnectionSettings settings, string scriptFilename, string md5)
        {
            bool shouldBeExecuted = false;
            if (ScriptAlreadyExecuted(settings, scriptFilename))
            {
                var filehash = _executor.ReadFirstColumnAsStringArray(settings,"select hash from usd_AppliedDatabaseScript where ScriptFile = '" + scriptFilename + "'");
                shouldBeExecuted = filehash.Any() && (filehash[0] != md5);
            }
            else
            {
                shouldBeExecuted = true;
            }
            return shouldBeExecuted;
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