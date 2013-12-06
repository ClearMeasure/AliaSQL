
using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface ISchemaInitializer
	{
		void EnsureSchemaCreated(ConnectionSettings settings);
        void EnsureTestDataSchemaCreated(ConnectionSettings settings);
	}
}