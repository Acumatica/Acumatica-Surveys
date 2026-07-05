// Captures remaining screenshots:
//   - SU101000 (retry with longer timeouts)
//   - CS100000 features (retry)
//   - Customization editor Screens tab
//   - Anonymous survey pages (rendered from saved HTML files)

const { chromium } = require("playwright");
const path = require("path");
const fs = require("fs");

const BASE     = "http://localhost/Survey";
const USER     = "admin";
const PASSWORD = "123";
const OUT_DIR  = path.join(__dirname, "images", "fresh");
fs.mkdirSync(OUT_DIR, { recursive: true });

async function login(page) {
    await page.goto(`${BASE}/Frames/Login.aspx`, { waitUntil: "networkidle" });
    await page.fill("#txtUser", USER);
    await page.fill("#txtPass", PASSWORD);
    await Promise.all([
        page.waitForNavigation({ waitUntil: "networkidle", timeout: 45000 }).catch(() => {}),
        page.click("#btnLogin"),
    ]);
    await page.waitForTimeout(5000); // let SPA warm up
}

async function captureMuiScreen(page, id, waitForSelectors) {
    const url = `${BASE}/Main?ScreenId=${id}`;
    console.log(`[${id}] opening`);
    try {
        await page.goto(url, { waitUntil: "load", timeout: 120000 });
    } catch (e) {
        console.log(`[${id}] initial load timed out, retrying...`);
        try {
            await page.goto(url, { waitUntil: "load", timeout: 120000 });
        } catch (e2) {
            console.log(`[${id}] second retry`);
            await page.goto(url, { waitUntil: "networkidle", timeout: 180000 });
        }
    }
    // Scroll to top + wait for screen-specific selector
    await page.evaluate(() => window.scrollTo(0, 0));
    if (waitForSelectors && waitForSelectors.length > 0) {
        for (const sel of waitForSelectors) {
            try {
                await page.waitForSelector(sel, { timeout: 15000 });
                break;
            } catch {}
        }
    }
    await page.waitForTimeout(3500);
    const out = path.join(OUT_DIR, `${id}.png`);
    await page.screenshot({ path: out, fullPage: false });
    console.log(`[${id}] saved ${out}`);
}

async function renderAnonSurvey(browser, label, htmlPath) {
    if (!fs.existsSync(htmlPath)) {
        console.log(`[anon ${label}] missing ${htmlPath}`);
        return;
    }
    const context = await browser.newContext({ viewport: { width: 900, height: 1100 } });
    const page = await context.newPage();
    const fileUrl = "file:///" + htmlPath.replace(/\\/g, "/");
    console.log(`[anon ${label}] rendering ${fileUrl}`);
    await page.goto(fileUrl, { waitUntil: "networkidle", timeout: 30000 });
    await page.waitForTimeout(1500);
    const out = path.join(OUT_DIR, `anon-${label}.png`);
    await page.screenshot({ path: out, fullPage: true });
    console.log(`[anon ${label}] saved ${out}`);
    await context.close();
}

(async () => {
    const browser = await chromium.launch({ headless: true });
    const context = await browser.newContext({
        viewport: { width: 1440, height: 900 },
        userAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36",
    });
    const page = await context.newPage();
    try {
        await login(page);

        await captureMuiScreen(page, "SU101000",
            ["qp-fieldset[id='SurveySetup_Numbering']", "qp-fieldset", "qp-tabbar"]);

        await captureMuiScreen(page, "CS100000",
            ["qp-tree-view", "qp-features-list", "qp-fieldset", "qp-template"]);

        // Customization editor — open list then click first project
        console.log("[cust] opening Customization Projects");
        await page.goto(`${BASE}/Main?ScreenId=SM204505`, { waitUntil: "load", timeout: 60000 });
        await page.waitForTimeout(5000);
        await page.screenshot({ path: path.join(OUT_DIR, "SM204505.png") });

        // Try to click Survey26R1 link in the list
        try {
            const link = await page.locator("text=Survey26R1").first();
            if (await link.count() > 0) {
                await link.click();
                await page.waitForTimeout(6000);
                await page.screenshot({ path: path.join(OUT_DIR, "AU-project-editor-screens.png") });
                // Try to click "Screens" tree item
                const screensNode = await page.locator("text=Screens").first();
                if (await screensNode.count() > 0) {
                    await screensNode.click().catch(() => {});
                    await page.waitForTimeout(3000);
                    await page.screenshot({ path: path.join(OUT_DIR, "AU-screens-populated.png") });
                }
            }
        } catch (e) { console.log("[cust] editor click chain failed:", e.message); }

    } finally {
        // Now render the anon survey pages from saved HTML
        await renderAnonSurvey(browser, "CaseSurvey-p1-welcome", "C:/Users/yuriyzaletskyy/AppData/Local/Temp/dualtest/CaseSurvey-p1.html");
        await renderAnonSurvey(browser, "CaseSurvey-p2-question", "C:/Users/yuriyzaletskyy/AppData/Local/Temp/dualtest/CaseSurvey-p2.html");
        await renderAnonSurvey(browser, "CaseSurvey-p3-progress", "C:/Users/yuriyzaletskyy/AppData/Local/Temp/dualtest/CaseSurvey-p3.html");
        await renderAnonSurvey(browser, "CaseSurvey-p6-thankyou", "C:/Users/yuriyzaletskyy/AppData/Local/Temp/dualtest/CaseSurvey-p6.html");

        await renderAnonSurvey(browser, "ProductFeedback-p1-welcome", "C:/Users/yuriyzaletskyy/AppData/Local/Temp/dualtest/ProductFeedback-p1.html");
        await renderAnonSurvey(browser, "ProductFeedback-p2-question", "C:/Users/yuriyzaletskyy/AppData/Local/Temp/dualtest/ProductFeedback-p2.html");
        await renderAnonSurvey(browser, "ProductFeedback-p6-thankyou", "C:/Users/yuriyzaletskyy/AppData/Local/Temp/dualtest/ProductFeedback-p6.html");

        await browser.close();
    }

    console.log(`\nDone. ${OUT_DIR}:`);
    fs.readdirSync(OUT_DIR).sort().forEach(f => console.log("  " + f));
})();
