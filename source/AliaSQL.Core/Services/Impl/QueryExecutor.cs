using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
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

        /// <summary>
        /// Runs queries that are not specific to a database such as Drop, Create, single user mode
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="sql"></param>
        /// <param name="includeDatabaseName"></param>
        public void ExecuteNonQuery(ConnectionSettings settings, string sql, bool includeDatabaseName = false)
        {
            string connectionString = _connectionStringGenerator.GetConnectionString(settings, includeDatabaseName);

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

        /// <summary>
        /// Runs larger queries that may be multiline separated with GO
        /// Runs entire sql block in a single transaction that will rollback if any part of the query errors
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="sql"></param>
        public void ExecuteNonQueryTransactional(ConnectionSettings settings, string sql)
        {
            if (!ScriptSupportsTransactions(sql))
            {
                ExecuteNonQuery(settings, sql, true);
                return;
            }
                
            //do all this in a single transaction
            using (var scope = new TransactionScope())
            {
                string connectionString = _connectionStringGenerator.GetConnectionString(settings, true);
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
                    scope.Complete();
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

        /// <summary>
        /// Some commands are not allowed inside transactions
        /// http://msdn.microsoft.com/en-us/library/ms191544.aspx
        /// </summary>
        public bool ScriptSupportsTransactions(string sql)
        {
            if (sql.IndexOf("ALTER DATABASE", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("ALTER FULLTEXT CATALOG ", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("ALTER FULLTEXT INDEX ", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("BACKUP ", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("CREATE DATABASE", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("CREATE FULLTEXT CATALOG ", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("CREATE FULLTEXT INDEX", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("DROP DATABASE", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("DROP FULLTEXT CATALOG", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("DROP FULLTEXT INDEX", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("RECONFIGURE", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (sql.IndexOf("RESTORE ", StringComparison.OrdinalIgnoreCase) >= 0) return false;

            //UPDATE STATISTICS can be used inside an explicit transaction. However, UPDATE STATISTICS commits independently of the enclosing transaction and cannot be rolled back.

            return true;
        }

    }
}