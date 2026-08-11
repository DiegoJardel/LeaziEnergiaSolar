$ErrorActionPreference = "Stop"

$project = Join-Path $PSScriptRoot `
    "src\LeaziEnergiaSolar.Wpf\LeaziEnergiaSolar.Wpf.csproj"

$output = Join-Path $PSScriptRoot "publicacao\win-x64"

if (Test-Path $output) {
    Remove-Item $output -Recurse -Force
}

dotnet restore $project

dotnet publish $project `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    --output $output

Write-Host "Publicação criada em: $output" -ForegroundColor Green
