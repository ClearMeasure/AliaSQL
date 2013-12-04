using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	public interface ISqlDatabaseManager
	{
		void Upgrade(TaskAttributes taskAttributes, ITaskObserver taskObserver);
	}
}