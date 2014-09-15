
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface IScriptExecutionTracker
	{
		void MarkScriptAsExecuted(ConnectionSettings settings, string scriptFilename, ITaskObserver task, string md5 = "");
		bool ScriptAlreadyExecuted(ConnectionSettings settings, string scriptFilename);

        bool EverytimeScriptShouldBeExecuted(ConnectionSettings settings, string scriptFilename, string md5);
        void MarkTestDataScriptAsExecuted(ConnectionSettings settings, string scriptFilename, ITaskObserver task);
		bool TestDataScriptAlreadyExecuted(ConnectionSettings settings, string scriptFilename);
	}
}