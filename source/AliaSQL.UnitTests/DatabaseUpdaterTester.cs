using System;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace AliaSQL.UnitTests
{
	[TestFixture]
	public class DatabaseUpdaterTester
	{
		[Test]
		public void Updates_database()
		{
			var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, "c:\\scripts");
            taskAttributes.RequestedDatabaseAction = RequestedDatabaseAction.Update;
			var mocks = new MockRepository();
            var scriptfolderexecutor = mocks.StrictMock<IScriptFolderExecutor>();
            var queryexecutor = mocks.StrictMock<IQueryExecutor>();
            queryexecutor.Stub(x => x.CheckDatabaseExists(taskAttributes.ConnectionSettings)).Return(true);

            var taskObserver = mocks.StrictMock<ITaskObserver>();
			using (mocks.Record())
			{
                taskObserver.Log(string.Format("Run scripts in Update folder."));
                scriptfolderexecutor.ExecuteScriptsInFolder(taskAttributes, "Update", taskObserver);

                taskObserver.Log(string.Format("Run scripts in Everytime folder."));
                scriptfolderexecutor.ExecuteChangedScriptsInFolder(taskAttributes, "Everytime", taskObserver);
			}

			using (mocks.Playback())
			{
				IDatabaseActionExecutor updater = new DatabaseUpdater(scriptfolderexecutor, queryexecutor);
                updater.Execute(taskAttributes, taskObserver);
			}

			mocks.VerifyAll();
		}


        //TODO: implement these tests
        //[Test]
        //public void Update_Database_Runs_New_Everytime_Script()
        //{
        //    throw new Exception();
        //}
        //[Test]
        //public void Update_Database_Skips_Already_Ran_Everytime_Script()
        //{
        //    throw new Exception();
        //}

        //[Test]
        //public void Update_Database_Runs_Altered_Everytime_Script()
        //{
        //    throw new Exception();
        //}

        //[Test]
        //public void Updates_Hash_For_Previously_Logged_Everytime_Script()
        //{
        //    throw new Exception();
        //}

	}
}