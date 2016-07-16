using System;
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services.Impl
{
	public class DatabaseUpdaterCustom : IDatabaseActionExecutor
	{
		private readonly IScriptFolderExecutor _folderExecutor;
        private readonly IQueryExecutor _queryExecutor;

		public DatabaseUpdaterCustom(IScriptFolderExecutor folderExecutor, IQueryExecutor queryExecutor)
		{
			_folderExecutor = folderExecutor;
            _queryExecutor = queryExecutor;
		}

	    public DatabaseUpdaterCustom():this(new ScriptFolderExecutor(), new QueryExecutor())
	    {
	        
	    }

	    public void Execute(TaskAttributes taskAttributes, ITaskObserver taskObserver)
		{
	        if (!_queryExecutor.CheckDatabaseExists(taskAttributes.ConnectionSettings))
	        {
                taskObserver.Log(string.Format("Database does not exist. Attempting to create database before updating."));
                string sql = string.Format("create database [{0}]", taskAttributes.ConnectionSettings.Database);
                _queryExecutor.ExecuteNonQuery(taskAttributes.ConnectionSettings, sql);
                 taskObserver.Log(string.Format("Run scripts in Create folder."));
                _folderExecutor.ExecuteScriptsInFolder(taskAttributes, "Create", taskObserver);
	        }
            taskObserver.Log(string.Format("Run scripts in Update folder."));
            _folderExecutor.ExecuteScriptsInFolder(taskAttributes, "Update", taskObserver);

            taskObserver.Log(string.Format("Run scripts in Everytime folder."));
            _folderExecutor.ExecuteChangedScriptsInFolder(taskAttributes, "Everytime", taskObserver);

            taskObserver.Log(string.Format("Run scripts in RunAlways folder."));
            _folderExecutor.ExecuteRunAlwaysScriptsInFolder(taskAttributes, "RunAlways", taskObserver);
        }

	}
}