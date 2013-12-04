using System.Collections.Generic;

using AliaSQL.Core.Services.Impl;
using AliaSQL.Core.Services.Impl.AliaSQL.Core.Services.Impl;

namespace AliaSQL.Core.Services
{
	
	public interface IDatabaseActionResolver
	{
		IEnumerable<DatabaseAction> GetActions(RequestedDatabaseAction requestedDatabaseAction);
	}
}