declare @databaseVersion int;

select 
    @databaseVersion = count(1) 
from 
    usd_AppliedDatabaseScript

update 
    usd_AppliedDatabaseScript 
set 
    Version = @databaseVersion 
where 
    Version is null

select @databaseVersion