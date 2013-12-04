
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	public interface IDatabaseActionExecutor
	{
		void Execute(TaskAttributes taskAttributes, ITaskObserver taskObserver);
	}
}