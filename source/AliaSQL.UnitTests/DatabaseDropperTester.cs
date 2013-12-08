using System;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace AliaSQL.UnitTests
{
	[TestFixture]
	public class DatabaseDropperTester
	{
		[Test]
		public void Drops_database()
		{
			var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, null);

			var mocks = new MockRepository();
            var connectionDropper = mocks.StrictMock<IDatabaseConnectionDropper>();
            var taskObserver = mocks.StrictMock<ITaskObserver>();
            var queryExecutor = mocks.StrictMock<IQueryExecutor>();
            
			using (mocks.Record())
			{
				connectionDropper.Drop(settings, taskObserver);
                
                queryExecutor.ExecuteNonQuery(settings, "ALTER DATABASE [db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE drop database [db]");
			    
			}

			using (mocks.Playback())
			{
				IDatabaseActionExecutor dropper = new DatabaseDropper(connectionDropper, queryExecutor);
				dropper.Execute(taskAttributes, taskObserver);
			}

			mocks.VerifyAll();
		}

		[Test]
		public void Should_not_fail_if_datebase_does_not_exist()
		{
			var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, null);

			var mocks = new MockRepository();
			var connectionDropper = mocks.DynamicMock<IDatabaseConnectionDropper>();
            var taskObserver = mocks.StrictMock<ITaskObserver>();
            var queryExecutor = mocks.StrictMock<IQueryExecutor>();

			using (mocks.Record())
			{
                Expect.Call(() => queryExecutor.ExecuteNonQuery(settings, "ALTER DATABASE [db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE drop database [db]"))
					.Throw(new Exception("foo message"));
				Expect.Call(() => taskObserver.Log("Database 'db' could not be dropped."));
			}

			using (mocks.Playback())
			{
				IDatabaseActionExecutor dropper = new DatabaseDropper(connectionDropper, queryExecutor);
                dropper.Execute(taskAttributes, taskObserver);
			}

			mocks.VerifyAll();
		}
	}
}