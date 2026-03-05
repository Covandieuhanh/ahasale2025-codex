$ErrorActionPreference = 'Stop'

$siteDir = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$iisExpress = Join-Path $env:ProgramFiles 'IIS Express\\iisexpress.exe'

if (-not (Test-Path $iisExpress)) {
    $iisExpress = Join-Path ${env:ProgramFiles(x86)} 'IIS Express\\iisexpress.exe'
}

if (-not (Test-Path $iisExpress)) {
    throw 'Khong tim thay iisexpress.exe. Hay cai IIS Express hoac Visual Studio 2022.'
}

Write-Host 'Starting AhaSale local site at http://localhost:56445'
& $iisExpress /path:$siteDir /port:56445 /clr:v4.0
