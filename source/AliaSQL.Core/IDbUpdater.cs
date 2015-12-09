using System.Collections.Generic;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;

namespace AliaSQL.Core
{
    public interface IDbUpdater : ITaskObserver
    {
        new void Log(string message);
        new void SetVariable(string name, string value);
        AliaSqlResult UpdateDatabase(string connectionString, RequestedDatabaseAction action, string scriptDirectory = "");
        List<string> PendingChanges(string connectionString, string scriptDirectory = "");
        List<string> PendingTestData(string connectionString, string scriptDirectory = "");
        bool DatabaseExists(string connectionString);
        int DatabaseVersion(string connectionString);
    }
}