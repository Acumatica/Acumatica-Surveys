# Logs in via the UI (Frames/Login.aspx form POST), which counts as a UI
# session (not API), then hits each screen's actual page through the
# rendering pipeline. This exercises the graph instantiation and aspx
# compilation — errors in either surface here.

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

# 1. GET login page
Write-Host "GET Login.aspx" -ForegroundColor Cyan
$r = Invoke-WebRequest -Uri "$BaseUrl/Frames/Login.aspx" -Headers @{ 'User-Agent' = $UA } `
    -SessionVariable session -UseBasicParsing
$html = $r.Content
$VS    = Extract $html '__VIEWSTATE'
$VSGen = Extract $html '__VIEWSTATEGENERATOR'

# 2. Submit login form
Write-Host "POST Login.aspx" -ForegroundColor Cyan
$body = @{
    '__EVENTTARGET'              = ''
    '__EVENTARGUMENT'            = ''
    '__LASTFOCUS'                = ''
    '__VIEWSTATE'                = $VS
    '__VIEWSTATEGENERATOR'       = $VSGen
    'ctl00$txtLoginBgIndex'      = 'login_bg6.jpg'
    'ctl00$phUser$txtSingleCompany' = $Company
    'ctl00$phUser$txtUser'       = $User
    'ctl00$phUser$txtPass'       = $Password
    'ctl00$phUser$oneTimePasswordText' = ''
    'ctl00$phUser$rememberDevice' = 'on'
    'ctl00$phUser$MultiFactorPipelineNotStarted' = 'true'
    'ctl00$phUser$MultiFactorHubConnectionFailed' = ''
    'ctl00$phUser$MultiFactorWarninigWasShown'    = ''
    'ctl00$phUser$btnLogin'      = 'Sign In'
    'ctl00$phUser$txtDummyInstallationID' = ''
}
$r = Invoke-WebRequest -Uri "$BaseUrl/Frames/Login.aspx" -Method POST -Body $body `
    -Headers @{ 'User-Agent' = $UA } -WebSession $session -UseBasicParsing
Write-Host "  status: $($r.StatusCode)  final: $($r.BaseResponse.ResponseUri)"

# Print cookies
Write-Host "Cookies:"
$uri = [System.Uri]$BaseUrl
$session.Cookies.GetCookies($uri) | ForEach-Object { Write-Host "  $($_.Name)" }

# 3. Fetch each screen via Main.aspx + then Pages/SU aspx
$screens = @("SU101000", "SU201000", "SU204003", "SU301000", "SU501000")

Write-Host ""
Write-Host "Testing each screen page:" -ForegroundColor Cyan
foreach ($id in $screens) {
    # Establish session for this screen
    $null = Invoke-WebRequest -Uri "$BaseUrl/Main.aspx?ScreenId=$id" -Headers @{ 'User-Agent' = $UA } `
        -WebSession $session -UseBasicParsing

    # Fetch the actual page
    try {
        $r = Invoke-WebRequest -Uri "$BaseUrl/Pages/SU/$id.aspx" `
            -Headers @{ 'User-Agent' = $UA; 'Referer' = "$BaseUrl/Main.aspx?ScreenId=$id" } `
            -WebSession $session -UseBasicParsing
        $html = $r.Content
        $len = $html.Length
        $title = if ($html -match '<title>([^<]*)</title>') { $matches[1].Trim() } else { '' }
        $hasRedirect = $html -match 'window.open'
        $hasForm = $html -match '<form' -and $html -match 'phDS|phF'

        $errMarkers = @('Server Error','Parser Error','TypeLoadException',
            'FileNotFoundException','MissingMethodException','could not load type',
            'Unknown control','Could not load file or assembly','PXException','ArgumentException')
        $firstErr = $null
        foreach ($m in $errMarkers) {
            if ($html -imatch [Regex]::Escape($m)) { $firstErr = $m; break }
        }

        $state = if ($firstErr) { "ERROR($firstErr)" } elseif ($hasRedirect) { 'redirect' } elseif ($hasForm) { 'rendered' } else { 'unknown' }
        Write-Host ("  {0} http={1} len={2,-7} state={3} title='{4}'" -f $id, $r.StatusCode, $len, $state, $title)

        if ($firstErr) {
            $html | Set-Content -Path "D:\SourceCode\SurveyYZ\$id.bad.html" -Encoding UTF8
            # Extract main error text
            if ($html -match '<span[^>]*class="errorMessage"[^>]*>([^<]+)</span>') { Write-Host "    msg: $($matches[1])" -ForegroundColor Red }
            if ($html -match '<b>Description:</b>\s*([^<]+)') { Write-Host "    desc: $($matches[1])" -ForegroundColor Red }
        }
    } catch {
        Write-Host "  $id FAILED: $($_.Exception.Message)" -ForegroundColor Red
    }
}
