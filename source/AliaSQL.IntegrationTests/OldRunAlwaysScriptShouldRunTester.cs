using System;
using System.Data.SqlClient;
using System.IO;
using AliaSQL.Console;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Should;

namespace AliaSQL.IntegrationTests.OldRunAlwaysScriptShouldRun
{
    [TestFixture]
    public class OldRunAlwaysScriptShouldRunTester
    {
        [Test]
        public void Update_Database_ShouldRun_Old_RunAlways_Script()
        {
            //arrange
            string scriptsDirectory = Path.Combine("Scripts", GetType().Name.Replace("Tester", ""));

            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);
            new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Drop);

            //act
            //run once 
            new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Update).ShouldBeTrue();
            new QueryExecutor().ExecuteScalarInteger(settings, "select 1 from dbo.sysobjects where name = 'TestTable' and type='U'").ShouldEqual(1);
            var dateApplied = DateTime.MinValue;
            QueryUsdAppliedDatabaseScriptTable(settings, reader =>
            {
                while (reader.Read())
                {
                    reader["ScriptFile"].ShouldEqual("TestScript.sql");
                    dateApplied = (DateTime)reader["DateApplied"];
                }
            });
            dateApplied.ShouldBeGreaterThan(DateTime.MinValue);

            //delete TestTable to ensure script doesn't run again
            new QueryExecutor().ExecuteNonQuery(settings, "drop table TestTable", true);

            //run again
            bool success = new ConsoleAliaSQL().UpdateDatabase(settings, scriptsDirectory, RequestedDatabaseAction.Update);

            //assert
            new QueryExecutor().ExecuteScalarInteger(settings, "select 1 from dbo.sysobjects where name = 'TestTable' and type='U'").ShouldEqual(1);

            DateTime dateAppliedUpdated = DateTime.MinValue; ;
            int records = 0;
            QueryUsdAppliedDatabaseScriptTable(settings, reader =>
            {
                while (reader.Read())
                {
                    records++;
                    reader["ScriptFile"].ShouldEqual("TestScript.sql");
                    dateAppliedUpdated = (DateTime) reader["DateApplied"];
                }
               
            });

            success.ShouldBeTrue();
            records.ShouldEqual(1);
            dateAppliedUpdated.ShouldBeGreaterThan(dateApplied);
        }


        private void QueryUsdAppliedDatabaseScriptTable(ConnectionSettings settings, Action<SqlDataReader> action)
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
                        action(reader);
                    }
                }
            }
        }

    }
}