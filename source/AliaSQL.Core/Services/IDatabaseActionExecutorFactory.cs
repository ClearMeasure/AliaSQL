using System.Collections.Generic;

using AliaSQL.Core.Services.Impl;

namespace AliaSQL.Core.Services
{
	
	public interface IDatabaseActionExecutorFactory
	{
		IEnumerable<IDatabaseActionExecutor> GetExecutors(RequestedDatabaseAction requestedDatabaseAction);
	}
}