param(
    [string]$Environment = "via-assurance-prod",
    [string]$Region      = "eu-west-1",
    [string]$Project     = "ClientApp/ClientApp.csproj",
    [string]$OutputDir   = "./publish",
    [string]$ZipPath     = "./deploy.zip"
)

Write-Host "=== VIA Assurance ERP - Deploiement Elastic Beanstalk ===" -ForegroundColor Cyan

# 1. Publish
Write-Host "[1/4] Publication .NET 10..." -ForegroundColor Yellow
dotnet publish $Project -c Release -o $OutputDir
if ($LASTEXITCODE -ne 0) { Write-Error "Echec publication"; exit 1 }
Write-Host "OK" -ForegroundColor Green

# 2. Zip
Write-Host "[2/4] Creation ZIP..." -ForegroundColor Yellow
if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
Compress-Archive -Path "$OutputDir/*" -DestinationPath $ZipPath -Force
Write-Host "ZIP cree : $ZipPath" -ForegroundColor Green

# 3. Deploy
Write-Host "[3/4] Deploiement EB ($Environment)..." -ForegroundColor Yellow
eb deploy $Environment --region $Region --label "deploy-$(Get-Date -Format 'yyyyMMdd-HHmm')"
if ($LASTEXITCODE -ne 0) { Write-Error "Echec deploiement EB"; exit 1 }
Write-Host "Deploiement reussi" -ForegroundColor Green

# 4. Status
Write-Host "[4/4] Statut de l'environnement..." -ForegroundColor Yellow
eb status $Environment --region $Region

Write-Host "=== Deploiement termine ===" -ForegroundColor Cyan
