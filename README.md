![](https://raw.github.com/ericdc1/AliaSQL/master/images/AliaSQL.PNG)

AliaSQL is a command line tool for database deployments

It depends on having a database scripts folder with these 3 folders:
- Create
- Update
- Seed

Example usage:

```dos
AliaSQL.exe [Action] [Database Server] [Scripts path] 
```

Create database and run all scripts in Create folder.
Logs to usd_AppliedDatabaseScript
```dos
AliaSQL.exe Create .\sqlexpress ./scripts  
```

Run all scripts in Create and Update folders that have not yet been ran - expects database to already exist.
Logs to usd_AppliedDatabaseScript
```dos
AliaSQL.exe Update .\sqlexpress ./scripts  
```

Drop and recreate database then run all scripts in Create and Update folders.
Logs to usd_AppliedDatabaseScript
```dos
AliaSQL.exe Rebuild .\sqlexpress ./scripts  
```

Run all scripts in Seed folder that has yet been ran - expects database to already exist.
Logs to usd_AppliedDatabaseSeedScript
```dos
AliaSQL.exe Seed .\sqlexpress ./scripts  
```

Logs but does not execute all scripts in Create and Update folders that have not yet been ran - expects database to already exist. This is to add the usd_AppliedDatabaseScript table and a record of all scripts to a legacy database.
Logs to usd_AppliedDatabaseScript
```dos
AliaSQL.exe Baseline .\sqlexpress ./scripts  
```


I like to create a console application in my solution that contains the Create/Update/Seed folders and a simple program to execute AliaSQL.exe from Visual Studio. Here is an example of this https://github.com/ericdc1/AliaSQL/blob/master/source/Database/Program.cs  I suggest downloading the code to give it a try.

Note that I have Psake set up to build the code, run the (limited) unit tests, create the nuget package, and zip it all up.  This is designed to work with Visual Studio 2013 and SQL Server 2012. It should work against SQL Server 2008 and will compile against older Visual Studio versions with a change in the /p:VisualStudioVersion= setting in default.ps1.

I posted some details on why I did this on our office blog at http://sharpcoders.org/post/Introducing-AliaSQL


Install it via Nuget at https://www.nuget.org/packages/AliaSQL/2.0.0.1202

Or download it via Github releases at https://github.com/ericdc1/AliaSQL/releases

