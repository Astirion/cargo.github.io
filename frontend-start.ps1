# Start CargoGo frontend only
$ErrorActionPreference = 'Continue'

function Test-Command {
    param([string]$Name)
    return [bool](Get-Command $Name -ErrorAction SilentlyContinue)
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$frontendDir = Join-Path $root 'frontend'
$siteUrl = 'http://localhost:3000/frontend/index.html'

# Resolve cmd.exe for child process launch
$cmdExe = (Join-Path $env:SystemRoot 'System32\cmd.exe')
if (-not (Test-Path $cmdExe)) { $cmdExe = 'cmd.exe' }

$hasNode = Test-Command -Name 'node'
$hasNpx  = Test-Command -Name 'npx'

if (-not (Test-Path $frontendDir)) {
    Write-Error "Не найдена папка фронтенда: $frontendDir"
    exit 1
}

if ($hasNode -and $hasNpx) {
    Write-Host "Запуск статического сервера на http://localhost:3000 ..."
    Start-Process -WindowStyle Minimized -FilePath $cmdExe -WorkingDirectory $root -ArgumentList @(
        '/c', 'npx --yes http-server -p 3000 -c-1'
    ) | Out-Null
    Start-Sleep -Seconds 2
    try { Start-Process $siteUrl } catch {}
}
else {
    Write-Warning 'Node.js/npx не найдены. Откроем файл напрямую.'
    $indexPath = Join-Path $frontendDir 'index.html'
    try { Start-Process $indexPath } catch { Write-Error $_ }
    Write-Host 'Для запуска через локальный сервер установите Node.js: https://nodejs.org'
}


