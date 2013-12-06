
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface IScriptExecutionTracker
	{
		void MarkScriptAsExecuted(ConnectionSettings settings, string scriptFilename, ITaskObserver task);
		bool ScriptAlreadyExecuted(ConnectionSettings settings, string scriptFilename);

        void MarkTestDataScriptAsExecuted(ConnectionSettings settings, string scriptFilename, ITaskObserver task);
		bool TestDataScriptAlreadyExecuted(ConnectionSettings settings, string scriptFilename);
	}
}