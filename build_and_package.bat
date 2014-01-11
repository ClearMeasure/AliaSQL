powershell.exe -NoProfile -ExecutionPolicy unrestricted -Command "& { Import-Module '.\lib\psakev4\psake.psm1'; Invoke-psake ci -parameters @{%*}; if ($psake.build_success -eq $false) { exit 1 } else { exit 0 } }" 
pause
