powershell.exe -NoProfile -ExecutionPolicy unrestricted -Command "& { Import-Module '.\lib\psakev4\psake.psm1'; Invoke-psake GenerateDatabaseDiff;  }" 

pause