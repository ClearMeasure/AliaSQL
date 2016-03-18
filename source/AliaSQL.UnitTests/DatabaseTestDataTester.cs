using System;
using System.Runtime.InteropServices;
using AliaSQL.Core;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Impl.Invocation.Specifications;

namespace AliaSQL.UnitTests
{
    [TestFixture]
    public class DatabaseTestDataTester
    {
        [Test]
        public void LogsTestData()
        {
            var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, "c:\\scripts");
            taskAttributes.RequestedDatabaseAction = RequestedDatabaseAction.TestData;
            var mocks = new MockRepository();
            var executor = mocks.StrictMock<IScriptFolderExecutor>();
            var taskObserver = mocks.StrictMock<ITaskObserver>();

            using (mocks.Record())
            {
                executor.ExecuteTestDataScriptsInFolder(taskAttributes, "TestData", taskObserver);
            }

            using (mocks.Playback())
            {
                IDatabaseActionExecutor testdata = new DatabaseTestData(executor);
                testdata.Execute(taskAttributes, taskObserver);

            }

            mocks.VerifyAll();
        }

        [Test]
        public void CorrectlyExecutesScriptIfItHasntAlreadyBeenExecuted()
        {
            var settings = new ConnectionSettings("server", "db", true, null, null);
            string scriptFile = @"c:\scripts\TestData\01_Test.sql";
            string fileContents = "file contents...";

            MockRepository mocks = new MockRepository();
            IScriptExecutionTracker executionTracker = mocks.StrictMock<IScriptExecutionTracker>();
            IFileSystem fileSystem = mocks.StrictMock<IFileSystem>();
            IQueryExecutor queryExecutor = mocks.StrictMock<IQueryExecutor>();
            ITaskObserver taskObserver = mocks.StrictMock<ITaskObserver>();

            Expect.Call(executionTracker.TestDataScriptAlreadyExecuted(settings, "01_Test.sql")).Return(false);
            taskObserver.Log("Executing: 01_Test.sql in a transaction");
            Expect.Call(fileSystem.ReadTextFile(scriptFile)).Return(fileContents);
            Expect.Call(queryExecutor.ScriptSupportsTransactions(fileContents)).Return(true);
            queryExecutor.ExecuteNonQueryTransactional(settings, fileContents);
            executionTracker.MarkTestDataScriptAsExecuted(settings, "01_Test.sql", taskObserver);

            mocks.ReplayAll();

            ITestDataScriptExecutor executor = new TestDataScriptExecutor(executionTracker, queryExecutor, fileSystem);
            executor.Execute(scriptFile, settings, taskObserver);

            mocks.VerifyAll();
        }
    }
}