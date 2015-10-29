using System.IO;
using AliaSQL.Console;
using AliaSQL.Core;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Should;

namespace AliaSQL.IntegrationTests.OldEverytimeScriptShouldNotRun
{
    [TestFixture]
    public class OldEverytimeScriptShouldNotRunTester
    {
        [Test]
        public void Update_Database_Skips_Old_Everytime_Script()
        {
            //arrange
            string scriptsDirectory = Path.Combine("Scripts", GetType().Name.Replace("Tester", ""));

            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);
            new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Drop);

            //act
            //run once 
            new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Update).ShouldBeTrue();
            new QueryExecutor().ExecuteScalarInteger(settings, "select 1 from dbo.sysobjects where name = 'TestTable' and type='U'").ShouldEqual(1);

            //delete TestTable to ensure script doesn't run again
            new QueryExecutor().ExecuteNonQuery(settings, "drop table TestTable", true);

            //run again
            bool success = new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Update);

            //assert
            new QueryExecutor().ExecuteScalarInteger(settings, "select 1 from dbo.sysobjects where name = 'TestTable' and type='U'").ShouldEqual(0);
        }

    }
}