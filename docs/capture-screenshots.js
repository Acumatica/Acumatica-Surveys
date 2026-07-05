// Captures screenshots of each Survey MUI screen from a logged-in
// localhost/Survey session. Output goes to docs/images/fresh/.
//
// Run:
//   node docs/capture-screenshots.js

const { chromium } = require("playwright");
const path = require("path");
const fs = require("fs");

const BASE     = "http://localhost/Survey";
const USER     = "admin";
const PASSWORD = "123";
const COMPANY  = "Company";

const OUT_DIR  = path.join(__dirname, "images", "fresh");
const SURVEY_WEBHOOK_URL_BASE = "http://localhost/Survey/Webhooks/Company/9d2fd3bc-aae4-4b34-ab66-f40879be1be2";

fs.mkdirSync(OUT_DIR, { recursive: true });

const screens = [
    { id: "SU101000", label: "Survey Preferences" },
    { id: "SU201000", label: "Surveys" },
    { id: "SU204003", label: "Survey Components" },
    { id: "SU301000", label: "Survey Collector" },
    { id: "SU501000", label: "Process Surveys" },
];

async function waitForScreenReady(page) {
    // Wait until an Acumatica MUI screen fieldset or grid actually renders.
    try {
        await page.waitForSelector(
            "qp-fieldset:not([style*='display: none']), qp-grid:not([style*='display: none']), qp-rich-text-editor, qp-tabbar",
            { timeout: 20000 }
        );
    } catch (e) { /* best-effort */ }
    // Let final layout settle
    await page.waitForTimeout(2500);
}

async function login(page) {
    console.log("[login] navigating");
    await page.goto(`${BASE}/Frames/Login.aspx`, { waitUntil: "networkidle" });
    await page.fill("#txtUser", USER);
    await page.fill("#txtPass", PASSWORD);
    // Company is typically pre-selected (single tenant). If a cmbCompany combo exists and is empty, try to set it.
    try {
        const companyCombo = await page.$("input[name*='cmbCompany']");
        if (companyCombo) {
            await companyCombo.fill(COMPANY).catch(() => {});
        }
    } catch {}
    await Promise.all([
        page.waitForNavigation({ waitUntil: "networkidle", timeout: 30000 }).catch(() => {}),
        page.click("#btnLogin"),
    ]);
    // Sometimes after login you land on the SPA; wait for it to be ready
    await page.waitForTimeout(3000);
    console.log("[login] done, url=", page.url());
}

async function captureScreen(page, id, label) {
    const url = `${BASE}/Main?ScreenId=${id}`;
    console.log(`[screen ${id}] navigating ${url}`);
    await page.goto(url, { waitUntil: "domcontentloaded", timeout: 60000 });
    await waitForScreenReady(page);
    const outPath = path.join(OUT_DIR, `${id}.png`);
    await page.screenshot({ path: outPath, fullPage: false });
    console.log(`[screen ${id}] saved ${outPath}`);
}

async function captureCustomizationEditor(page) {
    // Open Customization Projects (SM204505) - then open Survey26R1 project editor
    const listUrl = `${BASE}/Main?ScreenId=SM204505`;
    console.log(`[cust-editor] opening ${listUrl}`);
    await page.goto(listUrl, { waitUntil: "domcontentloaded" });
    await page.waitForTimeout(3000);
    await page.screenshot({ path: path.join(OUT_DIR, "SM204505-projects-list.png") });

    // Try to navigate to the project editor directly
    const editorUrl = `${BASE}/Main?ScreenId=AU000000&ProjectName=Survey26R1`;
    await page.goto(editorUrl, { waitUntil: "domcontentloaded" });
    await page.waitForTimeout(4000);
    await page.screenshot({ path: path.join(OUT_DIR, "AU000000-project-editor.png") });

    // Try to click "Screens" in the left panel
    const screensLink = await page.locator("text=Screens").first();
    if (await screensLink.count() > 0) {
        await screensLink.click().catch(() => {});
        await page.waitForTimeout(2000);
        await page.screenshot({ path: path.join(OUT_DIR, "AU000000-screens-tab.png") });
    }
}

async function captureAnonymousSurvey(page, surveyId, label) {
    // Use an anonymous browser context so we aren't carrying our logged-in session
    const context = await page.context().browser().newContext({
        viewport: { width: 1200, height: 900 },
    });
    const anon = await context.newPage();

    console.log(`[anon ${surveyId}] opening survey page 1`);
    const url = `${SURVEY_WEBHOOK_URL_BASE}?CollectorToken=${surveyId}&PageNbr=1`;
    await anon.goto(url, { waitUntil: "domcontentloaded", timeout: 30000 });
    await anon.waitForTimeout(2500);
    await anon.screenshot({ path: path.join(OUT_DIR, `anon-${surveyId}-page1-welcome.png`), fullPage: true });

    // Click Next to go to page 2 (first question)
    const nextBtn = anon.locator('button[name="action"][value="next"]').first();
    if (await nextBtn.count() > 0) {
        await nextBtn.click().catch(() => {});
        await anon.waitForTimeout(2000);
        await anon.screenshot({ path: path.join(OUT_DIR, `anon-${surveyId}-page2-question.png`), fullPage: true });

        // Try to pick a radio button (any in the group) and go next again
        const radios = await anon.locator('input[type="radio"]').all();
        if (radios.length > 0) {
            await radios[0].check().catch(() => {});
            await anon.waitForTimeout(500);
            const nextBtn2 = anon.locator('button[name="action"][value="next"]').first();
            if (await nextBtn2.count() > 0) {
                await nextBtn2.click().catch(() => {});
                await anon.waitForTimeout(2000);
                await anon.screenshot({ path: path.join(OUT_DIR, `anon-${surveyId}-page3-progress.png`), fullPage: true });
            }
        }
    }

    await context.close();
}

async function captureCS100000(page) {
    const url = `${BASE}/Main?ScreenId=CS100000`;
    console.log("[features] navigating", url);
    await page.goto(url, { waitUntil: "domcontentloaded" });
    await page.waitForTimeout(3000);
    await page.screenshot({ path: path.join(OUT_DIR, "CS100000-features.png") });
}

(async () => {
    const browser = await chromium.launch({ headless: true });
    const context = await browser.newContext({
        viewport: { width: 1440, height: 900 },
        // Use a supported Chrome UA so the browser-support check in Login.aspx passes
        userAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36",
        ignoreHTTPSErrors: true,
    });
    const page = await context.newPage();

    try {
        await login(page);

        for (const s of screens) {
            try { await captureScreen(page, s.id, s.label); }
            catch (e) { console.error(`[screen ${s.id}] FAILED`, e.message); }
        }

        try { await captureCustomizationEditor(page); }
        catch (e) { console.error("[cust-editor] FAILED", e.message); }

        try { await captureCS100000(page); }
        catch (e) { console.error("[features] FAILED", e.message); }

        try { await captureAnonymousSurvey(page, "000011", "Case Survey"); }
        catch (e) { console.error("[anon 000011] FAILED", e.message); }

        try { await captureAnonymousSurvey(page, "000012", "Product Feedback"); }
        catch (e) { console.error("[anon 000012] FAILED", e.message); }

    } finally {
        await browser.close();
    }

    const saved = fs.readdirSync(OUT_DIR).sort();
    console.log(`\nCaptured ${saved.length} screenshots in ${OUT_DIR}:`);
    saved.forEach(f => console.log("  " + f));
})();
