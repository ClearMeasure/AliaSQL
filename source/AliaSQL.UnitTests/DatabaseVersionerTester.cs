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
	public class DatabaseVersionerTester
	{
		[Test]
		public void Versions_database()
		{
			string assembly = SqlDatabaseManager.SQL_FILE_ASSEMBLY;
			string sqlFile = string.Format(SqlDatabaseManager.SQL_FILE_TEMPLATE, "VersionDatabase");

			ConnectionSettings settings = new ConnectionSettings(String.Empty, String.Empty, false, String.Empty, String.Empty);
			string sqlScript = "SQL script...";

			MockRepository mocks = new MockRepository();
            IResourceFileLocator fileLocator = mocks.StrictMock<IResourceFileLocator>();
            IQueryExecutor queryExecutor = mocks.StrictMock<IQueryExecutor>();
            ITaskObserver taskObserver = mocks.StrictMock<ITaskObserver>();

			using (mocks.Record())
			{
				Expect.Call(fileLocator.ReadTextFile(assembly, sqlFile)).Return(sqlScript);
				Expect.Call(queryExecutor.ExecuteScalarInteger(settings, sqlScript)).Return(7);
				taskObserver.SetVariable("usdDatabaseVersion", "7");
			}

			using (mocks.Playback())
			{
				IDatabaseVersioner versioner = new DatabaseVersioner(fileLocator, queryExecutor);
				versioner.VersionDatabase(settings, taskObserver);
			}

			mocks.VerifyAll();
		}
	}
}