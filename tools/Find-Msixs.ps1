# Source:
# https://github.com/zadjii/CmdPalExtensions/blob/23320c2f2ca41eaebdc0ac2d983dcd464296c130/src/tools/Find-Msixs.ps1

$gitRoot = git rev-parse --show-toplevel
$gitRoot
$winui3Apps = Join-Path $gitRoot "otp\bin\x64\Release\net9.0-windows10.0.22621.0\win-x64\AppPackages"

$winui3Apps
$msixs = Get-ChildItem -Path $winui3Apps -Recurse -File -Filter "*.msix" -exclude "Microsoft.WindowsAppRuntime.1.6.msix"

# write the path of each msix (relative to the git root)
Write-Host "Found the following msix's:"
foreach($msix in $msixs) {
    # Write-Host "* " + $msix.FullName

    Write-Host "*" $msix.FullName.Substring($gitRoot.Length + 1) "`n"

}

$DestinationFolder = Join-Path $gitRoot "x64\tmp"

if(Test-Path $DestinationFolder) {
    Remove-Item -Path $DestinationFolder -Recurse -Force | Out-Null
}
if(-not (Test-Path $DestinationFolder)) {
    New-Item -ItemType Directory -Path $DestinationFolder -Force | Out-Null
}

write-host "Copying msix's to $DestinationFolder..."

foreach($msix in $msixs) {
    Copy-Item -Path $msix -Destination $DestinationFolder -Force
}

write-host "Done"
