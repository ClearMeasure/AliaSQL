﻿using System;
using System.IO;
using AliaSQL.Core.Model;
using AliaSQL.Core;

namespace AliaSQL.Core.Services.Impl
{
	
	public class TestDataScriptExecutor : ITestDataScriptExecutor
	{
		private readonly IScriptExecutionTracker _executionTracker;
		private readonly IQueryExecutor _executor;
		private readonly IFileSystem _fileSystem;

		public TestDataScriptExecutor(IScriptExecutionTracker executionTracker, IQueryExecutor executor, IFileSystem fileSystem)
		{
			_executionTracker = executionTracker;
			_executor = executor;
			_fileSystem = fileSystem;
		}

	    public TestDataScriptExecutor():this(new ScriptExecutionTracker(),new QueryExecutor(),new FileSystem())
	    {
	        
	    }

	    public void Execute(string fullFilename, ConnectionSettings settings, ITaskObserver taskObserver)
		{
			string scriptFilename = getFilename(fullFilename);

            if (_executionTracker.TestDataScriptAlreadyExecuted(settings, scriptFilename))
			{
				taskObserver.Log(string.Format("Skipping (already executed): {0}", scriptFilename));
			}
			else
			{
				string sql = _fileSystem.ReadTextFile(fullFilename);
                if (!_executor.ScriptSupportsTransactions(sql))
                {
                    taskObserver.Log(string.Format("Executing: {0}", scriptFilename));
                    _executor.ExecuteNonQuery(settings, sql, true);
                }
                else
                {
                    taskObserver.Log(string.Format("Executing: {0} in a transaction", scriptFilename));
                    _executor.ExecuteNonQueryTransactional(settings, sql);
                }
			}
		}

		private string getFilename(string fullFilename)
		{
			return Path.GetFileName(fullFilename);
		}
	}
}