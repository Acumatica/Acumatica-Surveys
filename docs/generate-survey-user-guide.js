// Generates docs/Acumatica-Survey-User-Guide.docx with fresh screenshots
// captured from the local http://localhost/Survey instance.
//
// Usage:
//   node docs/generate-survey-user-guide.js
//
// Requires:
//   npm install -g docx
//
// Inputs:
//   docs/images/fresh/*.png (run docs/capture-screenshots.js first).

const fs = require("fs");
const path = require("path");
const {
    Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell, ImageRun,
    Header, Footer, AlignmentType, LevelFormat, HeadingLevel, BorderStyle,
    WidthType, ShadingType, PageNumber, TableOfContents,
} = require("docx");

// ------------------------------------------------------------------
// Paths
// ------------------------------------------------------------------
const REPO   = path.resolve(__dirname, "..");
const FRESH  = path.join(REPO, "docs", "images", "fresh");
const OUT    = path.join(REPO, "docs", "Acumatica-Survey-User-Guide.docx");

const img = (name) => path.join(FRESH, name);

// ------------------------------------------------------------------
// Page layout constants
// ------------------------------------------------------------------
const PAGE_W     = 12240;              // US Letter width  (DXA)
const PAGE_H     = 15840;              // US Letter height (DXA)
const MARGIN     = 1440;               // 1 inch
const CONTENT_W  = PAGE_W - 2 * MARGIN; // 9360 DXA

// On-screen pixel widths chosen to give readable screenshots in print
const MUI_WIDTH_PX     = 580;   // 1440:900 aspect ratio  => 362 px tall
const MUI_HEIGHT_PX    = Math.round(MUI_WIDTH_PX * 900 / 1440);
const ANON_WIDTH_PX    = 340;   // 900:1100 portrait      => 415 px tall
const ANON_HEIGHT_PX   = Math.round(ANON_WIDTH_PX * 1100 / 900);

// ------------------------------------------------------------------
// Helpers
// ------------------------------------------------------------------
function p(text, opts = {}) {
    return new Paragraph({
        children: [new TextRun({ text, ...opts })],
        spacing: { after: 120 },
        ...opts.paragraph,
    });
}

function h1(text) {
    return new Paragraph({
        heading: HeadingLevel.HEADING_1,
        children: [new TextRun(text)],
        pageBreakBefore: true,
    });
}
function h2(text) {
    return new Paragraph({ heading: HeadingLevel.HEADING_2, children: [new TextRun(text)] });
}
function h3(text) {
    return new Paragraph({ heading: HeadingLevel.HEADING_3, children: [new TextRun(text)] });
}

function bullet(text, level = 0, runs = null) {
    return new Paragraph({
        numbering: { reference: "bullets", level },
        children: runs || [new TextRun({ text })],
        spacing: { after: 80 },
    });
}
function numbered(text, level = 0, runs = null) {
    return new Paragraph({
        numbering: { reference: "steps", level },
        children: runs || [new TextRun({ text })],
        spacing: { after: 80 },
    });
}
function body(text, opts = {}) {
    return new Paragraph({
        children: [new TextRun({ text, ...opts })],
        spacing: { after: 160 },
        alignment: AlignmentType.LEFT,
    });
}
function bodyRuns(runs) {
    return new Paragraph({ children: runs, spacing: { after: 160 }, alignment: AlignmentType.LEFT });
}

// Generic screenshot inserter. Defaults to MUI aspect.
function screenshot(fileName, { widthPx = MUI_WIDTH_PX, heightPx = MUI_HEIGHT_PX, alt = "Acumatica screenshot" } = {}) {
    const filePath = img(fileName);
    if (!fs.existsSync(filePath)) {
        return new Paragraph({
            children: [new TextRun({ text: `[missing: ${fileName}]`, italics: true, color: "AA0000" })],
            spacing: { after: 160 },
        });
    }
    const data = fs.readFileSync(filePath);
    const lower = fileName.toLowerCase();
    let type = "png";
    if (lower.endsWith(".jpg") || lower.endsWith(".jpeg")) type = "jpg";
    else if (lower.endsWith(".gif")) type = "gif";

    return new Paragraph({
        alignment: AlignmentType.CENTER,
        children: [new ImageRun({
            type,
            data,
            transformation: { width: widthPx, height: heightPx },
            altText: { title: fileName, description: alt, name: fileName },
        })],
        spacing: { after: 160 },
    });
}

