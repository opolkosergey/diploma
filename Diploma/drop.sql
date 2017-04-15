USE master;

DECLARE @kill varchar(8000); SET @kill = '';  
SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), spid) + ';'  
FROM master..sysprocesses  
WHERE dbid = db_id('aspnet-Diploma-c78bd18c-cb2e-4a11-b557-5b00ebc5bc91')

EXEC(@kill); 
drop database [aspnet-Diploma-c78bd18c-cb2e-4a11-b557-5b00ebc5bc91]