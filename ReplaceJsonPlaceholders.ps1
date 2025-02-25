param (
    [string]$jsonFilePath,
    [string]$projectName
)

# Read the JSON file
$jsonContent = Get-Content -Raw -Path $jsonFilePath | Out-String

# Replace the [ProjectName] placeholder
$jsonContent = $jsonContent -replace '\[ProjectName\]', $projectName

# Write the modified content back to the JSON file
Set-Content -Path $jsonFilePath -Value $jsonContent