function anonShot(fileName, alt) {
    return screenshot(fileName, { widthPx: ANON_WIDTH_PX, heightPx: ANON_HEIGHT_PX, alt: alt || fileName });
}

function caption(text) {
    return new Paragraph({
        alignment: AlignmentType.CENTER,
        children: [new TextRun({ text: `Figure: ${text}`, italics: true, size: 20, color: "555555" })],
        spacing: { after: 280 },
    });
}

function borderLine(color = "CCCCCC", size = 1) {
    return { style: BorderStyle.SINGLE, size, color };
}
function allBorders(color = "CCCCCC") {
    const b = borderLine(color);
    return { top: b, bottom: b, left: b, right: b };
}

function infoTable(headers, rows) {
    const colCount = headers.length;
    const colWidth = Math.floor(CONTENT_W / colCount);
    const columnWidths = Array(colCount).fill(colWidth);
    columnWidths[columnWidths.length - 1] = CONTENT_W - colWidth * (colCount - 1);

    const headerCells = headers.map((h, i) => new TableCell({
        borders: allBorders(),
        width: { size: columnWidths[i], type: WidthType.DXA },
        shading: { fill: "2E75B6", type: ShadingType.CLEAR, color: "auto" },
        margins: { top: 80, bottom: 80, left: 120, right: 120 },
        children: [new Paragraph({ children: [new TextRun({ text: h, bold: true, color: "FFFFFF" })] })],
    }));
    const dataRows = rows.map(rowCells => new TableRow({
        children: rowCells.map((cell, i) => new TableCell({
            borders: allBorders(),
            width: { size: columnWidths[i], type: WidthType.DXA },
            margins: { top: 80, bottom: 80, left: 120, right: 120 },
            children: Array.isArray(cell) ? cell : [new Paragraph({ children: [new TextRun({ text: String(cell) })] })],
        })),
    }));

    return new Table({
        width: { size: CONTENT_W, type: WidthType.DXA },
        columnWidths,
        rows: [new TableRow({ tableHeader: true, children: headerCells }), ...dataRows],
    });
}

function callout(kind, text) {
    const palette = {
        Note:    { bg: "E7F1FB", color: "2E75B6" },
        Tip:     { bg: "E8F5E9", color: "2E7D32" },
        Warning: { bg: "FFF4E5", color: "B26A00" },
    }[kind] || { bg: "F2F2F2", color: "444444" };

    return new Table({
        width: { size: CONTENT_W, type: WidthType.DXA },
        columnWidths: [CONTENT_W],
        rows: [new TableRow({
            children: [new TableCell({
                borders: {
                    top:    { style: BorderStyle.SINGLE, size: 6,  color: palette.color },
                    bottom: { style: BorderStyle.SINGLE, size: 6,  color: palette.color },
                    left:   { style: BorderStyle.SINGLE, size: 24, color: palette.color },
                    right:  { style: BorderStyle.SINGLE, size: 6,  color: palette.color },
                },
                width:   { size: CONTENT_W, type: WidthType.DXA },
                shading: { fill: palette.bg, type: ShadingType.CLEAR, color: "auto" },
                margins: { top: 120, bottom: 120, left: 200, right: 160 },
                children: [new Paragraph({
                    children: [
                        new TextRun({ text: `${kind}: `, bold: true, color: palette.color }),
                        new TextRun({ text }),
                    ],
                })],
            })],
        })],
    });
}

// ------------------------------------------------------------------
// Document sections
// ------------------------------------------------------------------

