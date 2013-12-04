
DECLARE @i INT
DECLARE @error INT
DECLARE @x INT
declare @command nvarchar(4000)

CREATE TABLE #LIVE_SPIDS (nid INT IDENTITY(1,1), spid INT NOT NULL)
SET @error = 0
SET @i = 1

TRUNCATE TABLE #LIVE_SPIDS

INSERT INTO #LIVE_SPIDS
SELECT distinct
    spid
FROM 
    Master.dbo.SYSPROCESSES sp
      inner join sys.dm_exec_sessions es on es.host_process_id = sp.hostprocess
WHERE 
    dbid IN (SELECT dbid FROM Master.dbo.SYSDATABASES WHERE name = '||DatabaseName||') 
AND spid <> @@SPID 
AND es.is_user_process = 1

SET @x = (SELECT MIN(nid) FROM #LIVE_SPIDS)

WHILE @x <= (SELECT MAX(nid) FROM #LIVE_SPIDS)
BEGIN

	SET @command = 'KILL ' + CONVERT(VARCHAR, (SELECT spid FROM #LIVE_SPIDS WHERE nid = @x))

	EXECUTE sp_executesql @command--, NO OUTPUT
	SET @error = @error + @@ERROR

	SET @x = @x + 1

END
