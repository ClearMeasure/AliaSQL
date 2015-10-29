using System;
using AliaSQL.Core;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace AliaSQL.UnitTests
{
	[TestFixture]
	public class DatabaseCreatorTester
	{
		[Test]
		public void Creates_database()
		{
			var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, "c:\\scripts");
            taskAttributes.RequestedDatabaseAction= RequestedDatabaseAction.Create;
			var mocks = new MockRepository();
            var queryExecutor = mocks.StrictMock<IQueryExecutor>();
            var executor = mocks.StrictMock<IScriptFolderExecutor>();
            var taskObserver = mocks.StrictMock<ITaskObserver>();
			
			using (mocks.Record())
			{
				queryExecutor.ExecuteNonQuery(settings, "create database [db]");
                taskObserver.Log(string.Format("Run scripts in Create folder."));
				executor.ExecuteScriptsInFolder(taskAttributes, "Create", taskObserver);
			}

			using (mocks.Playback())
			{
				IDatabaseActionExecutor creator = new DatabaseCreator(queryExecutor, executor);
				creator.Execute(taskAttributes, taskObserver);
			}

			mocks.VerifyAll();
		}
	}
}