function coverPage() {
    return [
        new Paragraph({
            alignment: AlignmentType.CENTER,
            spacing: { before: 1200, after: 400 },
            children: [new TextRun({ text: "Acumatica Surveys", size: 72, bold: true, color: "2E75B6" })],
        }),
        new Paragraph({
            alignment: AlignmentType.CENTER,
            spacing: { after: 400 },
            children: [new TextRun({ text: "User Guide", size: 44, color: "555555" })],
        }),
        new Paragraph({
            alignment: AlignmentType.CENTER,
            spacing: { after: 600 },
            children: [new TextRun({
                text: "Configure, send and track surveys from inside Acumatica ERP",
                italics: true, size: 26, color: "666666",
            })],
        }),
        screenshot("SU201000.png", { widthPx: 600, heightPx: 375, alt: "Surveys screen in Acumatica MUI" }),
        new Paragraph({
            alignment: AlignmentType.CENTER,
            spacing: { before: 400 },
            children: [
                new TextRun({ text: "Customization package: ", color: "555555" }),
                new TextRun({ text: "Survey26R1.zip", bold: true }),
            ],
        }),
        new Paragraph({
            alignment: AlignmentType.CENTER,
            children: [
                new TextRun({ text: "Acumatica version: ", color: "555555" }),
                new TextRun({ text: "2026 R1 (26.100)", bold: true }),
            ],
        }),
        new Paragraph({
            alignment: AlignmentType.CENTER,
            spacing: { after: 200 },
            children: [
                new TextRun({ text: "Screenshots captured from: ", color: "555555" }),
                new TextRun({ text: "http://localhost/Survey", bold: true, font: "Consolas" }),
            ],
        }),
    ];
}

function tocSection() {
    return [
        h1("Table of Contents"),
        new Paragraph({
            children: [new TableOfContents("Table of Contents", { hyperlink: true, headingStyleRange: "1-3" })],
        }),
        body("To refresh the table of contents in Word, right-click inside it and choose " +
             "“Update Field”, then “Update entire table”."),
    ];
}

function overviewChapter() {
    return [
        h1("1. Overview"),
        body("Acumatica Surveys is a customization package that adds a complete survey / " +
             "questionnaire engine to any Acumatica ERP instance. Originally built as a " +
             "COVID-19 employee wellness tracker, it has grown into a general-purpose tool " +
             "for sending surveys to employees, customers or contacts and capturing their " +
             "responses back inside Acumatica."),

        h2("1.1 What you get after installing the package"),
        body("Once Survey26R1 is imported and published, a new Surveys workspace appears in " +
             "the left-hand menu. It groups together the screens listed below."),
        infoTable(
            ["Screen ID", "Name", "Purpose"],
            [
                ["SU101000", "Survey Preferences",
                    "Numbering sequence, default components, webhook binding, anonymous contact."],
                ["SU204003", "Survey Components",
                    "Reusable HTML/Scriban fragments — welcome page, radio-buttons, progress bar, etc. Ships with a WYSIWYG editor."],
                ["SU201000", "Surveys",
                    "The survey itself: compose the sequence of components and questions, manage recipients, watch collectors and answers."],
                ["SU301000", "Survey Collector",
                    "One record per recipient (the 'ballot'): captures token, status, sent / expired dates and the raw answers."],
                ["SU501000", "Process Surveys",
                    "Mass-generate or mass-send collectors to recipients and start or close surveys."],
            ]
        ),
        body(" "),

        h2("1.2 Architecture at a glance"),
        body("End-to-end, the flow looks like this:"),
        bullet("A survey is built in Acumatica from reusable components and stored in the Survey table."),
        bullet("When you start or send a survey, the Process Surveys screen creates a SurveyCollector row for each recipient, each with a unique token."),
        bullet("The recipient opens the survey via a webhook URL. No Acumatica login is required when the survey allows anonymous answers."),
        bullet("Each page submission is persisted into SurveyCollectorData (raw payload) and, once processed, split into SurveyAnswer rows for reporting."),
        bullet("Dashboards and Generic Inquiries aggregate the answers for management."),
    ];
}

