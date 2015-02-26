using AliaSQL.Console;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;
using AliaSQL.IntegrationTests.Utils;
using NUnit.Framework;
using Should;
using System.IO;

namespace AliaSQL.IntegrationTests
{
    [TestFixture]
    public class RebuildOfMissingDatabaseShouldRunTester
    {
        [Test]
        public void Rebuild_Missing_Database_Generates_Database()
        {
            //arrange
            string scriptsDirectory = Path.Combine("Scripts",
                GetType().Name.Replace("Tester", string.Empty));

            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);

            var aliaConsole = new ConsoleAliaSQL();

            //act
            //database should not exist
            DropDatabaseIfExists(settings, scriptsDirectory);

            //assert
            aliaConsole.UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Rebuild).ShouldBeTrue();

            int records = 0;
            DatabaseIntegrationHelpers.AssertUsdAppliedDatabaseScriptTable(settings, reader =>
            {
                while (reader.Read())
                {
                    records++;
                    reader["ScriptFile"].ShouldEqual("TestScript.sql");
                }
            });

            records.ShouldEqual(1);

            DropDatabaseIfExists(settings, scriptsDirectory);
        }

        private void DropDatabaseIfExists(ConnectionSettings settings, string scriptsDirectory)
        {
            if (DatabaseExists(settings))
            {
                DropDatabase(settings, scriptsDirectory);
            }
        }

        private bool DatabaseExists(ConnectionSettings settings)
        {
            var queryExecutor = new QueryExecutor();

            return queryExecutor.CheckDatabaseExists(settings);
        }

        private void DropDatabase(ConnectionSettings settings, string scriptsDirectory)
        {
            new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Drop);
        }
    }
}
