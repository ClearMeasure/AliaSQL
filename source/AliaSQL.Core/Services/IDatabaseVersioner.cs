
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface IDatabaseVersioner
	{
		void VersionDatabase(ConnectionSettings settings, ITaskObserver taskObserver);
	}
}