function installationChapter() {
    return [
        h1("2. Installation"),
        body("Before you can configure or use surveys, the customization package must be " +
             "imported and published."),

        h2("2.1 Prerequisites"),
        bullet("A running Acumatica ERP 2026 R1 instance (build 26.100.x or later)."),
        bullet("Administrator access (the admin user is used in examples below)."),
        bullet("The customization package Survey26R1.zip included with this release."),

        h2("2.2 Import and publish the package"),
        body("Open the Customization Projects screen, upload the ZIP, and publish the imported project. The ZIP contains the DLL, Classic ASPX pages, Modern UI templates, SQL schemas and starter data."),
        screenshot("SM204505.png", { alt: "Customization Projects screen" }),
        caption("Customization Projects (SM204505) — where Survey26R1.zip is imported"),

        callout("Note", "After the first publish you may see a prompt asking to overwrite " +
            "existing notification templates or sample survey records. Confirm the overwrite — " +
            "the package ships with a demo \"Case Survey\" that you will use later to verify the flow."),
    ];
}

function configurationChapter() {
    return [
        h1("3. Configuring the Survey Module"),

        h2("3.1 Survey Preferences (SU101000)"),
        body("Survey Preferences is the system-wide configuration screen. You must complete " +
             "at least the Numbering Sequence and Contacts before sending any survey."),
        screenshot("SU101000.png", { alt: "Survey Preferences (SU101000)" }),
        caption("Survey Preferences — Numbering Sequences, Default Components, Default Field Sizes, Notifications"),

        h3("3.1.1 Numbering Sequences"),
        body("Pick a numbering sequence that will generate Survey IDs (for example SV10001, SV10002, …). " +
             "You can create a new sequence directly from the selector by clicking the edit icon."),
        body("Also assign a Bad Request Template — this is the Survey Component rendered when a respondent uses " +
             "an invalid or expired token. Default is SUBADREQUEST."),

        h3("3.1.2 Default Components"),
        body("These fields point to the reusable components the survey generator uses by default when it builds a survey page:"),
        infoTable(
            ["Field", "Purpose"],
            [
                ["Default Main Template", "Wrapper HTML used for every page of the survey."],
                ["Default Header",        "Header shown at the top of every survey page (logo, title)."],
                ["Default Page Header",   "Header fragment rendered on every page (progress bar, title)."],
                ["Default Question",      "Template for a rendered question row."],
                ["Default Question Type", "Attribute providing the radio / drop-down options."],
                ["Default Comment",       "Template for free-text comment fields."],
                ["Default Comment Type",  "Attribute describing how comments are sized."],
                ["Default Page Footer",   "Footer fragment (Previous / Next buttons)."],
                ["Default Footer",        "Footer shown at the bottom of every survey page."],
            ]
        ),
        body(" "),
        callout("Tip", "Start from the pre-loaded components (SUMAIN, SUWELCOME, SUPROGRESS, " +
            "SUPREVNEXT, SURADIOBUTTONS, SUTEXTAREA, SUTHANKYOU, SUBADREQUEST). They are ready " +
            "to use and cover most scenarios."),

        h3("3.1.3 Default Field Sizes"),
        body("Default Comment Rows and Default Max Length set the rendering size of multi-line comment " +
             "fields and the max-length of single-line answers."),

        h3("3.1.4 Notifications and contacts"),
        infoTable(
            ["Field", "Purpose"],
            [
                ["Web Hook",              "Webhook handler that serves survey pages (Survey)."],
                ["New Notification",      "Email template sent when a collector is generated."],
                ["Reminder Notification", "Email template sent when a reminder is triggered."],
                ["Sample Contact",        "Default From: contact for all notifications."],
                ["Anonymous Contact",     "Contact placeholder used for anonymous collectors."],
            ]
        ),
        body(" "),
        callout("Warning", "Anonymous Contact is required for public (AllowAnonymous=true) surveys. " +
            "Without it the webhook endpoint returns 500 for every anonymous request."),

        h2("3.2 Survey Components (SU204003)"),
        body("Survey Components are reusable HTML / Scriban fragments. They are composed into " +
             "survey pages at generation time. The Modern UI version of this screen uses a WYSIWYG " +
             "rich-text editor for the Body field, with a built-in Visual / HTML / Plain Text mode " +
             "switch on the right side of the toolbar."),
        screenshot("SU204003.png", { alt: "Survey Components with WYSIWYG editor" }),
        caption("Survey Components (SU204003) — Component fields on top, full-width rich-text editor below"),

        callout("Note", "Toggle the editor to HTML mode when you need to edit the raw Scriban tags " +
            "such as {{survey.title}} or {{page.next_label}}. Toggle back to Visual mode to preview " +
            "formatting (bold, colors, lists, tables) as the respondent will see them."),

        body("Every component has:"),
        bullet("Component ID — e.g. SURADIOBUTTONS."),
        bullet("Description — shown in selectors and in the page designer."),
        bullet("Component Type — PH (page header), PF (page footer), HE (header), FO (footer), QU (question), CM (comment), MN (main container), BA (bad request)."),
        bullet("Active — whether the component can be used on new surveys."),
        bullet("Body — the Scriban / HTML template that renders the component."),
    ];
}

