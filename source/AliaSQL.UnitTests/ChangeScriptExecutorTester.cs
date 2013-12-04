using System;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;
using AliaSQL.Core;

namespace AliaSQL.UnitTests.Core.DatabaseManager.Services
{
	[TestFixture]
	public class ChangeScriptExecutorTester
	{
		[Test]
		public void CorrectlyLogsWarningWhenScriptHasAlreadyBeenExecuted()
		{
			ConnectionSettings settings = getConnectionSettings();
			string scriptFile = @"c:\scripts\Update\01_Test.sql";

			MockRepository mocks = new MockRepository();
            IScriptExecutionTracker executionTracker = mocks.StrictMock<IScriptExecutionTracker>();
            ITaskObserver taskObserver = mocks.StrictMock<ITaskObserver>();

			Expect.Call(executionTracker.ScriptAlreadyExecuted(settings, "01_Test.sql")).Return(true);
			taskObserver.Log("Skipping (already executed): 01_Test.sql");

			mocks.ReplayAll();

			IChangeScriptExecutor executor = new ChangeScriptExecutor(executionTracker, null, null);
			executor.Execute(scriptFile, settings, taskObserver);

			mocks.VerifyAll();
		}

		[Test]
		public void CorrectlyExecutesScriptIfItHasntAlreadyBeenExecuted()
		{
			ConnectionSettings settings = getConnectionSettings();
			string scriptFile = @"c:\scripts\Update\01_Test.sql";
			string fileContents = "file contents...";

			MockRepository mocks = new MockRepository();
            IScriptExecutionTracker executionTracker = mocks.StrictMock<IScriptExecutionTracker>();
            IFileSystem fileSystem = mocks.StrictMock<IFileSystem>();
            IQueryExecutor queryExecutor = mocks.StrictMock<IQueryExecutor>();
            ITaskObserver taskObserver = mocks.StrictMock<ITaskObserver>();

			Expect.Call(executionTracker.ScriptAlreadyExecuted(settings, "01_Test.sql")).Return(false);
			taskObserver.Log("Executing: 01_Test.sql");
			Expect.Call(fileSystem.ReadTextFile(scriptFile)).Return(fileContents);
			queryExecutor.ExecuteNonQuery(settings, fileContents, true);
			executionTracker.MarkScriptAsExecuted(settings, "01_Test.sql", taskObserver);

			mocks.ReplayAll();

			IChangeScriptExecutor executor = new ChangeScriptExecutor(executionTracker, queryExecutor, fileSystem);
			executor.Execute(scriptFile, settings, taskObserver);

			mocks.VerifyAll();
		}

		private ConnectionSettings getConnectionSettings()
		{
			return new ConnectionSettings(String.Empty, String.Empty, false, String.Empty, String.Empty);
		}
	}
}