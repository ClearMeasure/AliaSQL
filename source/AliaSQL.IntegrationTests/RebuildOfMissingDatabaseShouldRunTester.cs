using AliaSQL.Console;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;
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
                this.GetType().Name.Replace("Tester", string.Empty));

            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);
            
            var aliaConsole = new ConsoleAliaSQL();

            //act
            //database should not exist
            DatabaseDoesNotExist(settings).ShouldBeTrue();

            //assert
            aliaConsole.UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Rebuild).ShouldBeTrue();
        }

        private bool DatabaseDoesNotExist(ConnectionSettings settings)
        {
            var queryExecutor = new QueryExecutor();

            string sql = string.Format("select count(1) AS Databases from master..sysdatabases where name = '{0}';", settings.Database);

            return queryExecutor.ExecuteScalarInteger(settings, sql) == 0;
        }
    }
}