function usingChapter() {
    return [
        h1("4. Creating a Survey"),

        h2("4.1 Open the Surveys screen (SU201000)"),
        body("The Surveys screen is the main entry point for building a survey. It is laid out as:"),
        bullet("Survey Header (left) — Survey ID, Status, Target, Layout, Title, Template, Entity Type."),
        bullet("Options (left) — Allow Public Answers, Keep Answers Anonymous, Allow Duplicate."),
        bullet("Notifications (right) — New Notification and Reminder Notification templates."),
        bullet("Tabs (bottom) — Details, Recipients, Collectors, Answers."),
        body(" "),
        screenshot("SU201000.png", { alt: "Surveys screen new record" }),
        caption("Surveys (SU201000) — two-column header with Details / Recipients / Collectors / Answers tabs"),

        h2("4.2 Compose the survey"),
        numbered("Click + to start a new survey."),
        numbered("Give it a Title, set the Target (e.g. User or Anonymous) and Layout (Single Page / Multi Page)."),
        numbered("Pick a Template (the default comes from Survey Preferences)."),
        numbered("On the Details tab, add the sequence of components (header, questions, comments, footers). Each row has a Component Type and a Component ID."),
        numbered("For question rows, pick an Attribute — its predefined values will be rendered as radio buttons or a drop-down."),
        body(" "),
        callout("Tip", "On Survey Preferences there is a Create Demo Survey button. It generates " +
            "a ready-made satisfaction survey you can use as a reference or a starting point. Survey 000011 " +
            "in the demo data (\"Acumatica Case Support Satisfaction Survey\") is the result of that action."),

        h2("4.3 Pick recipients"),
        body("On the Recipients tab of the Survey screen you can add users or contacts that will " +
             "receive the survey. For internal surveys, users are linked to Employee records; for " +
             "external surveys, Contact records are used. The Add Recipients action on the toolbar " +
             "opens a filter-driven picker."),
    ];
}

function sendingChapter() {
    return [
        h1("5. Sending Surveys"),

        h2("5.1 Start the survey from the Survey screen"),
        body("The simplest way to send a single survey to its configured recipients is to click the " +
             "Start Survey action on the Surveys (SU201000) toolbar. Acumatica will:"),
        bullet("Generate one SurveyCollector row per recipient, each with a unique token."),
        bullet("Set the survey status to Active."),
        bullet("Fire the New Notification email if Email is one of the recipient channels."),

        h2("5.2 Mass operations from Process Surveys (SU501000)"),
        body("When you need to run a survey against many recipients at once, use Process Surveys. " +
             "The Action dropdown controls what happens:"),
        infoTable(
            ["Action", "What happens"],
            [
                ["Send New Notification",      "Create collectors (if missing) and send the New Notification email."],
                ["Send Reminder Notification", "Send the Reminder Notification email to collectors that are still open."],
                ["Process Answers",            "Convert raw payloads in SurveyCollectorData into SurveyAnswer rows for reporting."],
                ["Close Expired",              "Mark collectors older than Expire After as closed."],
            ]
        ),
        body(" "),
        screenshot("SU501000.png", { alt: "Process Surveys (SU501000)" }),
        caption("Process Surveys (SU501000) — Selection filter on top, Documents grid below"),

        h2("5.3 The webhook URL served to the respondent"),
        body("Each collector carries a token. Emails generated from the notification template contain " +
             "a personalised URL of the form:"),
        bodyRuns([
            new TextRun({
                text: "http://<your-host>/<instance>/Webhooks/<tenant>/<survey-webhook-id>?CollectorToken=<token>&PageNbr=1",
                font: "Consolas", size: 20,
            }),
        ]),
        body("This URL is served by the Survey webhook " +
             "(PX.Survey.Ext.WebHook.SurveyWebhookServerHandler). It renders the correct page of the correct " +
             "survey for the token, and accepts POSTs as the respondent advances from page to page."),

        h2("5.4 Scheduling and business events"),
        body("Two built-in Acumatica mechanisms let you run surveys without any manual action:"),
        bullet("Automation Schedules — run Process Surveys on a cron-like schedule (Send New / Send Reminder / Process Answers)."),
        bullet("Business Events — trigger surveys in response to something happening inside Acumatica (for example, a Case being closed)."),
    ];
}

