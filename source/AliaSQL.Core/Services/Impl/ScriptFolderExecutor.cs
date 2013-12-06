
using System;
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services.Impl
{
	
	public class ScriptFolderExecutor : IScriptFolderExecutor
	{
		private readonly ISchemaInitializer _schemaInitializer;
		private readonly ISqlFileLocator _fileLocator;
		private readonly IChangeScriptExecutor _scriptExecutor;
        private readonly ITestDataScriptExecutor _testDataScriptExecutor;
		private readonly IDatabaseVersioner _versioner;
	    private readonly IFileFilterService _fileFilterService;
		public ScriptFolderExecutor(ISchemaInitializer schemaInitializer, ISqlFileLocator fileLocator, IChangeScriptExecutor scriptExecutor, ITestDataScriptExecutor testDataScriptExecutor, IDatabaseVersioner versioner, IFileFilterService fileFilterService)
		{
			_schemaInitializer = schemaInitializer;
		    _fileFilterService = fileFilterService;
		    _fileLocator = fileLocator;
			_scriptExecutor = scriptExecutor;
		    _testDataScriptExecutor = testDataScriptExecutor;
			_versioner = versioner;
		}

	    public ScriptFolderExecutor():this(new SchemaInitializer(),new SqlFileLocator(),new ChangeScriptExecutor(), new TestDataScriptExecutor(), new DatabaseVersioner(),new FileFilterService())
	    {
	        
	    }

	    public void ExecuteScriptsInFolder(TaskAttributes taskAttributes, string scriptDirectory, ITaskObserver taskObserver)
		{
            _schemaInitializer.EnsureSchemaCreated(taskAttributes.ConnectionSettings);
            
            var sqlFilenames = _fileLocator.GetSqlFilenames(taskAttributes.ScriptDirectory, scriptDirectory);
            
            var filteredFilenames = _fileFilterService.GetFilteredFilenames(sqlFilenames, taskAttributes.SkipFileNameContaining);
            
			foreach (string sqlFilename in filteredFilenames)
			{
                _scriptExecutor.Execute(sqlFilename, taskAttributes.ConnectionSettings, taskObserver, taskAttributes.LogOnly);
			}

            _versioner.VersionDatabase(taskAttributes.ConnectionSettings, taskObserver);
		}

        public void ExecuteTestDataScriptsInFolder(TaskAttributes taskAttributes, string scriptDirectory, ITaskObserver taskObserver)
        {
            _schemaInitializer.EnsureTestDataSchemaCreated(taskAttributes.ConnectionSettings);

            var sqlFilenames = _fileLocator.GetSqlFilenames(taskAttributes.ScriptDirectory, scriptDirectory);

            var filteredFilenames = _fileFilterService.GetFilteredFilenames(sqlFilenames, taskAttributes.SkipFileNameContaining);

            foreach (string sqlFilename in filteredFilenames)
            {
                _testDataScriptExecutor.Execute(sqlFilename, taskAttributes.ConnectionSettings, taskObserver);
            }
        }
	}
}