using System;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

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
				Expect.Call(() => taskObserver.Log("Running against: SQL Server"));
                Expect.Call(queryExecutor.ReadFirstColumnAsStringArray(settings, "select @@version")).Return(new string[] { "SQL Server"});
                connectionDropper.Drop(settings, taskObserver);
                queryExecutor.ExecuteNonQuery(settings, "ALTER DATABASE [db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE drop database [db]");
                Expect.Call(() => taskObserver.Log("Dropping database: db\n"));
			}

			using (mocks.Playback())
			{
				IDatabaseActionExecutor dropper = new DatabaseDropper(connectionDropper, queryExecutor);
				dropper.Execute(taskAttributes, taskObserver);
			}

			mocks.VerifyAll();
		}

        [Test]
        public void Drops_Azure_database_without_dropping_connections()
        {
            var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, null);

            var mocks = new MockRepository();
            var connectionDropper = mocks.StrictMock<IDatabaseConnectionDropper>();
            var taskObserver = mocks.StrictMock<ITaskObserver>();
            var queryExecutor = mocks.StrictMock<IQueryExecutor>();

            using (mocks.Record())
            {
                Expect.Call(() => taskObserver.Log("Running against: SQL Azure"));
                Expect.Call(queryExecutor.ReadFirstColumnAsStringArray(settings, "select @@version")).Return(new string[] { "SQL Azure" });
                queryExecutor.ExecuteNonQuery(settings, "drop database [db]");
                Expect.Call(() => taskObserver.Log("Dropping database: db\n"));
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
                Expect.Call(() => taskObserver.Log("Running against: SQL Server"));
                Expect.Call(queryExecutor.ReadFirstColumnAsStringArray(settings, "select @@version")).Return(new string[] { "SQL Server" });
                Expect.Call(() => taskObserver.Log("Dropping database: db\n"));
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