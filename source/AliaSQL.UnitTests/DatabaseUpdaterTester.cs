using NUnit.Framework;
using Rhino.Mocks;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;

namespace AliaSQL.UnitTests.Core.DatabaseManager.Services
{
	[TestFixture]
	public class DatabaseUpdaterTester
	{
		[Test]
		public void Updates_database()
		{
			var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, "c:\\scripts");

			var mocks = new MockRepository();
            var scriptfolderexecutor = mocks.StrictMock<IScriptFolderExecutor>();
            var queryexecutor = mocks.StrictMock<IQueryExecutor>();
            queryexecutor.Stub(x => x.CheckDatabaseExists(taskAttributes.ConnectionSettings)).Return(true);

            var taskObserver = mocks.StrictMock<ITaskObserver>();
			using (mocks.Record())
			{
                scriptfolderexecutor.ExecuteScriptsInFolder(taskAttributes, "Update", taskObserver);
			}

			using (mocks.Playback())
			{
				IDatabaseActionExecutor updater = new DatabaseUpdater(scriptfolderexecutor, queryexecutor);
                updater.Execute(taskAttributes, taskObserver);
			}

			mocks.VerifyAll();
		}
	}
}