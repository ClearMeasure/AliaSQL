using System.IO;
using AliaSQL.Console;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Should;

namespace AliaSQL.IntegrationTests.ChangedEverytimeScriptShouldRun
{
    [TestFixture]
    public class ChangedEverytimeScriptShouldRunTester
    {
        [Test]
        public void Update_Database_Runs_Changed_Everytime_Script()
        {

            //arrange
            string scriptsDirectory = Path.Combine("Scripts", GetType().Name.Replace("Tester", ""));

            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);
            new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Drop);

            //act
            //run once 
            new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Update).ShouldBeTrue();
            new QueryExecutor().ExecuteScalarInteger(settings, "select 1 from dbo.sysobjects where name = 'TestTable' and type='U'").ShouldEqual(1);

            //change contents of script
            File.WriteAllText(Path.Combine(scriptsDirectory, "Everytime", "TestScript.sql"), "CREATE TABLE [dbo].[TestTable2]([Id] [int] IDENTITY(1,1) NOT NULL, [FullName] [nvarchar](50) NULL)");

            new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Update).ShouldBeTrue();

            //assert
            new QueryExecutor().ExecuteScalarInteger(settings, "select 1 from dbo.sysobjects where name = 'TestTable2' and type='U'").ShouldEqual(1);

            //change contents of script back in case you run again without rebuilding
            File.WriteAllText(Path.Combine(scriptsDirectory, "Everytime", "TestScript.sql"), "CREATE TABLE [dbo].[TestTable]([Id] [int] IDENTITY(1,1) NOT NULL, [FullName] [nvarchar](50) NULL)");

        }

    }
}