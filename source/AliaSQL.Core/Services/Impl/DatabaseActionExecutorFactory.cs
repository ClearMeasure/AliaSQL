using System;
using System.Collections.Generic;
using AliaSQL.Core.Services.Impl.AliaSQL.Core.Services.Impl;
using AliaSQL.Core;

namespace AliaSQL.Core.Services.Impl
{
	
	public class DatabaseActionExecutorFactory : IDatabaseActionExecutorFactory
	{
		private readonly IDatabaseActionResolver _resolver;
        private readonly IDataBaseActionLocator _locator;

		public DatabaseActionExecutorFactory(IDatabaseActionResolver resolver, IDataBaseActionLocator locator)
		{
			_resolver = resolver;
			_locator = locator;
		}

	    public DatabaseActionExecutorFactory():this(new DatabaseActionResolver(),new DataBaseActionLocator())
	    {
	        
	    }

	    public IEnumerable<IDatabaseActionExecutor> GetExecutors(RequestedDatabaseAction requestedDatabaseAction)
		{
			IEnumerable<DatabaseAction> actions = _resolver.GetActions(requestedDatabaseAction);

			foreach (DatabaseAction action in actions)
			{
				IDatabaseActionExecutor instance = _locator.CreateInstance(action);
				yield return instance;
			}
		}
	}
}