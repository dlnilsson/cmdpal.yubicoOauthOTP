# Source:
# https://github.com/zadjii/CmdPalExtensions/blob/23320c2f2ca41eaebdc0ac2d983dcd464296c130/src/tools/Sign-Msixs.ps1

$gitRoot = git rev-parse --show-toplevel
$DestinationFolder = Join-Path $gitRoot "x64\tmp"

$params = @{
    Endpoint = "https://eus.codesigning.azure.net/"
    CodeSigningAccountName = "dlnilsson-signing-cmdpalext"
    CertificateProfileName = "dlnilsson-cmdpalext-cert-profile"
    FilesFolder = $DestinationFolder
    FilesFolderFilter = "msix"
    FileDigest = "SHA256"
    TimestampRfc3161 = "http://timestamp.acs.microsoft.com"
    TimestampDigest = "SHA256"

    ExcludeEnvironmentCredential = $true
    ExcludeManagedIdentityCredential = $true
    ExcludeSharedTokenCacheCredential = $true
    ExcludeVisualStudioCredential = $true
    ExcludeVisualStudioCodeCredential = $true
    ExcludeAzureCliCredential = $false
    ExcludeAzurePowershellCredential = $true
    ExcludeInteractiveBrowserCredential = $true
}

Invoke-TrustedSigning @params
