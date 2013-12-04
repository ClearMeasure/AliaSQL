
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface IDatabaseConnectionDropper
	{
		void Drop(ConnectionSettings settings, ITaskObserver taskObserver);
	}
}