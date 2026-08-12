$ErrorActionPreference = "Stop"

$installerDirectory = $PSScriptRoot
$solutionDirectory = Join-Path (Split-Path $installerDirectory -Parent) "LeaziEnergiaSolar"
$project = Join-Path $solutionDirectory "src\LeaziEnergiaSolar.Wpf\LeaziEnergiaSolar.Wpf.csproj"
$publishDirectory = Join-Path $installerDirectory "publish"
$installerScript = Join-Path $installerDirectory "LeaziEnergiaSolar.iss"

$innoCandidates = @(
    (Join-Path ${env:ProgramFiles(x86)} "Inno Setup 6\ISCC.exe"),
    (Join-Path $env:ProgramFiles "Inno Setup 6\ISCC.exe"),
    (Join-Path ${env:ProgramFiles(x86)} "Inno Setup 7\ISCC.exe"),
    (Join-Path $env:ProgramFiles "Inno Setup 7\ISCC.exe")
)

$innoCompiler = $innoCandidates |
    Where-Object { $_ -and (Test-Path $_) } |
    Select-Object -First 1

if (-not (Test-Path $project)) {
    throw "Projeto não encontrado em: $project"
}

if (-not $innoCompiler) {
    throw "Inno Setup Compiler não encontrado. Instale o Inno Setup e execute novamente."
}

if (Test-Path $publishDirectory) {
    Remove-Item $publishDirectory -Recurse -Force
}

New-Item $publishDirectory -ItemType Directory -Force | Out-Null

dotnet restore $project

dotnet publish `
    $project `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    --output $publishDirectory

$executable = Join-Path $publishDirectory "LeaziEnergiaSolar.Wpf.exe"

if (-not (Test-Path $executable)) {
    throw "Executável publicado não encontrado em: $executable"
}

& $innoCompiler $installerScript

$outputDirectory = Join-Path $installerDirectory "output"
$setup = Get-ChildItem $outputDirectory -Filter "Setup_ControleComissoes_Leazi_*.exe" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $setup) {
    throw "O instalador não foi encontrado na pasta: $outputDirectory"
}

Write-Host ""
Write-Host "Instalador gerado com sucesso:" -ForegroundColor Green
Write-Host $setup.FullName -ForegroundColor Yellow
