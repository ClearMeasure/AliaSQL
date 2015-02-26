using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using System;
using System.Data.SqlClient;

namespace AliaSQL.IntegrationTests.Utils
{
    internal class DatabaseIntegrationHelpers
    {
        public static void AssertUsdAppliedDatabaseScriptTable(ConnectionSettings settings, Action<SqlDataReader> assertAction)
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
