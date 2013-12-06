using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace $rootnamespace$
{
    class Program
    {
        static string databaseName = GetDatabaseName();
        static string DbServer = GetDatabaseServer();

        static void Main()
        {
            // Change to your number of menuitems.
            const int maxMenuItems = 6;
            var selector = 0;
            Console.Title = "AliaSQL Database Migrations Visual Studio Runner";
            while (selector != maxMenuItems)
            {
                Console.Clear();
                DrawMenu();
                bool good = int.TryParse(Console.ReadLine(), out selector);
                if (good)
                {

                    var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
                    var parentDirectory = currentDirectory.Parent.Parent.FullName;
                    var scriptspath = parentDirectory + "\\scripts\\";
                    var deployerpath = scriptspath + "AliaSQL.exe";
                    var p = new Process();

                    switch (selector)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            string cmdArguments = string.Format("{0} {1} {2} {3}", GetVerbForCase(selector), DbServer, databaseName, scriptspath);
                            p.StartInfo.FileName = deployerpath;
                            p.StartInfo.Arguments = cmdArguments;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.Start();
                            Console.WriteLine(p.StandardOutput.ReadToEnd());
                            Console.WriteLine("Press any key to continue.");
                            break;
                        default:
                            if (selector != maxMenuItems)
                            {
                                ErrorMessage();
                            }
                            break;
                    }
                }
                else
                {
                    ErrorMessage();
                }
                Console.ReadKey();
            }
        }
        private static void ErrorMessage()
        {
            Console.WriteLine("Typing error, press key to continue.");
        }

        private static void DrawMenu()
        {
            Console.WriteLine(" Database: " + databaseName);
            Console.WriteLine(" Server: " + DbServer);
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["DatabaseName"]))
            {
                Console.WriteLine(" ");
                Console.WriteLine(" The default database name comes from the Assembly Name of this project");
                Console.WriteLine(" -If the Assembly Name contains '.Database.' it will be removed.");
                Console.WriteLine(" -Database.Demo, Demo.Database, or DemoDatabase becomes 'Demo' ");
                Console.WriteLine(" ");
                Console.WriteLine(" Change the database name by changing the Assembly Name or editing app.config");
            }

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


        private static string GetVerbForCase(int selector)   
        {
            if (selector == 1) return "Update";
            if (selector == 2) return "Create";
            if (selector == 3) return "Rebuild";
            if (selector == 4) return "TestData";
            if (selector == 5) return "Baseline";
            throw new Exception("invalid selector");
        }
    }
}