function respondentChapter() {
    return [
        h1("6. Answering a Survey (Recipient View)"),
        body("Recipients receive an email with a personalised URL. They do not need an Acumatica " +
             "login when the survey allows anonymous answers. The form is served from the Survey " +
             "webhook and works equally well on desktop and mobile."),

        body("The screenshots below are captured from an actual walk-through of the survey " +
             "pipeline against the localhost instance, using two different surveys:"),
        bullet("Survey 000011 — \"Acumatica Case Support Satisfaction Survey\" (the demo that ships with the package)."),
        bullet("Survey 000012 — \"Product Feedback Survey\" (created during end-to-end testing by duplicating 000011)."),
        body("Each was filled end-to-end from a fresh anonymous browser session, answering every required question, and finishing on the Thank-You page."),

        h2("6.1 Case Survey — anonymous walk-through"),
        anonShot("anon-CaseSurvey-p1-welcome.png", "Case Survey welcome page"),
        caption("Case Survey — welcome page (page 1). Token minted, Next button ready"),
        anonShot("anon-CaseSurvey-p2-question.png", "Case Survey Q1"),
        caption("Case Survey — page 2. Progress bar, radio-button question, Previous / Next footer"),
        anonShot("anon-CaseSurvey-p3-progress.png", "Case Survey progress"),
        caption("Case Survey — page 3. Progress continues"),
        anonShot("anon-CaseSurvey-p6-thankyou.png", "Case Survey thank you"),
        caption("Case Survey — final page. The collector is marked Completed (Status = Y)"),

        h2("6.2 Product Feedback Survey — anonymous walk-through"),
        anonShot("anon-ProductFeedback-p1-welcome.png", "Product Feedback welcome"),
        caption("Product Feedback Survey — welcome page (page 1)"),
        anonShot("anon-ProductFeedback-p2-question.png", "Product Feedback Q1"),
        caption("Product Feedback Survey — page 2. Same component library, different survey"),
        anonShot("anon-ProductFeedback-p6-thankyou.png", "Product Feedback thank you"),
        caption("Product Feedback Survey — final page"),

        callout("Note", "Every page submission is persisted immediately into SurveyCollectorData. " +
            "Even if the respondent abandons the survey mid-way, the partial answers are retained " +
            "and can be reviewed later from the Collector screen (SU301000)."),
    ];
}

