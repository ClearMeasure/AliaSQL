
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface IScriptExecutionTracker
	{
		void MarkScriptAsExecuted(ConnectionSettings settings, string scriptFilename, ITaskObserver task);
		bool ScriptAlreadyExecuted(ConnectionSettings settings, string scriptFilename);
  
		void MarkSeedScriptAsExecuted(ConnectionSettings settings, string scriptFilename, ITaskObserver task);
		bool SeedScriptAlreadyExecuted(ConnectionSettings settings, string scriptFilename);
	}
}