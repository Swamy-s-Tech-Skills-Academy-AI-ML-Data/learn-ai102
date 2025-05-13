Param(
    [Parameter(Mandatory=$true)][string]$subscriptionId,
    [Parameter(Mandatory=$true)][string]$azureStorageAccount,
    [Parameter(Mandatory=$true)][string]$azureStorageKey,
    [Parameter()][string]$localFolderPath = f"D:\STSAAIMLDT\learn-ai102\src\Data\KnowledgeMining\Data",
    [Parameter()][string]$containerName    = "margies"
)

# Set values for your storage account
# $subscriptionId          = "YOUR_SUBSCRIPTION_ID"
# $azureStorageAccount    = "YOUR_AZURE_STORAGE_ACCOUNT_NAME"
# $azureStorageKey        = "YOUR_AZURE_STORAGE_KEY"

# Set the context for the subscription
Write-Host "Setting Azure context..."
Set-AzContext -SubscriptionId $subscriptionId

$storageContext = (New-AzStorageContext -StorageAccountName $azureStorageAccount -StorageAccountKey $azureStorageKey)

# Create the storage container
Write-Host "Creating container..."
New-AzStorageContainer -Name $containerName -Context $storageContext | Out-Null

# Upload files to the container
Write-Host "Uploading files..."
# Recursively get all files in the local folder
$files = Get-ChildItem -Path $localFolderPath -Recurse -File

# Upload each file to the blob container
foreach ($file in $files) {
    # Determine the relative path for the blob
    $relativePath = $file.FullName.Substring($localFolderPath.Length + 1).Replace("\", "/")
    
    # Upload the file
    Set-AzStorageBlobContent -File $file.FullName -Container $containerName -Blob $relativePath -Context $storageContext
    Write-Host "Uploaded: $relativePath"
}

Write-Host "All files uploaded successfully!"