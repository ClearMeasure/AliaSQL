using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using ConnectionSettings = AliaSQL.Core.Model.ConnectionSettings;

namespace AliaSQL.Infrastructure.DatabaseManager.DataAccess
{

    public class QueryExecutor : IQueryExecutor
    {
        private readonly IConnectionStringGenerator _connectionStringGenerator;

        public QueryExecutor(IConnectionStringGenerator connectionStringGenerator)
        {
            _connectionStringGenerator = connectionStringGenerator;
        }

        public QueryExecutor()
            : this(new ConnectionStringGenerator())
        {

        }

        public void ExecuteNonQuery(ConnectionSettings settings, string sql, bool runAgainstSpecificDatabase)
        {
            string connectionString = _connectionStringGenerator.GetConnectionString(settings, runAgainstSpecificDatabase);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    var scripts = SplitSqlStatements(sql);
                    foreach (var splitScript in scripts)
                    {
                        command.CommandText = splitScript;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public int ExecuteScalarInteger(ConnectionSettings settings, string sql)
        {
            string connectionString = _connectionStringGenerator.GetConnectionString(settings, true);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public string[] ReadFirstColumnAsStringArray(ConnectionSettings settings, string sql)
        {
            var list = new List<string>();
            string connectionString = _connectionStringGenerator.GetConnectionString(settings, true);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string item = reader[0].ToString();
                            list.Add(item);
                        }
                    }
                }


            }
            return list.ToArray();
        }

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Split by "GO" statements
            var statements = Regex.Split(
                    sqlScript,
                    @"^\s*GO\s* ($ | \-\- .*$)",
                    RegexOptions.Multiline |
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.IgnoreCase);

            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'));
        }

        public bool CheckDatabaseExists(ConnectionSettings settings)
        {
            bool result;
            var tmpConn = new SqlConnection(_connectionStringGenerator.GetConnectionString(settings, false));
            try
            {
                string sqlCreateDbQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", settings.Database);
                using (tmpConn)
                {
                    using (var sqlCmd = new SqlCommand(sqlCreateDbQuery, tmpConn))
                    {
                        tmpConn.Open();
                        var databaseId = (int)sqlCmd.ExecuteScalar();
                        tmpConn.Close();

                        result = (databaseId > 0);
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

    }
}