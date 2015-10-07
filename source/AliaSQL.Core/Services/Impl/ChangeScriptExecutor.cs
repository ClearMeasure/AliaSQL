﻿using System;
using System.IO;
using System.Security.Cryptography;
using AliaSQL.Core.Model;
using AliaSQL.Core;

namespace AliaSQL.Core.Services.Impl
{

    public class ChangeScriptExecutor : IChangeScriptExecutor
    {
        private readonly IScriptExecutionTracker _executionTracker;
        private readonly IQueryExecutor _executor;
        private readonly IFileSystem _fileSystem;

        public ChangeScriptExecutor(IScriptExecutionTracker executionTracker, IQueryExecutor executor, IFileSystem fileSystem)
        {
            _executionTracker = executionTracker;
            _executor = executor;
            _fileSystem = fileSystem;
        }

        public ChangeScriptExecutor()
            : this(new ScriptExecutionTracker(), new QueryExecutor(), new FileSystem())
        {

        }

        public void Execute(string fullFilename, ConnectionSettings settings, ITaskObserver taskObserver, bool logOnly = false)
        {
            string scriptFilename = getFilename(fullFilename);
            if (_executionTracker.ScriptAlreadyExecuted(settings, scriptFilename))
            {
                taskObserver.Log(string.Format("Skipping (already executed): {0}{1}", getLastFolderName(fullFilename),scriptFilename));
            }
            else
            {
                if (!logOnly)
                {
                    string sql = _fileSystem.ReadTextFile(fullFilename);
                    if (!ScriptSupportsTransactions(sql))
                    {
                        taskObserver.Log(string.Format("Executing: {0}{1}", getLastFolderName(fullFilename),scriptFilename));
                        _executor.ExecuteNonQuery(settings, sql, true);
                    }
                    else
                    {
                        taskObserver.Log(string.Format("Executing: {0}{1} in a transaction", getLastFolderName(fullFilename),scriptFilename));
                        _executor.ExecuteNonQueryTransactional(settings, sql);
                    }
                }
                else
                {
                    taskObserver.Log(string.Format("Executing: {0}{1} in log only mode",getLastFolderName(fullFilename), scriptFilename));
                }

                _executionTracker.MarkScriptAsExecuted(settings, scriptFilename, taskObserver);
            }
        }


        public void ExecuteIfChanged(string fullFilename, ConnectionSettings settings, ITaskObserver taskObserver, bool logOnly = false)
        {
            string scriptFilename = getFilename(fullFilename);
            var scriptFileMD5 = GetFileMD5Hash(fullFilename);
           
            if (_executionTracker.EverytimeScriptShouldBeExecuted(settings, scriptFilename, scriptFileMD5))
            {
                if (!logOnly)
                {
                    string sql = _fileSystem.ReadTextFile(fullFilename);

                    taskObserver.Log(string.Format("Executing: {0}{1}", getLastFolderName(fullFilename), scriptFilename));
                    _executor.ExecuteNonQuery(settings, sql, true);
                }
                else
                {
                    taskObserver.Log(string.Format("Executing: {0}{1} in log only mode", getLastFolderName(fullFilename), scriptFilename));
                }

                _executionTracker.MarkScriptAsExecuted(settings, scriptFilename, taskObserver, scriptFileMD5);
            }
            else
            {
                taskObserver.Log(string.Format("Skipping (unchanged): {0}{1}", getLastFolderName(fullFilename), scriptFilename));
            }
        }

        public void ExecuteAlways(string fullFilename, ConnectionSettings settings, ITaskObserver taskObserver, bool logOnly = false)
        {
            string scriptFilename = getFilename(fullFilename);
            var scriptFileMD5 = GetFileMD5Hash(fullFilename);

            if (!logOnly)
            {
                string sql = _fileSystem.ReadTextFile(fullFilename);

                taskObserver.Log(string.Format("Executing: {0}{1}", getLastFolderName(fullFilename), scriptFilename));
                _executor.ExecuteNonQuery(settings, sql, true);
            }
            else
            {
                taskObserver.Log(string.Format("Executing: {0}{1} in log only mode", getLastFolderName(fullFilename), scriptFilename));
            }

            _executionTracker.MarkScriptAsExecuted(settings, scriptFilename, taskObserver, scriptFileMD5);
        }

        public static string GetFileMD5Hash(string fullFilename)
        {
            string scriptFileMD5;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fullFilename))
                {
                    scriptFileMD5 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                }
            }
            return scriptFileMD5;
        }


        private string getFilename(string fullFilename)
        {
            return Path.GetFileName(fullFilename);
        }

        private string getLastFolderName(string fullFilename)
        {
            string lastfolder = Path.GetFileName(Path.GetDirectoryName(fullFilename));
            if (lastfolder.ToLower() == "create") return string.Empty;
            if (lastfolder.ToLower() == "update") return string.Empty;
            if (lastfolder.ToLower() == "everytime") return string.Empty;
            if (lastfolder.ToLower() == "runalways") return string.Empty;
            return lastfolder + "/";
        }


        /// <summary>
        /// Some commands are not allowed inside transactions
        /// http://msdn.microsoft.com/en-us/library/ms191544.aspx
        /// </summary>
        private bool ScriptSupportsTransactions(string sql)
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

            //Many system stored procedures can't run in a transaction such as sp_fulltext_database
            //More can be added here as they are discovered
            if (sql.IndexOf("sp_fulltext_database", StringComparison.OrdinalIgnoreCase) >= 0) return false;

            //manual override of transactions
            if (sql.IndexOf("--NOTRANSACTION", StringComparison.OrdinalIgnoreCase) >= 0) return false;

            return true;
        }
    }
}