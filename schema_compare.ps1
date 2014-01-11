$databaseName = "Demo"
$base_dir = resolve-path .
$source_dir = "$base_dir\source"
$databaseScripts = "$source_dir\Database.Demo\"
$SqlPackage = "C:\Program Files (x86)\Microsoft SQL Server\110\DAC\bin\SqlPackage.exe"
$AliaSQL = "$base_dir\lib\AliaSQL\AliaSQL.exe"
$databaseName_Original = "$databaseName" + "_Original"
$databaseScriptsUpdate = "$databaseScripts\Scripts\Update"
$newScriptName = ((Get-ChildItem $databaseScriptsUpdate -filter "*.sql" | ForEach-Object {[int]$_.Name.Substring(0, 4)} | Sort-Object)[-1] + 1).ToString("0000-") + "$databaseName" + ".sql.temp"


write-host "Building original database..."
& $AliaSQL Rebuild .\sqlexpress "$databaseName_Original" "$databaseScripts\Scripts"   
write-host "`n`nGenerating the diff script"
#generate the needed .dacpac (we'll delete it later)
& $SqlPackage /a:Extract /ssn:.\SQLEXPRESS /sdn:$databaseName /tf:$databaseScripts$databaseName.dacpac 
#generate the diff script
& $SqlPackage /a:Script /op:$databaseScriptsUpdate\$newScriptName /sf:$databaseScripts$databaseName.dacpac /tsn:.\SQLEXPRESS /tdn:"$databaseName_Original"
   
write-host "`n`nCleaning up generated script..."
$scriptLines = Get-Content $databaseScriptsUpdate\$newScriptName
Clear-Content $databaseScriptsUpdate\$newScriptName

$passedLastSqlCmdThing = $false
$skipBlock = $false
$blocksToSkip = "usd_AppliedDatabaseTestDataScript", "IX_usd_DateApplied"
$noDiff = $true
$noDiffLines = "", "GO", "PRINT N'Update complete.';" #these are the only lines left when there are no DB diffs
foreach($line in $scriptLines)
{   
    #don't add anything until we get past the line USE [`$(DatabaseName)]; -- all the previous stuff should be sqlcmd/unncessary junk 
    if ($line -eq "USE [`$(DatabaseName)];")
    {
        $passedLastSqlCmdThing = $true
    }
    #skip any blocks which contain any of the text in the skippable array
    elseif ($blocksToSkip | Where-Object { $line.Contains($_) })
    {
            $skipBlock = $true
    }        
    elseif ($passedLastSqlCmdThing -and -not $skipBlock)
    {
        $newLine = "$line"
        Add-Content  $databaseScriptsUpdate\$newScriptName "$newLine"

        if ($noDiff)
        {
            $noDiff = $noDiffLines.Contains($newLine)
        }
    }
    elseif ($line -eq "GO")
    {
        $skipBlock = $false
    }
}    

write-host "Cleaning up temporary files and databases..."
& del $databaseScripts$databaseName.dacpac 
& sqlcmd -S .\SQLEXPRESS -Q "DROP DATABASE $databaseName_Original"  

if ($noDiff)
{
    Remove-Item $databaseScriptsUpdate\$newScriptName
    write-host "No schema changes found for $databaseName" -foregroundcolor "green"
}
else 
{
    write-host "Please validate the new script $databaseScriptsUpdate\$newScriptName is correct, then rename to .sql and add to the database project" -foregroundcolor "yellow"
}

