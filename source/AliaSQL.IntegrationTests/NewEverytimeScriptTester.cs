using AliaSQL.Console;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;
using AliaSQL.IntegrationTests.Utils;
using NUnit.Framework;
using Should;
using System;
using System.Data.SqlClient;
using System.IO;

namespace AliaSQL.IntegrationTests.NewEverytimeScript
{
    [TestFixture]
    public class NewEverytimeScriptTester
    {
        [Test]
        public void Update_Database_Runs_New_Everytime_Script()
        {
            //arrange
            string scriptsDirectory =  Path.Combine("Scripts",GetType().Name.Replace("Tester", ""));

            string scriptFileMd5 = ChangeScriptExecutor.GetFileMD5Hash(Path.Combine(scriptsDirectory,"Everytime", "TestScript.sql"));
            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);
            new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Drop);

            //act
            bool success = new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Update);

            //assert
            int records = 0;
            DatabaseIntegrationHelpers.AssertUsdAppliedDatabaseScriptTable(settings, reader =>
            {
                while (reader.Read())
                {
                    records++;
                    reader["ScriptFile"].ShouldEqual("TestScript.sql");
                    reader["hash"].ShouldEqual(scriptFileMd5);
                }
            });

            success.ShouldEqual(true);
            records.ShouldEqual(1);
        }
    }
}