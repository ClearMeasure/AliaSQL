
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface ITestDataScriptExecutor
	{
		void Execute(string fullFilename, ConnectionSettings settings, ITaskObserver taskObserver);
	}
}