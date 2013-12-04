
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface IScriptFolderExecutor
	{
		void ExecuteScriptsInFolder(TaskAttributes taskAttributes, string scriptDirectory, ITaskObserver taskObserver);
        void ExecuteSeedScriptsInFolder(TaskAttributes taskAttributes, string scriptDirectory, ITaskObserver taskObserver);

	}
}