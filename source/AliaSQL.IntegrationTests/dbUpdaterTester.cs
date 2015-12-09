using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using AliaSQL.Console;
using AliaSQL.Core;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;
using NUnit.Framework;
using Should;

namespace AliaSQL.IntegrationTests
{
    [TestFixture]
    public class DbUpdaterTester
    {

        [Test]
        public void UpdateDatabaseTester()
        {

            //arrange
            string scriptsDirectory = Path.Combine("Scripts", GetType().Name.Replace("Tester", ""));
            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);
            new DbUpdater().UpdateDatabase(new ConnectionStringGenerator().GetConnectionString(settings, true), RequestedDatabaseAction.Drop, scriptsDirectory);

            //act
            bool success = new DbUpdater().UpdateDatabase(new ConnectionStringGenerator().GetConnectionString(settings, true), RequestedDatabaseAction.Create, scriptsDirectory).Success;


            //assert
            int records = 0;
            AssertUsdAppliedDatabaseScriptTable(settings, reader =>
            {
                while (reader.Read())
                {
                    records++;
                    reader["ScriptFile"].ShouldEqual("0001-Update.sql");
                }
            });

            success.ShouldEqual(true);
            records.ShouldEqual(1);
        }

        [Test]
        public void DatabaseExistsTester()
        {

            //arrange
            string scriptsDirectory = Path.Combine("Scripts", GetType().Name.Replace("Tester", ""));
            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);
            new DbUpdater().UpdateDatabase(new ConnectionStringGenerator().GetConnectionString(settings, true), RequestedDatabaseAction.Drop, scriptsDirectory);

            //act
            bool dbexistsbefore = new DbUpdater().DatabaseExists(new ConnectionStringGenerator().GetConnectionString(settings, true));

            bool updated = new DbUpdater().UpdateDatabase(new ConnectionStringGenerator().GetConnectionString(settings, true), RequestedDatabaseAction.Update, scriptsDirectory).Success;

            bool dbexistsafter = new DbUpdater().DatabaseExists(new ConnectionStringGenerator().GetConnectionString(settings, true));
           
            //assert
            dbexistsbefore.ShouldEqual(false);
            dbexistsafter.ShouldEqual(true);
        }

        [Test]
        public void PendingChangesTester()
        {

            //arrange
            string scriptsDirectory = Path.Combine("Scripts", GetType().Name.Replace("Tester", ""));
            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);
            new DbUpdater().UpdateDatabase(new ConnectionStringGenerator().GetConnectionString(settings, true), RequestedDatabaseAction.Drop, scriptsDirectory);
            bool updated = new DbUpdater().UpdateDatabase(new ConnectionStringGenerator().GetConnectionString(settings, true), RequestedDatabaseAction.Update, scriptsDirectory).Success;

            //act
            List<string> pendingChanges = new DbUpdater().PendingChanges(new ConnectionStringGenerator().GetConnectionString(settings, true), scriptsDirectory.Replace("DbUpdater", "NewEverytimeScript"));

            //assert
            pendingChanges.Count.ShouldEqual(1);
        }

        [Test]
        public void DatabaseVersionTester()
        {

            //arrange
            string scriptsDirectory = Path.Combine("Scripts", GetType().Name.Replace("Tester", ""));
            var settings = new ConnectionSettings(".\\sqlexpress", "aliasqltest", true, null, null);
            new DbUpdater().UpdateDatabase(new ConnectionStringGenerator().GetConnectionString(settings, true), RequestedDatabaseAction.Drop, scriptsDirectory);
            int dbversionbeforeupdate = new DbUpdater().DatabaseVersion(new ConnectionStringGenerator().GetConnectionString(settings, true));

            bool updated = new DbUpdater().UpdateDatabase(new ConnectionStringGenerator().GetConnectionString(settings, true), RequestedDatabaseAction.Update, scriptsDirectory).Success;

            //act
            int dbversionafterupdate = new DbUpdater().DatabaseVersion(new ConnectionStringGenerator().GetConnectionString(settings, true));

            //assert
            dbversionbeforeupdate.ShouldEqual(0);
            dbversionafterupdate.ShouldEqual(1);
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