function trackingChapter() {
    return [
        h1("7. Tracking Results"),

        h2("7.1 Per-collector view (SU301000)"),
        body("The Survey Collector screen shows one respondent's submission in detail:"),
        bullet("Header (left) — Collector ID, Survey ID, Status, Sent On, Expiration Date."),
        bullet("Options (left) — Anonymous, Is Test."),
        bullet("Recipient (right) — Contact, First/Last/Display Name, Source."),
        bullet("Processing (right) — any error message raised while processing answers."),
        bullet("Collected Answers (tab) — one row per page submission."),
        screenshot("SU301000.png", { alt: "Survey Collector (SU301000)" }),
        caption("Survey Collector (SU301000) — two-column layout with Collected Answers tab"),

        h2("7.2 End-to-end test results"),
        body("During the preparation of this guide we filled both 000011 and 000012 from a fresh " +
             "anonymous session and verified the results in the database. Summary of what we observed:"),
        infoTable(
            ["Survey ID", "Title",                                      "Collectors created", "Status", "Page submissions"],
            [
                ["000011", "Acumatica Case Support Satisfaction Survey", "8", "Y (Completed)", "5 pages + thank-you"],
                ["000012", "Product Feedback Survey",                    "9", "Y (Completed)", "5 pages + thank-you"],
            ]
        ),
        body(" "),
        callout("Note", "Both surveys accepted anonymous respondents, captured radio-button " +
            "answers into SurveyCollectorData, and ended with Status = Y on the collector row. " +
            "This confirms the end-to-end pipeline (numbering → collector creation → webhook → " +
            "payload persistence → completion) is functional on the instance."),

        h2("7.3 Answer lifecycle"),
        body("Answers move through the following states:"),
        infoTable(
            ["State", "Meaning"],
            [
                ["Pending (N)",   "Collector created but no submissions yet (respondent has not opened the URL)."],
                ["Started (P)",  "Respondent has opened the survey; page payloads are accumulating in SurveyCollectorData."],
                ["Completed (Y)", "Respondent reached the thank-you page; Process Answers has nothing more to add."],
                ["Closed (C)",   "Collector was marked closed by the operator or by the Close Expired process."],
            ]
        ),
        body(" "),
        callout("Tip", "Schedule Process Surveys → Process Answers to run every few minutes. That " +
            "way dashboards always show results in near real time."),
    ];
}

function troubleshootingChapter() {
    return [
        h1("8. Troubleshooting"),
        body("This table lists the issues we actually hit while commissioning Survey26R1 on a " +
             "clean 2026 R1 instance, with the root cause and the fix for each."),
        infoTable(
            ["Symptom", "Likely cause and fix"],
            [
                ["The Surveys workspace does not appear after publish.",
                    "The customization project's Screens section is empty. Re-import Survey26R1.zip — the package registers each page as a <Page> entry so the editor's Screens tab is populated."],
                ["Error \"Numbering ID is null\" when saving a new survey.",
                    "Survey Preferences is missing a Numbering Sequence, or the sequence's CompanyMask byte is the wrong width. Open SU101000 and pick a numbering sequence; save."],
                ["The anonymous survey URL returns HTTP 500.",
                    "The Anonymous Contact field on Survey Preferences is empty. Assign any existing contact (Employee is fine)."],
                ["The respondent sees a blank page with just Save / Cancel.",
                    "Browser cache — press Ctrl + Shift + R. If it persists after a fresh publish, check App_Data\\TSScreenInfo\\<tenant>\\SU*.json: the \"views\" array must not be empty."],
                ["Modern UI toolbar buttons look chaotic.",
                    "That is the default when many graph actions are public. Use @actionConfig decorators in the screen .ts file to hide or group them."],
            ]
        ),
    ];
}

function appendixChapter() {
    return [
        h1("Appendix A. Screen Reference"),
        infoTable(
            ["Screen ID", "Graph",                              "DAC"],
            [
                ["SU101000",  "PX.Survey.Ext.SurveySetupMaint",         "SurveySetup"],
                ["SU201000",  "PX.Survey.Ext.SurveyMaint",              "Survey / SurveyDetail / SurveyUser / SurveyCollector / SurveyAnswer"],
                ["SU204003",  "PX.Survey.Ext.SurveyComponentMaint",     "SurveyComponent"],
                ["SU301000",  "PX.Survey.Ext.SurveyCollectorMaint",     "SurveyCollector / SurveyCollectorData"],
                ["SU501000",  "PX.Survey.Ext.SurveyProcess",            "SurveyFilter (filter) / SurveyCollector (documents)"],
            ]
        ),

        h1("Appendix B. Database Tables"),
        infoTable(
            ["Table",                 "Holds"],
            [
                ["SurveySetup",          "One row per tenant. System-wide defaults."],
                ["Survey",               "Survey definitions."],
                ["SurveyDetail",         "Ordered list of components that make up a survey page sequence."],
                ["SurveyComponent",      "Reusable HTML / Scriban fragments."],
                ["SurveyUser",           "Users/Contacts who will receive a given survey (recipients)."],
                ["SurveyCollector",      "One row per respondent per survey (the 'ballot')."],
                ["SurveyCollectorData",  "Raw payload for each page submission."],
                ["SurveyAnswer",         "Parsed answer — one row per question on a processed page."],
                ["SurveySetupEntity",    "Binds default surveys to an existing Acumatica entity (Case, Contact, …)."],
            ]
        ),
    ];
}

