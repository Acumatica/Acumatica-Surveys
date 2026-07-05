# Exercises each survey graph by hitting its Contract-Based API endpoint.
# This actually instantiates the graph. If the DLL is missing types or
# methods that would break the page at runtime, this will surface here.

[CmdletBinding()]
param(
    [string]$BaseUrl  = "http://localhost/Survey",
    [string]$User     = "admin",
    [string]$Password = "123",
    [string]$Company  = "Company"
)

$ErrorActionPreference = "Continue"
$UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36"

$loginBody = @{ name = $User; password = $Password; company = $Company } | ConvertTo-Json
try {
    Invoke-WebRequest -Uri "$BaseUrl/entity/auth/login" -Method POST `
        -Body $loginBody -ContentType "application/json" `
        -Headers @{ 'User-Agent' = $UA } `
        -SessionVariable session -UseBasicParsing | Out-Null
    Write-Host "Logged in." -ForegroundColor Green
} catch {
    Write-Host "Login failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

try {
    # Try screen-info endpoint - returns the screen metadata (toolbar, views) if graph compiled
    # Attempt multiple known endpoints
    $endpoints = @(
        @{url = "$BaseUrl/api/ScreenInfo/SU101000"; desc = "ScreenInfo/SU101000"},
        @{url = "$BaseUrl/api/SiteMapInfo/GetNavigationNode?screenID=SU101000"; desc = "SiteMap SU101000"},
        @{url = "$BaseUrl/Main.aspx?ScreenId=SU101000"; desc = "Main.aspx SU101000"}
    )
    foreach ($ep in $endpoints) {
        try {
            $r = Invoke-WebRequest -Uri $ep.url -Headers @{ 'User-Agent' = $UA; 'Accept' = 'application/json,*/*' } `
                -WebSession $session -UseBasicParsing
            Write-Host ("  {0}: http={1} len={2}" -f $ep.desc, $r.StatusCode, $r.Content.Length)
        } catch {
            Write-Host ("  {0}: ERROR {1}" -f $ep.desc, $_.Exception.Message)
        }
    }

    Write-Host ""
    Write-Host "=== Running compiled graph check ===" -ForegroundColor Cyan
    # The /ScreenInfo.ashx endpoint returns screen JSON
    # Try each screen via Main.aspx with the ScreenId, which forces the graph to instantiate
    $screens = @("SU101000", "SU201000", "SU204003", "SU301000", "SU501000")
    foreach ($id in $screens) {
        $url = "$BaseUrl/Main.aspx?ScreenId=$id"
        $r = Invoke-WebRequest -Uri $url -Headers @{ 'User-Agent' = $UA } `
            -WebSession $session -UseBasicParsing
        # Now fetch the actual page via the proper route used by MUI. The MUI route is /Main?screenId=... and
        # internally the SPA will hit various endpoints. Let's check Workspace endpoint
        $workspace = "$BaseUrl/api/v1/workspace/GetWorkspaceIds?screenId=$id"
        try {
            $r2 = Invoke-WebRequest -Uri $workspace -Headers @{ 'User-Agent' = $UA; 'Accept' = 'application/json' } `
                -WebSession $session -UseBasicParsing
            Write-Host ("  {0} workspace: http={1} len={2}" -f $id, $r2.StatusCode, $r2.Content.Length)
        } catch {
            Write-Host ("  {0} workspace: {1}" -f $id, $_.Exception.Message)
        }
    }
} finally {
    try {
        Invoke-WebRequest -Uri "$BaseUrl/entity/auth/logout" -Method POST `
            -Headers @{ 'User-Agent' = $UA } -WebSession $session -UseBasicParsing | Out-Null
    } catch { }
}
