<img src="https://raw.github.com/ericdc1/AliaSQL/master/images/AliaSQL.PNG" alt="AliaSQL" width="400">

What is AliaSQL?
--------------------------------
AliaSQL is a command line tool for database deployments.  

- Most significant business applications rely on at least one relational database for persisting data
- As new features are developed, database schema changes are often necessary – i.e. new tables, columns, views, and stored procedures
- Database schema changes and corresponding code changes must always be deployed together
- While deploying software to a production environment, code files and libraries may usually be deleted or overwritten – - Database files, however, must be intelligently manipulated so as not destroy vital business data
- The development tools available allow developers to make changes to their environement and do not address the problem of applying those changes to additional environments. (i.e. development, quality assurance, staging, production).

How do I get started?
--------------------------------
Check out the [getting started guide](https://github.com/ericdc1/AliaSQL/wiki/Getting-started).

Where can I get it?
--------------------------------
First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install AutoMapper from the package manager console:

    PM> Install-Package AliaSQL
