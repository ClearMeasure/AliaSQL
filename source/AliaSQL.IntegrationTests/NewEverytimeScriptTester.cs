using System;
using System.Data.SqlClient;
using System.IO;
using AliaSQL.Console;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Should;

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
            AssertUsdAppliedDatabaseScriptTable(settings, reader =>
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

        private void AssertUsdAppliedDatabaseScriptTable(ConnectionSettings settings, Action<SqlDataReader> assertAction)
        {
            string connectionString = new ConnectionStringGenerator().GetConnectionString(settings, true);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText =
                        "SELECT  [ScriptFile],[DateApplied],[Version],[hash] FROM [dbo].[usd_AppliedDatabaseScript]";
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        assertAction(reader);
                    }
                }
            }
        }

    }
}