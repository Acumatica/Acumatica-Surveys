# Logs into the Survey instance via the REST auth endpoint (cookie-based)
# and fetches each published screen, scanning for server errors.

[CmdletBinding()]
param(
    [string]$BaseUrl  = "http://localhost/Survey",
    [string]$User     = "admin",
    [string]$Password = "123",
    [string]$Company  = "Company"
)

$ErrorActionPreference = "Stop"
$UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36"

# ------------------------------------------------------------------
# 1. Login via REST
# ------------------------------------------------------------------
Write-Host "Logging in via /entity/auth/login ..." -ForegroundColor Cyan
$loginBody = @{ name = $User; password = $Password; company = $Company } | ConvertTo-Json

try {
    $r = Invoke-WebRequest -Uri "$BaseUrl/entity/auth/login" -Method POST `
        -Body $loginBody -ContentType "application/json" `
        -Headers @{ 'User-Agent' = $UA } `
        -SessionVariable session -UseBasicParsing
    Write-Host "  login status: $($r.StatusCode)"
} catch {
    Write-Host "  login failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails) { Write-Host "  Details: $($_.ErrorDetails.Message)" }
    exit 1
}

# Print cookies
Write-Host "Cookies:"
$uri = [System.Uri]$BaseUrl
$session.Cookies.GetCookies($uri) | ForEach-Object {
    $val = $_.Value; if ($val.Length -gt 30) { $val = $val.Substring(0,30) + '...' }
    Write-Host "  $($_.Name) = $val"
}

# ------------------------------------------------------------------
# 2. Fetch each page
# ------------------------------------------------------------------
$screens = @("SU101000", "SU201000", "SU204003", "SU301000", "SU501000")
Write-Host ""
Write-Host "Fetching pages:" -ForegroundColor Cyan
Write-Host ("-" * 100)

$errMarkers = @('Server Error','Parser Error','could not load',
    'TypeLoadException','FileNotFoundException','MissingMethodException',
    'Exception Details:','--- End of stack trace','yellow screen of death')

foreach ($id in $screens) {
    $url = "$BaseUrl/Main.aspx?ScreenId=$id"
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
        }
    }
}

# ------------------------------------------------------------------
# 3. Logout
# ------------------------------------------------------------------
try {
    Invoke-WebRequest -Uri "$BaseUrl/entity/auth/logout" -Method POST `
        -Headers @{ 'User-Agent' = $UA } -WebSession $session -UseBasicParsing | Out-Null
} catch { }