// ------------------------------------------------------------------
// Document assembly
// ------------------------------------------------------------------
const doc = new Document({
    creator: "Acumatica Surveys Team",
    title: "Acumatica Surveys — User Guide",
    description: "Configure, send and track surveys with the PX.Survey.Ext customization.",

    styles: {
        default: {
            document: {
                run: { font: "Arial", size: 22 },
                paragraph: { spacing: { after: 120 } },
            },
        },
        paragraphStyles: [
            { id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true,
                run:       { size: 44, bold: true, font: "Arial", color: "2E75B6" },
                paragraph: { spacing: { before: 360, after: 200 }, outlineLevel: 0 } },
            { id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true,
                run:       { size: 32, bold: true, font: "Arial", color: "1F4E79" },
                paragraph: { spacing: { before: 280, after: 160 }, outlineLevel: 1 } },
            { id: "Heading3", name: "Heading 3", basedOn: "Normal", next: "Normal", quickFormat: true,
                run:       { size: 26, bold: true, font: "Arial", color: "2E75B6" },
                paragraph: { spacing: { before: 200, after: 120 }, outlineLevel: 2 } },
        ],
    },

    numbering: {
        config: [
            { reference: "bullets", levels: [
                { level: 0, format: LevelFormat.BULLET, text: "\u2022",
                  alignment: AlignmentType.LEFT,
                  style: { paragraph: { indent: { left: 720, hanging: 360 } } } },
                { level: 1, format: LevelFormat.BULLET, text: "\u25E6",
                  alignment: AlignmentType.LEFT,
                  style: { paragraph: { indent: { left: 1440, hanging: 360 } } } },
            ]},
            { reference: "steps", levels: [
                { level: 0, format: LevelFormat.DECIMAL, text: "%1.",
                  alignment: AlignmentType.LEFT,
                  style: { paragraph: { indent: { left: 720, hanging: 360 } } } },
            ]},
        ],
    },

    sections: [{
        properties: {
            page: {
                size:   { width: PAGE_W, height: PAGE_H },
                margin: { top: MARGIN, right: MARGIN, bottom: MARGIN, left: MARGIN },
            },
        },
        headers: {
            default: new Header({ children: [new Paragraph({
                alignment: AlignmentType.RIGHT,
                children: [new TextRun({
                    text: "Acumatica Surveys — User Guide",
                    italics: true, size: 18, color: "888888",
                })],
            })] }),
        },
        footers: {
            default: new Footer({ children: [new Paragraph({
                alignment: AlignmentType.CENTER,
                children: [
                    new TextRun({ text: "Page ", size: 18, color: "888888" }),
                    new TextRun({ children: [PageNumber.CURRENT], size: 18, color: "888888" }),
                    new TextRun({ text: " of ", size: 18, color: "888888" }),
                    new TextRun({ children: [PageNumber.TOTAL_PAGES], size: 18, color: "888888" }),
                ],
            })] }),
        },
        children: [
            ...coverPage(),
            ...tocSection(),
            ...overviewChapter(),
            ...installationChapter(),
            ...configurationChapter(),
            ...usingChapter(),
            ...sendingChapter(),
            ...respondentChapter(),
            ...trackingChapter(),
            ...troubleshootingChapter(),
            ...appendixChapter(),
        ],
    }],
});

Packer.toBuffer(doc).then(buffer => {
    fs.writeFileSync(OUT, buffer);
    console.log(`Wrote ${OUT} (${buffer.length} bytes)`);
}).catch(err => {
    console.error("Error:", err);
    process.exit(1);
});
