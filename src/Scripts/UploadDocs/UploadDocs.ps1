# Usage:
#   pwsh.exe .\UploadDocs.ps1 -subscriptionId <SubscriptionId> -azureStorageAccount <StorageAccountName> \
#       -azureStorageKey <StorageKey> [-containerName <ContainerName>] [-localFolderPaths <Folder1>,<Folder2>,...]
#
# Example:
#   pwsh.exe .\UploadDocs.ps1 -subscriptionId "00000000-0000-0000-0000-000000000000" \
#       -azureStorageAccount "mystorageacct" -azureStorageKey "myKey==" \
#       -localFolderPaths "D:\STSAAIMLDT\learn-ai102\src\Data\KnowledgeMining\Data\collateral","D:\STSAAIMLDT\learn-ai102\src\Data\KnowledgeMining\Data\reviews" \
#       -containerName "margies"

Param(
    [Parameter(Mandatory = $true)][string]$subscriptionId,
    [Parameter(Mandatory = $true)][string]$azureStorageAccount,
    [Parameter(Mandatory = $true)][string]$azureStorageKey,
    [Parameter()][string[]]$localFolderPaths,
    [Parameter()][string]$containerName = "margies"
)

# If no folders specified, use default KnowledgeMining collateral and reviews
# resolve default folder paths if not provided
if (-not $localFolderPaths) {
    Write-Host "No data folders specified; using default KnowledgeMining paths..."
    $localFolderPaths = @(
        'D:\STSAAIMLDT\learn-ai102\src\Data\KnowledgeMining\Data\collateral',
        'D:\STSAAIMLDT\learn-ai102\src\Data\KnowledgeMining\Data\reviews'
    )
}

# Ensure Azure login
Write-Host "Logging into Azure..."
Connect-AzAccount | Out-Null

# Set the context for the subscription
Write-Host "Setting Azure context..."
Set-AzContext -SubscriptionId $subscriptionId

$storageContext = (New-AzStorageContext -StorageAccountName $azureStorageAccount -StorageAccountKey $azureStorageKey)

# Validate or create the storage container
Write-Host "Ensuring container '$containerName' exists..."
$containerExists = Get-AzStorageContainer -Name $containerName -Context $storageContext -ErrorAction SilentlyContinue
if (-not $containerExists) {
    Write-Host "Container not found. Creating '$containerName'..."
    New-AzStorageContainer -Name $containerName -Context $storageContext | Out-Null
} else {
    Write-Host "Container '$containerName' already exists. Skipping creation."
}

# Upload files to the container
Write-Host "Uploading files from specified folders..."
foreach ($folder in $localFolderPaths) {
    Write-Host "Scanning folder: $folder"
    $files = Get-ChildItem -Path $folder -Recurse -File
    foreach ($file in $files) {
        # Determine the relative blob path
        $relativePath = $file.FullName.Substring($folder.Length + 1).Replace("\\", "/")
        Set-AzStorageBlobContent -File $file.FullName -Container $containerName -Blob $relativePath -Context $storageContext
        Write-Host "Uploaded: $relativePath"
    }
}

Write-Host "All files uploaded successfully!"