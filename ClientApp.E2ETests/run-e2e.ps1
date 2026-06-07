# Script d'exécution des tests E2E.
# Démarre l'application ClientApp, attend que le port HTTPS réponde,
# exécute les tests Playwright, puis arrête l'application.
#
# Usage :
#   pwsh ./run-e2e.ps1
#
# Pré-requis : identifiants renseignés dans .runsettings (ou variables E2E_Username/E2E_Password).

$ErrorActionPreference = "Continue"

$testDir = $PSScriptRoot
$appDir = Join-Path (Split-Path $testDir -Parent) "ClientApp"
$log = Join-Path $testDir "app-run.log"
$err = Join-Path $testDir "app-run.err.log"

Remove-Item $log, $err -ErrorAction SilentlyContinue

# 1. Démarre l'application détachée (sorties redirigées vers des fichiers log).
$app = Start-Process -FilePath "dotnet" -ArgumentList "run --launch-profile https" `
	-WorkingDirectory $appDir -PassThru -WindowStyle Hidden `
	-RedirectStandardOutput $log -RedirectStandardError $err

Write-Host "App PID = $($app.Id). Attente du port 7246..."

# 2. Attend que le port TCP 7246 accepte les connexions (max 90s).
$ready = $false
for ($i = 0; $i -lt 90; $i++) {
	try {
		$tcp = New-Object System.Net.Sockets.TcpClient
		$tcp.Connect("localhost", 7246)
		$tcp.Close()
		$ready = $true
		break
	}
	catch {
		Start-Sleep -Seconds 1
	}
}

Write-Host "Port 7246 ouvert = $ready (apres $i s)"

# 3. Exécute les tests.
$exitCode = 1
if ($ready) {
	Start-Sleep -Seconds 2
	Set-Location $testDir
	dotnet test --settings .runsettings
	$exitCode = $LASTEXITCODE
}
else {
	Write-Host "ERREUR: l'application n'a pas demarre a temps. Derniers logs :"
	Get-Content $log -Tail 20 -ErrorAction SilentlyContinue
}

# 4. Arrête l'application.
Write-Host "Arret de l'app (PID $($app.Id))..."
Stop-Process -Id $app.Id -Force -ErrorAction SilentlyContinue

exit $exitCode
