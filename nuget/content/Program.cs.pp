using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;

namespace $rootnamespace$
{
    class Program
    {
        static readonly string DatabaseName = GetDatabaseName();
        static readonly string DbServer = GetDatabaseServer();

        static void Main()
        {
            Console.Title = "AliaSQL Database Migrations Visual Studio Runner";
            
            var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            var parentDirectory = currentDirectory.Parent.Parent.FullName;
            var scriptspath = parentDirectory + "\\Scripts\\";
            var deployerpath = scriptspath + "AliaSQL.exe";
            var p = new Process();
            var keySelection = string.Empty;

            while (string.Compare(keySelection, "Exit", StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                if (!string.IsNullOrEmpty(keySelection))
                {
                    Console.WriteLine();
                    var cmdArguments = string.Format("{0} {1} {2} \"{3}", keySelection, DbServer, DatabaseName, scriptspath); 
		    p.StartInfo.FileName = deployerpath;
                    p.StartInfo.Arguments = cmdArguments;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();

                    Console.WriteLine(p.StandardOutput.ReadToEnd());
                    Console.WriteLine("Press Any Key to Continue");
                }
                else
                {
                    DrawMenu();
                }

                var key = Console.ReadKey(true);
                keySelection = GetVerbForKeySelection(key);
            }
        }

        private static void DrawMenu()
        {
            Console.Clear();

            Console.WriteLine(" Database: " + DatabaseName);
            Console.WriteLine(" Server: " + DbServer);
            Console.WriteLine(" ----------------------------------------------------------------------------");
            Console.WriteLine(" 1. Update");
            Console.WriteLine(" 2. Create");
            Console.WriteLine(" 3. Rebuild");
            Console.WriteLine(" 4. TestData");
            Console.WriteLine(" 5. Baseline");
            Console.WriteLine(" 6. Exit program");
        }

        /// <summary>
        ///returns project name and removes the word ".database."
        /// </summary>
        /// <returns></returns>
        private static string GetDatabaseName()
        {
            var databasename = ConfigurationManager.AppSettings["DatabaseName"];

            if (string.IsNullOrEmpty(databasename))
            {
                var projectname = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

                databasename = projectname.Replace("Database.", "").Replace(".Database", "").Replace("Database", "");
            }
            return databasename;
        }

        private static string GetDatabaseServer()
        {
            var servername = ConfigurationManager.AppSettings["DatabaseServer"];

            if (string.IsNullOrEmpty(servername))
            {
                servername = ".\\sqlexpress";
            }
            return servername;
        }

        private static string GetVerbForKeySelection(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    return "Update";
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    return "Create";
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    return "Rebuild";
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    return "TestData";
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    return "Baseline";
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    return "Exit";
                default:
                    return string.Empty;
            }
        }
    }
}