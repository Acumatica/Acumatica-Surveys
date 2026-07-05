# Hits the Acumatica MUI screen-info endpoint for each SU* screen. This
# endpoint actually instantiates the graph and compiles the per-tenant Modern
# UI files, so it surfaces runtime errors (missing types, bad view names,
# compile failures, etc.).

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

Write-Host ""
Write-Host "=== Modern UI screen-info endpoint ===" -ForegroundColor Cyan
# The MUI graph info endpoint. Typical patterns:
#   /Survey/(W(N))/api/ScreenInfo?screenId=SU101000
#   /Survey/api/ScreenInfo?screenId=SU101000
# Try the canonical one.
foreach ($id in $screens) {
    # Attempt to hit the screen info API to force graph compile
    $urls = @(
        "$BaseUrl/api/ScreenInfo?screenId=$id",
        "$BaseUrl/Main.aspx?ScreenId=$id&JsonMetadata=true",
        "$BaseUrl/entity/default/24.200.001/$($id.Substring(2))/?`$top=1"
    )
    foreach ($u in $urls) {
        try {
            $r = Invoke-WebRequest -Uri $u -Headers @{ 'User-Agent' = $UA; 'Accept' = 'application/json' } `
                -WebSession $session -UseBasicParsing -ErrorAction Stop
            Write-Host ("{0} {1} len={2}" -f $id, $r.StatusCode, $r.Content.Length)
            break
        } catch {
            # try next
        }
    }
}

Write-Host ""
Write-Host "=== Test actual screen load via Pages/SU/*.aspx with ScreenId cookie ===" -ForegroundColor Cyan
# Establish a screen session first by hitting Main.aspx, which sets required session state
foreach ($id in $screens) {
    Invoke-WebRequest -Uri "$BaseUrl/Main.aspx?ScreenId=$id" -Headers @{ 'User-Agent' = $UA } `
        -WebSession $session -UseBasicParsing | Out-Null

    # Then hit the actual page; this triggers the graph via PXPage pipeline
    try {
        $r = Invoke-WebRequest -Uri "$BaseUrl/Pages/SU/$id.aspx?PopupPanel=On" `
            -Headers @{ 'User-Agent' = $UA; 'Referer' = "$BaseUrl/Main.aspx?ScreenId=$id" } `
            -WebSession $session -UseBasicParsing
        $html = $r.Content
        $len = $html.Length
        $hasRedirect = $html -match 'window.open'
        $hasDataSource = $html -match 'PXDataSource|pxdatasource'
        $title = if ($html -match '<title>([^<]*)</title>') { $matches[1].Trim() } else { '' }
        Write-Host ("  {0} http={1} len={2,-7} redirect={3} ds={4} title='{5}'" -f `
            $id, $r.StatusCode, $len, $hasRedirect, $hasDataSource, $title)

        $errMarkers = @('Server Error','Parser Error','TypeLoadException',
            'FileNotFoundException','MissingMethodException','could not load type',
            'Unknown control','Could not load file or assembly')
        foreach ($m in $errMarkers) {
            if ($html -imatch [Regex]::Escape($m)) {
                Write-Host "      ERROR: $m" -ForegroundColor Red
                $html | Set-Content -Path "D:\SourceCode\SurveyYZ\$id.bad.html" -Encoding UTF8
            }
        }
    } catch {
        Write-Host "  $id FAILED: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Try Modern UI per-tenant file endpoint
Write-Host ""
Write-Host "=== Per-tenant Modern UI file rendering ===" -ForegroundColor Cyan
foreach ($id in $screens) {
    $urls = @(
        "$BaseUrl/screens/SU/$id/$id.html",
        "$BaseUrl/frontend/screens/SU/$id/$id.html"
    )
    foreach ($u in $urls) {
        try {
            $r = Invoke-WebRequest -Uri $u -Headers @{ 'User-Agent' = $UA } `
                -WebSession $session -UseBasicParsing -ErrorAction Stop
            Write-Host ("  {0} {1} http={2} len={3}" -f $id, $u, $r.StatusCode, $r.Content.Length)
            break
        } catch { }
    }
}
