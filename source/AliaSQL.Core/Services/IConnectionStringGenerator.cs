
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface IConnectionStringGenerator
	{
		string GetConnectionString(ConnectionSettings settings, bool includeDatabaseName);
	}
}