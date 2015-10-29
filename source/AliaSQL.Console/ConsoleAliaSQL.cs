using System;
using System.Collections.Generic;
using System.Diagnostics;
using AliaSQL.Core;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;

namespace AliaSQL.Console
{
    public class ConsoleAliaSQL:ITaskObserver
    {
        IDictionary<string,string> _properties = new Dictionary<string, string>();
        public void Log(string message)
        {
            System.Console.WriteLine(message);
        }

        public void SetVariable(string name, string value)
        {
            if(_properties.ContainsKey(name))
            {
                _properties[name] = value;
            }
            else
            {
                _properties.Add(name, value);    
            }
            
        }

        public bool UpdateDatabase(ConnectionSettings settings, string scriptDirectory, RequestedDatabaseAction action)
        {
            var manager = new SqlDatabaseManager();
            
            var taskAttributes = new TaskAttributes(settings, scriptDirectory)
                                     {
                                         RequestedDatabaseAction = action,
                                     };
            try
            {
                manager.Upgrade(taskAttributes, this);

                foreach (var property in _properties)
                {
                    Log(property.Key +": " + property.Value);
                }
                return true;
            }
            catch (Exception exception)
            {
                var ex = exception;
                do
                {
                    Log("Failure: " + ex.Message);
                    if (ex.Data["Custom"] != null)
                        Log(ex.Data["Custom"].ToString());
                    ex = ex.InnerException;
                } while (ex != null);

            }

            if (Debugger.IsAttached)
                System.Console.ReadLine();

            return false;
        }
    }
}