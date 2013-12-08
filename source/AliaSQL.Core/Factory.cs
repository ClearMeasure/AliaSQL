using System;
using System.Collections.Generic;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;

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