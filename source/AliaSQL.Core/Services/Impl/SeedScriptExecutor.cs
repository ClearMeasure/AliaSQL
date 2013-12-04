using System;
using System.IO;
using AliaSQL.Core.Model;
using AliaSQL.Core;
using AliaSQL.Infrastructure.DatabaseManager.DataAccess;

namespace AliaSQL.Core.Services.Impl
{
	
	public class SeedScriptExecutor : ISeedScriptExecutor
	{
		private readonly IScriptExecutionTracker _executionTracker;
		private readonly IQueryExecutor _executor;
		private readonly IFileSystem _fileSystem;

		public SeedScriptExecutor(IScriptExecutionTracker executionTracker, IQueryExecutor executor, IFileSystem fileSystem)
		{
			_executionTracker = executionTracker;
			_executor = executor;
			_fileSystem = fileSystem;
		}

	    public SeedScriptExecutor():this(new ScriptExecutionTracker(),new QueryExecutor(),new FileSystem())
	    {
	        
	    }

	    public void Execute(string fullFilename, ConnectionSettings settings, ITaskObserver taskObserver)
		{
			string scriptFilename = getFilename(fullFilename);

			if (_executionTracker.SeedScriptAlreadyExecuted(settings, scriptFilename))
			{
				taskObserver.Log(string.Format("Skipping (already executed): {0}", scriptFilename));
			}
			else
			{
				taskObserver.Log(string.Format("Executing: {0}", scriptFilename));
				string sql = _fileSystem.ReadTextFile(fullFilename);
				_executor.ExecuteNonQuery(settings, sql, true);
				_executionTracker.MarkSeedScriptAsExecuted(settings, scriptFilename, taskObserver);
			}
		}

		private string getFilename(string fullFilename)
		{
			return Path.GetFileName(fullFilename);
		}
	}
}