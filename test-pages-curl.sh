#!/bin/bash
# Tests classic UI pages after publishing by logging in as admin and
# fetching each SU*.aspx page, scanning for server errors.
set -u

BASE="http://localhost/Survey"
UA="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36"
COOKIES=/tmp/svcookies.txt
rm -f "$COOKIES"

CURL="curl -s -c $COOKIES -b $COOKIES --max-time 30 -H User-Agent:$UA"

# 1. Get login page
$CURL -o /tmp/login1.html "$BASE/Frames/Login.aspx" -w "login_get=%{http_code}\n"

# Extract form fields (VIEWSTATE, VIEWSTATEGENERATOR, EVENTVALIDATION)
VIEWSTATE=$(grep -oE 'id="__VIEWSTATE"[^>]*value="[^"]*"' /tmp/login1.html | sed -E 's/.*value="([^"]*)".*/\1/')
VSGEN=$(grep -oE 'id="__VIEWSTATEGENERATOR"[^>]*value="[^"]*"' /tmp/login1.html | sed -E 's/.*value="([^"]*)".*/\1/')
EV=$(grep -oE 'id="__EVENTVALIDATION"[^>]*value="[^"]*"' /tmp/login1.html | sed -E 's/.*value="([^"]*)".*/\1/')

# Url-encode function
urlenc() {
    python3 -c "import urllib.parse,sys; print(urllib.parse.quote(sys.argv[1]))" "$1"
}

VIEWSTATE_E=$(urlenc "$VIEWSTATE")
VSGEN_E=$(urlenc "$VSGEN")
EV_E=$(urlenc "$EV")

# 2. Login post
BODY="__EVENTTARGET=ctl00%24phF%24btnLogin&__EVENTARGUMENT=&__VIEWSTATE=$VIEWSTATE_E&__VIEWSTATEGENERATOR=$VSGEN_E&__EVENTVALIDATION=$EV_E&ctl00%24phF%24txtUser=admin&ctl00%24phF%24txtPass=123&ctl00%24phF%24cmbCompany=Company&ctl00%24phF%24cmbLocale=en-US"

$CURL -L -o /tmp/login_after.html -X POST -H "Content-Type: application/x-www-form-urlencoded" --data "$BODY" "$BASE/Frames/Login.aspx" -w "login_post=%{http_code} final=%{url_effective}\n"

# 3. Fetch each page
echo
echo "=== Testing published pages ==="
for ID in SU101000 SU201000 SU204003 SU301000 SU501000; do
    OUT=/tmp/$ID.html
    SIZE=$($CURL -L -o "$OUT" -w "%{http_code} %{size_download}\n" "$BASE/Main.aspx?ScreenId=$ID")
    # Scan for typical error markers
    ERR=""
    if grep -qi "Server Error\|Parser Error\|stack trace\|could not load\|TypeLoadException\|FileNotFoundException\|MissingMethod" "$OUT"; then
        ERR="ERROR-DETECTED"
    fi
    TITLE=$(grep -oE '<title>[^<]*</title>' "$OUT" | head -1 | sed -E 's/<[^>]+>//g' | tr -d '\t')
    echo "  $ID => $SIZE  title='$TITLE' $ERR"
done
