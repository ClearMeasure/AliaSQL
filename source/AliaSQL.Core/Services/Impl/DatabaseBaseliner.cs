using System;
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services.Impl
{
	public class DatabaseBaseliner : IDatabaseActionExecutor
	{
		private readonly IScriptFolderExecutor _folderExecutor;

		public DatabaseBaseliner(IScriptFolderExecutor folderExecutor)
		{
			_folderExecutor = folderExecutor;
		}

        public DatabaseBaseliner()
            : this(new ScriptFolderExecutor())
	    {
	        
	    }

	    public void Execute(TaskAttributes taskAttributes, ITaskObserver taskObserver)
	    {
	        taskAttributes.LogOnly = true;
            _folderExecutor.ExecuteScriptsInFolder(taskAttributes, "Create", taskObserver);
            _folderExecutor.ExecuteScriptsInFolder(taskAttributes, "Update", taskObserver);
		}
	}
}