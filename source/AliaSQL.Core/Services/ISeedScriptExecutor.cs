
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface ISeedScriptExecutor
	{
		void Execute(string fullFilename, ConnectionSettings settings, ITaskObserver taskObserver);
	}
}