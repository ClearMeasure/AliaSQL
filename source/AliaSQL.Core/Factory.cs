using System;
using System.Collections.Generic;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using AliaSQL.Infrastructure.DatabaseManager.DataAccess;

namespace AliaSQL.Core
{
    public class Factory
    {
        public static ISqlDatabaseManager Create()
        {
            return new SqlDatabaseManager();
        }
    }
}