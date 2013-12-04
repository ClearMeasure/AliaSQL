using System;
using AliaSQL.Core.Services.Impl.AliaSQL.Core.Services.Impl;

namespace AliaSQL.Core.Services.Impl
{
    public interface IDataBaseActionLocator
    {
        IDatabaseActionExecutor CreateInstance(DatabaseAction databaseAction);
    }

    public class DataBaseActionLocator : IDataBaseActionLocator
    {
        public IDatabaseActionExecutor CreateInstance(DatabaseAction databaseAction)
        {
            if (databaseAction != null)
                if(databaseAction.Equals(DatabaseAction.Create))
                {
                    return new DatabaseCreator();
                }
                else if(databaseAction.Equals(DatabaseAction.Update))
                {
                    return new DatabaseUpdater();
                }
                else if (databaseAction.Equals(DatabaseAction.Drop))
                {
                    return new DatabaseDropper();
                }
                else if (databaseAction.Equals(DatabaseAction.Seed))
                {
                    return new DatabaseSeeder();
                }
                else if (databaseAction.Equals(DatabaseAction.Baseline))
                {
                    return new DatabaseBaseliner();
                }

            return null;
        }
    }
}