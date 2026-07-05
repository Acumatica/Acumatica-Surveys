# Forces classic UI by setting UIVersion=Classic cookie (or whatever Acumatica
# uses). If the aspx pages have any parser/compile/runtime errors, they'll
# surface here even if the default is MUI.

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
    if ($html -match "name=`"$name`"[^>]*value=`"([^`"]*)`"") {
        return $matches[1]
    }
    return ''
}

$r = Invoke-WebRequest -Uri "$BaseUrl/Frames/Login.aspx" -Headers @{ 'User-Agent' = $UA } `
    -SessionVariable session -UseBasicParsing
$VS    = Extract $r.Content '__VIEWSTATE'
$VSGen = Extract $r.Content '__VIEWSTATEGENERATOR'

$body = @{
    '__EVENTTARGET' = ''; '__EVENTARGUMENT' = ''; '__LASTFOCUS' = ''
    '__VIEWSTATE' = $VS; '__VIEWSTATEGENERATOR' = $VSGen
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

# Try several ways to force classic UI rendering
$headers = @{
    'User-Agent' = $UA
    'X-Requested-With' = 'XMLHttpRequest'
    'X-AcumaticaUI' = 'Classic'
    'Accept' = 'text/html,application/xhtml+xml'
}

$screens = @("SU101000", "SU201000", "SU204003", "SU301000", "SU501000")
$errMarkers = @('Server Error','Parser Error','TypeLoadException',
    'FileNotFoundException','MissingMethodException','could not load type',
    'Unknown control','Could not load file or assembly','PXException')

Write-Host ""
Write-Host "=== Classic UI forced ===" -ForegroundColor Cyan
foreach ($id in $screens) {
    # Open via frameset, which is the classic route
    try {
        $r = Invoke-WebRequest -Uri "$BaseUrl/Api/Frameset.htm?ScreenID=$id" `
            -Headers $headers -WebSession $session -UseBasicParsing
        $html = $r.Content
    } catch {
        $html = ''
    }

    # Hit the aspx with a plain Accept that doesn't trigger MUI redirect
    try {
        $r2 = Invoke-WebRequest -Uri "$BaseUrl/Pages/SU/$id.aspx?PopupPanel=On" `
            -Headers @{ 'User-Agent' = $UA; 'Accept' = '*/*'; 'X-Requested-With' = 'XMLHttpRequest' } `
            -WebSession $session -UseBasicParsing
        $h2 = $r2.Content

        $firstErr = $null
        foreach ($m in $errMarkers) {
            if ($h2 -imatch [Regex]::Escape($m)) { $firstErr = $m; break }
        }
        $title = if ($h2 -match '<title>([^<]*)</title>') { $matches[1].Trim() } else { '' }
        $state = if ($firstErr) { "ERROR($firstErr)" } elseif ($h2 -match 'window.open') { 'MUI-redirect' } elseif ($h2 -match 'ctl00\$phDS|PXDataSource') { 'classic-rendered' } else { "unknown(len=$($h2.Length))" }
        Write-Host ("  {0} http={1} {2} title='{3}' len={4}" -f $id, $r2.StatusCode, $state, $title, $h2.Length)
        if ($firstErr) {
            $h2 | Set-Content -Path "D:\SourceCode\SurveyYZ\$id.classic.bad.html" -Encoding UTF8
            if ($h2 -match '<b>Description:</b>\s*([^<]{0,300})') { Write-Host "    desc: $($matches[1])" -ForegroundColor Yellow }
        }
    } catch {
        Write-Host "  $id FAILED: $($_.Exception.Message)" -ForegroundColor Red
    }
}
