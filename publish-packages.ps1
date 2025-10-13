# Fail on first error
$ErrorActionPreference = "Stop"

$source = "github"
$token  = $env:GITHUB_TOKEN

if (-not $token) {
    Write-Error "GITHUB_TOKEN environment variable not set. Aborting."
    exit 1
}

Write-Host "Publishing NuGet packages to GitHub Packages..."
$packages = Get-ChildItem -Recurse -Path "artifacts/packages" -Filter *.nupkg | Where-Object { $_.Name -notmatch "symbols.nupkg" }

foreach ($pkg in $packages) {
    Write-Host "→ Pushing $($pkg.Name)..."
    dotnet nuget push $pkg.FullName `
        --source $source `
        --api-key $token `
        --skip-duplicate
}

Write-Host "✅ All packages processed."
