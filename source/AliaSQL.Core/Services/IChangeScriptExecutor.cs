
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface IChangeScriptExecutor
	{
		void Execute(string fullFilename, ConnectionSettings settings, ITaskObserver taskObserver, bool logOnly = false);

        void ExecuteIfChanged(string fullFilename, ConnectionSettings settings, ITaskObserver taskObserver, bool logOnly = false);
       
	}
}