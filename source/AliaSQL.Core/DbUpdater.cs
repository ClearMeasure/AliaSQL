using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;

namespace AliaSQL.Core
{
    public class DbUpdater : IDbUpdater
    {
        private StringBuilder sb = new StringBuilder();
        private readonly IQueryExecutor _queryExecutor = new QueryExecutor();

        private readonly IConnectionStringGenerator _connectionStringGenerator = new ConnectionStringGenerator();

        IDictionary<string, string> _properties = new Dictionary<string, string>();

        public void Log(string message)
        {
            sb.AppendLine(message);
        }

        public void SetVariable(string name, string value)
        {
            if (_properties.ContainsKey(name))
            {
                _properties[name] = value;
            }
            else
            {
                _properties.Add(name, value);
            }
        }

        /// <summary>
        /// <para>Runs AliaSQL against a database</para>
        /// <para>Default action is Update but it can be set to other AliaSQL actions</para>
        /// <para>Default script directory is ~/App_Data/scripts/ but it can bet set to any physical path</para>
        /// <para>-If database does not exist it will be created</para>
        /// <para>-Script directory path must exist</para>
        /// <para>Returns an object with a success boolean and a result string</para>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="action"></param>
        /// <param name="scriptDirectory"></param>
        /// <returns>Returns an object with a success boolean and a result string</returns>
        public AliaSqlResult UpdateDatabase(string connectionString, RequestedDatabaseAction action = RequestedDatabaseAction.Update, string scriptDirectory = "")
        {
            if (scriptDirectory == "")
            {
                scriptDirectory = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "scripts");
            }

            if (!Directory.Exists(scriptDirectory))
            {
                throw new ArgumentException("There are no scripts in the defined data directory.");
            }

            if (action == RequestedDatabaseAction.Update && !PendingChanges(connectionString, scriptDirectory).Any())
            {
                return new AliaSqlResult { Result = "No pending changes", Success = true };
            }

            var result = new AliaSqlResult { Success = true };
            var manager = new SqlDatabaseManager();

            var taskAttributes = new TaskAttributes(_connectionStringGenerator.GetConnectionSettings(connectionString), scriptDirectory)
            {
                RequestedDatabaseAction = action,
            };
            try
            {
                manager.Upgrade(taskAttributes, this);
                foreach (var property in _properties)
                {
                    Log(property.Key + ": " + property.Value);
                }
                result.Result = sb.ToString();
            }
            catch (Exception exception)
            {
                result.Success = false;
                var ex = exception;
                do
                {
                    Log("Failure: " + ex.Message);
                    if (ex.Data["Custom"] != null)
                        Log(ex.Data["Custom"].ToString());
                    ex = ex.InnerException;
                } while (ex != null);

            }
            result.Result = sb.ToString();
            return result;
        }

        /// <summary>
        /// <para>Gets list of SQL scripts that have not been ran against the target database</para>
        /// <para>Default script directory is ~/App_Data/scripts/ but it can bet set to any physical path</para>
        /// <para>-Script directory path must exist</para>
        /// <para>Returns a list of string with names of pending sql scripts</para>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="scriptDirectory"></param>
        /// <returns>Returns a list of string with names of pending sql scripts</returns>
        public List<string> PendingChanges(string connectionString, string scriptDirectory = "")
        {
            if (scriptDirectory == "")
            {
                scriptDirectory = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "scripts");
            }

            if (!Directory.Exists(scriptDirectory))
            {
                throw new ArgumentException("There are no scripts in the defined data directory.");
            }

            if (!DatabaseExists(connectionString))
            {
                var result = new List<string>();
                result.Add("Database does not exist");
                return result;
            }

            var filelocator = new SqlFileLocator();
            var allfiles = new List<string>();
            allfiles.AddRange(filelocator.GetSqlFilenames(scriptDirectory, "Create").ToList());
            allfiles.AddRange(filelocator.GetSqlFilenames(scriptDirectory, "Update").ToList());
            allfiles.AddRange(filelocator.GetSqlFilenames(scriptDirectory, "Everytime").ToList());
            var executedfiles = _queryExecutor.GetExecutedScripts(_connectionStringGenerator.GetConnectionSettings(connectionString));
            return allfiles.Select(f => f.Replace(Path.GetDirectoryName(f) + "\\", "")).Except(executedfiles).ToList();
        }

        /// <summary>
        /// <para>Gets list of SQL test data scripts that have not been ran against the target database</para>
        /// <para>Default script directory is ~/App_Data/scripts/ but it can bet set to any physical path</para>
        /// <para>-Script directory path must exist</para>
        /// <para>Returns a list of string with names of pending test data sql scripts</para>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="scriptDirectory"></param>
        /// <returns>RReturns a list of string with names of pending test data sql scripts</returns>
        public List<string> PendingTestData(string connectionString, string scriptDirectory = "")
        {
            if (scriptDirectory == "")
            {
                scriptDirectory = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "scripts");
            }

            if (!Directory.Exists(scriptDirectory))
            {
                throw new ArgumentException("There are no scripts in the defined data directory.");
            }

            if (!DatabaseExists(connectionString))
            {
                var result = new List<string>();
                result.Add("Database does not exist");
                return result;
            }

            var filelocator = new SqlFileLocator();
            var allfiles = new List<string>();
            allfiles.AddRange(filelocator.GetSqlFilenames(scriptDirectory, "TestData").ToList());
            var executedfiles = _queryExecutor.GetExecutedTestDataScripts(_connectionStringGenerator.GetConnectionSettings(connectionString));
            return allfiles.Select(f => f.Replace(Path.GetDirectoryName(f) + "\\", "")).Except(executedfiles).ToList();
        }

        /// <summary>
        /// <para>Returns a boolean if the target database exists</para>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>Returns a boolean if the target database exists</returns>
        public bool DatabaseExists(string connectionString)
        {
            return _queryExecutor.CheckDatabaseExists(_connectionStringGenerator.GetConnectionSettings(connectionString));
        }

        /// <summary>
        /// Returns database version representing the number of scripts that have been ran against the database
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>Returns database version representing the number of scripts that have been ran against the database</returns>
        public int DatabaseVersion(string connectionString)
        {
            return _queryExecutor.DatabaseVersion(_connectionStringGenerator.GetConnectionSettings(connectionString));
        }
    }
}