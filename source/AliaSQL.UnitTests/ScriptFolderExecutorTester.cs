using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace AliaSQL.UnitTests
{
	[TestFixture]
	public class ScriptFolderExecutorTester
	{
		[Test]
		public void Executes_all_the_scripts_within_a_folder()
		{
			var settings = new ConnectionSettings("server", "db", true, null, null);
            var sqlFiles = new[] { "c:\\scripts\\Update\\001.sql", "c:\\scripts\\Update\\002_data_.sql", "c:\\scripts\\Update\\003.sql" };
            var taskAttributes = new TaskAttributes(settings, "c:\\scripts")
                                     {
                                         RequestedDatabaseAction = RequestedDatabaseAction.Update,
                                     };

			var mocks = new MockRepository();
            var initializer = mocks.StrictMock<ISchemaInitializer>();
            var fileLocator = mocks.StrictMock<ISqlFileLocator>();
            var executor = mocks.StrictMock<IChangeScriptExecutor>();
            var testdataexecutor = mocks.StrictMock<ITestDataScriptExecutor>();
            var versioner = mocks.StrictMock<IDatabaseVersioner>();
            var taskObserver = mocks.StrictMock<ITaskObserver>();

			using (mocks.Record())
			{
				initializer.EnsureSchemaCreated(settings);
				Expect.Call(fileLocator.GetSqlFilenames("c:\\scripts", "Update")).Return(sqlFiles);
				executor.Execute("c:\\scripts\\Update\\001.sql", settings, taskObserver);
				executor.Execute("c:\\scripts\\Update\\002_data_.sql", settings, taskObserver);
				executor.Execute("c:\\scripts\\Update\\003.sql", settings, taskObserver);
				versioner.VersionDatabase(settings, taskObserver);
			}

			using (mocks.Playback())
			{
                IScriptFolderExecutor folderExecutor = new ScriptFolderExecutor(initializer, fileLocator, executor, testdataexecutor, versioner);
				folderExecutor.ExecuteScriptsInFolder(taskAttributes, "Update", taskObserver);
			}

			mocks.VerifyAll();
		}

	}
}