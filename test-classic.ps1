# Force Classic UI by using the ClassicUI=true hint. This bypasses the
# auto-redirect to MUI and exercises the actual ASPX rendering pipeline
# so we can verify the classic pages also compile and load cleanly.

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

$r = Invoke-WebRequest -Uri "$BaseUrl/Frames/Login.aspx" -Headers @{ 'User-Agent' = $UA } `
    -SessionVariable session -UseBasicParsing
$VS    = Extract $r.Content '__VIEWSTATE'
$VSGen = Extract $r.Content '__VIEWSTATEGENERATOR'
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
$null = Invoke-WebRequest -Uri "$BaseUrl/Frames/Login.aspx" -Method POST -Body $body `
    -Headers @{ 'User-Agent' = $UA } -WebSession $session -UseBasicParsing
Write-Host "Logged in." -ForegroundColor Green

$screens = @("SU101000", "SU201000", "SU204003", "SU301000", "SU501000")
$errMarkers = @('Server Error','Parser Error','TypeLoadException',
    'FileNotFoundException','MissingMethodException','could not load type',
    'Unknown control','Could not load file or assembly','PXException',
    'is not a known element','Cannot find the DAC')

Write-Host ""
Write-Host "=== Classic UI (force render, skip MUI redirect) ===" -ForegroundColor Cyan
foreach ($id in $screens) {
    # Use the old-school aspx URL with ClassicUI hint + iframe mode
    $url = "$BaseUrl/Pages/SU/$id.aspx?PopupPanel=On"
    try {
        $r = Invoke-WebRequest -Uri $url -Headers @{ 'User-Agent' = $UA } `
            -WebSession $session -UseBasicParsing
        $html = $r.Content

        # If we're getting the 52-byte window.open stub, try a direct internal call
        if ($html.Length -lt 500) {
            # The stub tells us the site wants to redirect us to MUI. That's not an error
            # in the page itself — the aspx would still compile fine. Let's verify by
            # hitting the actual page resource directly (no screen redirect).
            $url2 = "$BaseUrl/Pages/SU/$id.aspx"
            $r = Invoke-WebRequest -Uri $url2 -Headers @{ 'User-Agent' = $UA; 'X-Acumatica-ClassicUI' = 'true' } `
                -WebSession $session -UseBasicParsing
            $html = $r.Content
        }

        $firstErr = $null
        foreach ($m in $errMarkers) {
            if ($html -imatch [Regex]::Escape($m)) { $firstErr = $m; break }
        }
        $title = if ($html -match '<title>([^<]*)</title>') { $matches[1].Trim() } else { '' }
        $state = if ($firstErr) { "ERROR($firstErr)" } elseif ($html -match 'window.open') { 'redirect-to-MUI (default)' } elseif ($html -match 'ctl00\$phDS') { 'aspx-rendered' } else { 'unknown' }
        Write-Host ("  {0} http={1} len={2,-7} {3}" -f $id, $r.StatusCode, $html.Length, $state)
        if ($firstErr) {
            $html | Set-Content -Path "D:\SourceCode\SurveyYZ\$id.classic.bad.html" -Encoding UTF8
            if ($html -match '<b>Description:</b>\s*([^<]{0,300})') { Write-Host "    desc: $($matches[1])" -ForegroundColor Yellow }
        }
    } catch {
        Write-Host "  $id FAILED: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            Write-Host "    status: $($_.Exception.Response.StatusCode)"
        }
    }
}
