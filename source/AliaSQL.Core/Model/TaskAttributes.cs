using AliaSQL.Core.Services.Impl;

namespace AliaSQL.Core.Model
{
    public class TaskAttributes
    {
        public TaskAttributes(ConnectionSettings connectionSettings, string scriptDirectory)
        {
            ConnectionSettings = connectionSettings;
            ScriptDirectory = scriptDirectory;
        }

        public ConnectionSettings ConnectionSettings { get; set; }
        public string ScriptDirectory { get; set; }
        public RequestedDatabaseAction RequestedDatabaseAction { get; set; }

        public bool LogOnly = false;
    }
}