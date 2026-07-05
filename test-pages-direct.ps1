# Tests published ASPX pages directly by hitting Pages/SU/SU*.aspx.
# This exercises the page markup and compiled code-behind, surfacing any
# runtime errors (missing types, wrong view names, etc.).

[CmdletBinding()]
param(
    [string]$BaseUrl  = "http://localhost/Survey",
    [string]$User     = "admin",
    [string]$Password = "123",
    [string]$Company  = "Company"
)

$ErrorActionPreference = "Stop"
$UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36"

# Login
$loginBody = @{ name = $User; password = $Password; company = $Company } | ConvertTo-Json
Invoke-WebRequest -Uri "$BaseUrl/entity/auth/login" -Method POST `
    -Body $loginBody -ContentType "application/json" `
    -Headers @{ 'User-Agent' = $UA } `
    -SessionVariable session -UseBasicParsing | Out-Null
Write-Host "Logged in." -ForegroundColor Green

$screens = @("SU101000", "SU201000", "SU204003", "SU301000", "SU501000")
$errMarkers = @('Server Error','Parser Error','could not load',
    'TypeLoadException','FileNotFoundException','MissingMethodException',
    'Exception Details:','--- End of stack trace','An error occurred while processing',
    'yellow screen of death','PXException','is invalid')

Write-Host ""
Write-Host "Testing direct page URLs:" -ForegroundColor Cyan
Write-Host ("-" * 100)

foreach ($id in $screens) {
    $url = "$BaseUrl/Pages/SU/$id.aspx"
    try {
        $r = Invoke-WebRequest -Uri $url -Headers @{ 'User-Agent' = $UA } `
            -WebSession $session -UseBasicParsing
        $html = $r.Content
        $title = if ($html -match '<title>([^<]*)</title>') { $matches[1].Trim() } else { '' }

        $firstErr = $null
        foreach ($m in $errMarkers) {
            if ($html -imatch [Regex]::Escape($m)) { $firstErr = $m; break }
        }
        $status = if ($firstErr) { 'ERROR' } else { 'ok   ' }
        $len = $html.Length
        Write-Host ("{0} {1} http={2} len={3,-7} title='{4}'" -f $id, $status, $r.StatusCode, $len, $title)
        if ($firstErr) {
            Write-Host "   marker: $firstErr" -ForegroundColor Yellow
            $html | Set-Content -Path "D:\SourceCode\SurveyYZ\$id.bad.html" -Encoding UTF8
        }
    } catch {
        Write-Host "$id FAILED: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails -and $_.ErrorDetails.Message) {
            $_.ErrorDetails.Message | Set-Content -Path "D:\SourceCode\SurveyYZ\$id.bad.html" -Encoding UTF8
            Write-Host "   Saved error to $id.bad.html" -ForegroundColor Yellow
        }
    }
}

# Also try the Modern UI route
Write-Host ""
Write-Host "Testing Modern UI routes:" -ForegroundColor Cyan
Write-Host ("-" * 100)
foreach ($id in $screens) {
    $url = "$BaseUrl/main?ScreenId=$id"
    try {
        $r = Invoke-WebRequest -Uri $url -Headers @{ 'User-Agent' = $UA } `
            -WebSession $session -UseBasicParsing
        $html = $r.Content
        $len = $html.Length
        $title = if ($html -match '<title>([^<]*)</title>') { $matches[1].Trim() } else { '' }
        Write-Host ("MUI {0}  http={1} len={2,-7} title='{3}'" -f $id, $r.StatusCode, $len, $title)
    } catch {
        Write-Host "MUI $id FAILED: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Logout
try {
    Invoke-WebRequest -Uri "$BaseUrl/entity/auth/logout" -Method POST `
        -Headers @{ 'User-Agent' = $UA } -WebSession $session -UseBasicParsing | Out-Null
} catch { }
