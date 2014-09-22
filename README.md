<img src="https://raw.github.com/ClearMeasure/AliaSQL/master/images/AliaSQL.PNG" alt="AliaSQL" width="400">

What is AliaSQL?
--------------------------------
AliaSQL is a command line tool for database deployments. It is a drop in replacement for [Tarantino](https://github.com/HeadspringLabs/Tarantino) with some additional features. 

How do I get started?
--------------------------------

Check out the [getting started guide](https://github.com/ClearMeasure/AliaSQL/wiki/Getting-started).

Check out the [wiki for some background information](https://github.com/ClearMeasure/AliaSQL/wiki/).

Read the blog posts [here](http://sharpcoders.org/post/Introducing-AliaSQL) and [here](http://jeffreypalermo.com/blog/aliasql-the-new-name-in-automated-database-change-management/).

Where can I get it?
--------------------------------
First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget).

We recommend starting with the AliaSQL Kickstarter that creates Create, Update, Everytime, and TestData folders for your SQL scripts and  provides a Visual Studio runner. To get started, create an empty C# console app then install AliaSQL.Kickstarter from the package manager console:

    PM> Install-Package AliaSQL.Kickstarter

To get the the AliaSQL.exe tool by itself install AliaSQL from the package manager console:

    PM> Install-Package AliaSQL

The latest compiled version can be found here: https://github.com/ClearMeasure/AliaSQL/raw/master/nuget/content/scripts/AliaSQL.exe

What else needs done?
--------------------- 
- More unit tests need written around Baseline, TestData, Update, and Everytime 
- There are likely some additional things in SQL scripts that will fail when running in a transaction. More detail on this in the [getting started guide](https://github.com/ClearMeasure/AliaSQL/wiki/Getting-started).
