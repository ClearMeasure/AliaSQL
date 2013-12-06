using System;
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services.Impl
{
	public class DatabaseTestData : IDatabaseActionExecutor
	{
		private readonly IScriptFolderExecutor _folderExecutor;

		public DatabaseTestData(IScriptFolderExecutor folderExecutor)
		{
			_folderExecutor = folderExecutor;
		}

        public DatabaseTestData()
            : this(new ScriptFolderExecutor())
	    {
	        
	    }

	    public void Execute(TaskAttributes taskAttributes, ITaskObserver taskObserver)
		{
            _folderExecutor.ExecuteTestDataScriptsInFolder(taskAttributes, "TestData", taskObserver);
		}
	}
}