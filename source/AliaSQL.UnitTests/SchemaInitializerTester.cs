using System;
using NUnit.Framework;
using Rhino.Mocks;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using AliaSQL.Core;

namespace AliaSQL.UnitTests.Core.DatabaseManager.Services
{
	[TestFixture]
	public class SchemaInitializerTester
	{
		[Test]
		public void CorrectlyInitializesSchema()
		{
			string assembly = AliaSQL.Core.Services.Impl.SqlDatabaseManager.SQL_FILE_ASSEMBLY;
			string sqlFile = string.Format(AliaSQL.Core.Services.Impl.SqlDatabaseManager.SQL_FILE_TEMPLATE, "CreateSchema");

			ConnectionSettings settings =
				new ConnectionSettings(String.Empty, String.Empty, false, String.Empty, String.Empty);
			string sqlScript = "SQL script...";

			MockRepository mocks = new MockRepository();
            IResourceFileLocator fileLocator = mocks.StrictMock<IResourceFileLocator>();
            IQueryExecutor queryExecutor = mocks.StrictMock<IQueryExecutor>();

			Expect.Call(fileLocator.ReadTextFile(assembly, sqlFile)).Return(sqlScript);
			queryExecutor.ExecuteNonQuery(settings, sqlScript, true);

			mocks.ReplayAll();

			ISchemaInitializer versioner = new SchemaInitializer(fileLocator, queryExecutor);
			versioner.EnsureSchemaCreated(settings);

			mocks.VerifyAll();
		}
	}
}