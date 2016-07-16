using System;
using System.Diagnostics;
using System.IO;
using AliaSQL.Core;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;

namespace AliaSQL.Console
{
    public class Program
    {

        private static void Main(string[] args)
        {
            // Debugger.Launch();

            System.Console.Title = "AliaSQL Database Deployment Tool";
            RequestedDatabaseAction requestedDatabaseAction = RequestedDatabaseAction.Default;
            if(args.Length>0) Enum.TryParse(args[0], true, out requestedDatabaseAction);
            if ((args.Length != 4 && args.Length != 6) || requestedDatabaseAction==RequestedDatabaseAction.Default)
            {
                InvalidArguments();
                return;
            }

            ConnectionSettings settings = null;

            var deployer = new ConsoleAliaSQL();
            var action = requestedDatabaseAction;
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
            System.Console.WriteLine( Path.GetFileName(typeof(Program).Assembly.Location) + @" Action(Create|Update|Rebuild|TestData|Baseline|Drop|UpdateCustom) .\SqlExpress DatabaseName  .\DatabaseScripts\ ");
            System.Console.WriteLine(Environment.NewLine + "-- or --"+ Environment.NewLine);
            System.Console.WriteLine( Path.GetFileName(typeof(Program).Assembly.Location) + @" Action(Create|Update|Rebuild|TestData|Baseline|Drop|UpdateCustom) .\SqlExpress DatabaseName  .\DatabaseScripts\ Username Password");
            System.Console.WriteLine(Environment.NewLine + "---------------------------------------------" + Environment.NewLine);           
            System.Console.WriteLine("Create - Creates database and runs scripts in 'Create' and 'Update' folders.");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("Update - Runs scripts in 'Update' folder. If database does not exist it will create it and run scripts in the 'Create' folder first.");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("Rebuild - Drops then recreates database then runs scripts in 'Create' and 'Update' folders");
            System.Console.WriteLine(" "); 
            System.Console.WriteLine("TestData - Runs scripts in 'TestData' folder. Database must already exist. Seed scripts are logged separate from Create and Update scripts.");
            System.Console.WriteLine(" "); 
            System.Console.WriteLine("Baseline - Creates usd_AppliedDatabaseScripts table and logs all current scripts in 'Create' and 'Update' folders as applied without actually running them.");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("Drop - Drops the database");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("UpdateCustom - Runs a custom update routine described by the updatecustom.xml file in the root of the scripts directory.");
            System.Console.WriteLine(" ");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("--- UpdateCustom Instructions -----------------");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("The UpdateCustom action enables granular configuration of AliaSQL's folder processing. ");
            System.Console.WriteLine("UpdateCustom allows you to define your own folders and the behavior AliaSQL will use ");
            System.Console.WriteLine("on the given folder. ");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("The following behaviors are supported: ");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("- new       : Only new files in the folder will be executed and logged. ");
            System.Console.WriteLine("- changed   : Both new and changed files in the folder will be executed and logged. ");
            System.Console.WriteLine("- all       : All files in the folder will be executed and logged. ");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("The UpdateCustom action expects to find an updatecustom.xml file in the root of your ");
            System.Console.WriteLine("scripts directory. It will use that file to determine which folders to run, what order ");
            System.Console.WriteLine("to run them in, and what behavior to run on each folder. You are free to nest subfolders  ");
            System.Console.WriteLine("within configured folders. Execution of files within configured folders will process  ");
            System.Console.WriteLine("using the normal AliaSQL rules. ");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("Here's a sample updatecustom.xml file: ");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?> ");
            System.Console.WriteLine("<folders> ");
            System.Console.WriteLine("  <folder name=\"BaseStructures\" order=\"1\" behavior=\"new\"/> ");
            System.Console.WriteLine("  <folder name=\"AutogenProcs\" order=\"2\" behavior=\"all\"/> ");
            System.Console.WriteLine("  <folder name=\"CustomProcs\" order=\"3\" behavior=\"changed\"/> ");
            System.Console.WriteLine("  <folder name=\"ProcDependentInserts\" order=\"4\" behavior=\"changed\"/> ");
            System.Console.WriteLine("  <folder name=\"FinalProcessing\" order=\"5\" behavior=\"all\"/> ");
            System.Console.WriteLine("</folders> ");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("--------------------");
            System.Console.WriteLine(" ");

            if (Debugger.IsAttached)
                System.Console.ReadLine();
        }
    }
}