# Logs in via the UI form POST (which counts as a UI session, not API) and
# fetches each SU* page to verify there are no parser/type errors.

[CmdletBinding()]
param(
    [string]$BaseUrl  = "http://localhost/Survey",
    [string]$User     = "admin",
    [string]$Password = "123",
    [string]$Company  = "Company"
)

$ErrorActionPreference = "Stop"
$UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36"

function Extract($html, $name) {
    if ($html -match "name=`"$name`"[^>]*value=`"([^`"]*)`"") { return $matches[1] }
    return ''
}

# 1. GET login
$r = Invoke-WebRequest -Uri "$BaseUrl/Frames/Login.aspx" -Headers @{ 'User-Agent' = $UA } `
    -SessionVariable session -UseBasicParsing
$VS    = Extract $r.Content '__VIEWSTATE'
$VSGen = Extract $r.Content '__VIEWSTATEGENERATOR'

# 2. POST login
$body = @{
    '__EVENTTARGET' = ''; '__EVENTARGUMENT' = ''; '__LASTFOCUS' = ''
    '__VIEWSTATE' = $VS; '__VIEWSTATEGENERATOR' = $VSGen
    'ctl00$txtLoginBgIndex' = 'login_bg6.jpg'
    'ctl00$phUser$txtSingleCompany' = $Company
    'ctl00$phUser$txtUser' = $User
    'ctl00$phUser$txtPass' = $Password
    'ctl00$phUser$rememberDevice' = 'on'
    'ctl00$phUser$MultiFactorPipelineNotStarted' = 'true'
    'ctl00$phUser$btnLogin' = 'Sign In'
}
$r = Invoke-WebRequest -Uri "$BaseUrl/Frames/Login.aspx" -Method POST -Body $body `
    -Headers @{ 'User-Agent' = $UA } -WebSession $session -UseBasicParsing
$finalUrl = $r.BaseResponse.ResponseUri.AbsoluteUri
Write-Host "Login OK -> $finalUrl" -ForegroundColor Green

$screens = @("SU101000", "SU201000", "SU204003", "SU301000", "SU501000")
$errMarkers = @('Server Error','Parser Error','TypeLoadException',
    'FileNotFoundException','MissingMethodException','could not load type',
    'Unknown control','Could not load file or assembly','PXException',
    'An error has occurred')

# ===== Classic UI tests =====
Write-Host ""
Write-Host "=== Classic UI (Pages/SU/*.aspx via Main.aspx) ===" -ForegroundColor Cyan
foreach ($id in $screens) {
    # Navigate through Main.aspx to establish the session for this screen
    $null = Invoke-WebRequest -Uri "$BaseUrl/Main.aspx?ScreenId=$id" -Headers @{ 'User-Agent' = $UA } `
        -WebSession $session -UseBasicParsing
    try {
        $r = Invoke-WebRequest -Uri "$BaseUrl/Pages/SU/$id.aspx" `
            -Headers @{ 'User-Agent' = $UA; 'Referer' = "$BaseUrl/Main.aspx?ScreenId=$id" } `
            -WebSession $session -UseBasicParsing
        $html = $r.Content
        $firstErr = $null
        foreach ($m in $errMarkers) {
            if ($html -imatch [Regex]::Escape($m)) { $firstErr = $m; break }
        }
        $title = if ($html -match '<title>([^<]*)</title>') { $matches[1].Trim() } else { '' }
        $state = if ($firstErr) { "ERROR($firstErr)" } elseif ($html -match 'window.open') { 'redirect-to-MUI' } else { 'rendered' }
        Write-Host ("  {0} http={1} len={2,-7} {3} title='{4}'" -f $id, $r.StatusCode, $html.Length, $state, $title)
        if ($firstErr) { $html | Set-Content -Path "D:\SourceCode\SurveyYZ\$id.classic.bad.html" -Encoding UTF8 }
    } catch {
        Write-Host "  $id FAILED: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# ===== MUI tests: fetch the precompiled tenant template =====
Write-Host ""
Write-Host "=== Modern UI (per-tenant compiled template) ===" -ForegroundColor Cyan
foreach ($id in $screens) {
    $url = "$BaseUrl/Scripts/Screens/Company/$id.html"
    try {
        $r = Invoke-WebRequest -Uri $url -Headers @{ 'User-Agent' = $UA } `
            -WebSession $session -UseBasicParsing
        $html = $r.Content
        # Should contain bundle.js script tag referencing our screen
        $hasBundle = $html -match "$id\..*\.bundle\.js"
        Write-Host ("  {0} http={1} len={2,-7} bundle={3}" -f $id, $r.StatusCode, $html.Length, $hasBundle)
    } catch {
        Write-Host "  $id FAILED: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# ===== MUI tests: fetch screen info JSON which the SPA uses =====
Write-Host ""
Write-Host "=== Modern UI screen-info JSON (exercises TSScreenInfo) ===" -ForegroundColor Cyan
foreach ($id in $screens) {
    try {
        $r = Invoke-WebRequest -Uri "$BaseUrl/App_Data/TSScreenInfo/Company/$id.json" `
            -Headers @{ 'User-Agent' = $UA } -WebSession $session -UseBasicParsing
        Write-Host ("  {0} http={1} len={2}" -f $id, $r.StatusCode, $r.Content.Length)
    } catch {
        try {
            $r = Invoke-WebRequest -Uri "$BaseUrl/api/screen/ScreenId?id=$id" `
                -Headers @{ 'User-Agent' = $UA } -WebSession $session -UseBasicParsing
            Write-Host ("  {0} via api/screen: http={1} len={2}" -f $id, $r.StatusCode, $r.Content.Length)
        } catch {
            Write-Host ("  {0}: couldn't fetch screen-info" -f $id) -ForegroundColor Yellow
        }
    }
}
