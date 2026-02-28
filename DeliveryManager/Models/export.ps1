param(
    [string]$OutputFile = "dump.txt"
)

$root = Get-Location

"// ======================================================" | Set-Content $OutputFile
"// Dump from: $root" | Add-Content $OutputFile
"// Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" | Add-Content $OutputFile
"// ======================================================" | Add-Content $OutputFile

Get-ChildItem -Recurse -Filter *.cs -File |
        Sort-Object FullName |
        ForEach-Object {
            Add-Content $OutputFile "`n`n// ======================================================"
            Add-Content $OutputFile "// FILE: $($_.Name)"
            Add-Content $OutputFile "// PATH: $($_.FullName)"
            Add-Content $OutputFile "// ======================================================`n"
            Get-Content $_.FullName | Add-Content $OutputFile
        }

Write-Host "Done -> $OutputFile"