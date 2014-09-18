The default database name comes from the Assembly Name of this project: $rootnamespace$
  -If the Assembly Name contains '.Database.' it will be removed.
  -Database.Demo, Demo.Database, or DemoDatabase becomes 'Demo'

You can change the the database name by changing the Assembly Name or editing app.config

Place schema creation scripts in the "Scripts/Update" folder named with a numeric prefix in the order you want them to run.

0001-InitialSchema.sql
0002-AddUserTable.sql

Place test data scripts in the "Scripts/TestData" folder named with a numeric prefix in the order you want them to run.

0001-SampleUsers.sql
0002-SampleRoles.sql

Run this project from Visual Studio to run database migrations.

For more information visit https://github.com/ClearMeasure/AliaSQL/
