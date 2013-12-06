using System;
using System.IO;
using AliaSQL.Core.Model;
using AliaSQL.Core;
using AliaSQL.Infrastructure.DatabaseManager.DataAccess;

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
                taskObserver.Log(string.Format("Skipping (already executed): {0}", scriptFilename));
            }
            else
            {
                taskObserver.Log(string.Format("Executing: {0}", scriptFilename));
                if (!logOnly)
                {
                    string sql = _fileSystem.ReadTextFile(fullFilename);
                    _executor.ExecuteNonQueryTransactional(settings, sql);
                }
                else
                {
                    taskObserver.Log("Executed in log only mode");
                }

                _executionTracker.MarkScriptAsExecuted(settings, scriptFilename, taskObserver);
            }
        }

        private string getFilename(string fullFilename)
        {
            return Path.GetFileName(fullFilename);
        }
    }
}