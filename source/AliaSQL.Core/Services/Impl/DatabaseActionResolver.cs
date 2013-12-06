using System.Collections.Generic;

using AliaSQL.Core.Services.Impl.AliaSQL.Core.Services.Impl;

namespace AliaSQL.Core.Services.Impl
{
	
	public class DatabaseActionResolver : IDatabaseActionResolver
	{
		public IEnumerable<DatabaseAction> GetActions(RequestedDatabaseAction requestedDatabaseAction)
		{
			if (requestedDatabaseAction == RequestedDatabaseAction.Create)
			{
				return new DatabaseAction[] {DatabaseAction.Create, DatabaseAction.Update};
			}
			else if (requestedDatabaseAction == RequestedDatabaseAction.Drop)
			{
				return new DatabaseAction[] {DatabaseAction.Drop};
			}
			else if (requestedDatabaseAction == RequestedDatabaseAction.Rebuild)
			{
				return new DatabaseAction[] { DatabaseAction.Drop, DatabaseAction.Create, DatabaseAction.Update };
			}
            else if (requestedDatabaseAction == RequestedDatabaseAction.TestData)
            {
                return new DatabaseAction[] { DatabaseAction.TestData };
            }
            else if (requestedDatabaseAction == RequestedDatabaseAction.Baseline)
            {
                return new DatabaseAction[] { DatabaseAction.Baseline };
            }
			else
			{
				return new DatabaseAction[] { DatabaseAction.Update };
			}
		}
	}
}