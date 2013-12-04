using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;

namespace AliaSQL.Console
{
    public class Program
    {
       
        private static void Main(string[] args)
        {
            System.Console.Title = "AliaSQL Database Deployment Tool"; 
            if (args.Length != 4 && args.Length != 6)
            {
                InvalidArguments();
                return;
            }

            ConnectionSettings settings = null;

            var deployer = new ConsoleAliaSQL();

            var action = (RequestedDatabaseAction)Enum.Parse(typeof(RequestedDatabaseAction), args[0]);
            string server = args[1];
            string database = args[2];
            string scriptDirectory = args[3];
            
            if (args.Length == 4)
            {
                settings = new ConnectionSettings(server, database, true, null, null);
            }

            else if (args.Length == 6)
            {
                string username = args[4];
                string password = args[5];

                settings = new ConnectionSettings(server, database, false, username, password);
            }

            if (deployer.UpdateDatabase(settings, scriptDirectory, action))
            {
                if (Debugger.IsAttached)
                    System.Console.ReadLine();

                return;
            }    

            Environment.ExitCode = 1;
        }

        private static void InvalidArguments()
        {
            System.Console.WriteLine("Invalid Arguments");
            System.Console.WriteLine(" ");
            System.Console.WriteLine( Path.GetFileName(typeof(Program).Assembly.Location) + @" Action(Update|Rebuild|Seed|Baseline) .\SqlExpress DatabaseName  .\DatabaseScripts\ ");
            System.Console.WriteLine(Environment.NewLine + "-- or --"+ Environment.NewLine);
            System.Console.WriteLine( Path.GetFileName(typeof(Program).Assembly.Location) + @" Action(Update|Rebuild|Seed|Baseline) .\SqlExpress DatabaseName  .\DatabaseScripts\ Username Password");
            System.Console.WriteLine(Environment.NewLine + "---------------------------------------------" + Environment.NewLine);           
            //System.Console.WriteLine("Create - Creates database and runs scripts in 'Create' and 'Update' folders.");
            //System.Console.WriteLine(" ");
            System.Console.WriteLine("Update - Runs scripts in 'Update' folder. If database does not exist it will create it and run scripts in the 'Create' folder first.");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("Rebuild - Drops then recreates database then runs scripts in 'Create' and 'Update' folders");
            System.Console.WriteLine(" "); 
            System.Console.WriteLine("Seed - Runs scripts in 'Seed' folder. Database must already exist. Seed scripts are logged separate from Create and Update scripts.");
            System.Console.WriteLine(" "); 
            System.Console.WriteLine("Baseline - Creates AppliedDatabaseScripts table and logs all current scripts in 'Create' and 'Update' folders as applied without actually running them.");

            if (Debugger.IsAttached)
                System.Console.ReadLine();
        }
    }
}