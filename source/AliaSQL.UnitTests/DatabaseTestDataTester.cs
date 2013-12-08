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
            var queryExecutor = mocks.StrictMock<IQueryExecutor>();
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
    }
}