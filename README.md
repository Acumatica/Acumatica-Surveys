# Acumatica Surveys (PX.Survey.Ext)

An Acumatica ERP customization that adds a full **survey / questionnaire engine** to your instance. Originally built as a COVID-19 employee wellness tracker, it has grown into a general-purpose tool for sending surveys to employees, customers, or contacts and collecting their responses back inside Acumatica.

Watch the demo: <https://youtu.be/RV7jsTgsVNE>
Background story: [Acumatica Surveys: A COVID-19 Wellness Tracker of Employees](https://www.acumatica.com/blog/acumatica-surveys-covid-19-wellness-tracker/)

---

## What you get

After installing the customization package, a new **Surveys** workspace appears in Acumatica:

<img src="docs/images/Survey-Workspace.PNG" width="60%">

It ships with:

- **Survey Preferences** (`SU101000`) — numbering sequences and global settings.
- **Survey Components** (`SU204003`) — reusable HTML/XML widgets (welcome page, radio buttons, checkboxes, progress bar, text area, thank-you page, etc.). A library of pre-built components is included in [Components/](Components/).
- **Surveys** (`SU201000`) — build a survey by composing components and defining questions.
- **Survey Collectors** (`SU301000`) — individual collectors (one per recipient) with their responses.
- **Process / Send Survey** (`SU501000`) — generate and mass-send collectors to recipients.
- **Webhook endpoint** — respondents answer from a browser (desktop or mobile) without having to log in; answers flow back into Acumatica through [SurveyWebHookServer.cs](PX.Survey.Ext/WebHook/SurveyWebHookServer.cs).
- **Graph extensions** ([GraphExt/](PX.Survey.Ext/GraphExt/)) — attach surveys to existing Acumatica entities (e.g. Cases).
- **Dashboards** for reviewing results.

---

## Supported versions

| Acumatica version | Branch |
|---|---|
| 2019 R2 (19.208.0051+) | [`2019R2`](https://github.com/Acumatica/Acumatica-Surveys/tree/2019R2) |
| 2020 R1 (20.100.0095+) | [`2020R1`](https://github.com/Acumatica/Acumatica-Surveys/tree/2020R1) |
| 2021 R2 | [`2021R2_Phase3`](https://github.com/Acumatica/Acumatica-Surveys/tree/2021R2_Phase3) |
| 2026 R1 | `26R1` (this branch) |

Ready-to-install packages on this branch: `Survey26R1.zip`, `Survey26R1.Base.zip`, `Survey26R1.SalesDemo.zip`.

---

## Installation (quick start)

The full step-by-step guide with every screenshot lives in **[Installation & Setup Guide.md](Installation%20&%20Setup%20Guide.md)**. In short:

### 1. Download the package

Pick the `.zip` that matches your Acumatica version from this repository.

<img src="docs/images/SS1-DownloadPackage.PNG" width="60%">

### 2. Open Customization Projects

In Acumatica, search for *Customization* and open **Customization Projects**.

<img src="docs/images/SS0-CustomizationProjects.PNG" width="60%">

### 3. Import the package

**Customization → Manage Customizations → Import → Import New Project**, then choose the `.zip` you downloaded.

<img src="docs/images/SS1-ImportCustPackage.PNG" width="60%">
<img src="docs/images/SS2-ImportNewProject.PNG" width="60%">
<img src="docs/images/SS2-UploadPackage.PNG" width="60%">

### 4. Publish

Tick the imported project and click **Publish**.

<img src="docs/images/SS3-PublishPackage.PNG" width="60%">

---

## Configure

### Survey Preferences

Open **Survey Preferences** and set up the numbering sequence (e.g. `SV10001`).

<img src="docs/images/S001 - Survey Preferences.PNG" width="60%">
<img src="docs/images/SS4-SurveyPreferencesNumberingID.PNG" width="60%">

### Survey Components

Browse or edit the reusable pieces that make up a survey page (welcome page, questions, progress bar, thank-you page…).

<img src="docs/images/SS02 - Surveys Components.PNG" width="60%">
<img src="docs/images/SS03 - Surveys Components.PNG" width="60%">
<img src="docs/images/SS04 - Surveys Components.PNG" width="60%">

### Attributes / questions

Questions are stored as Acumatica Attributes, so they can be reused across surveys and entities.

<img src="docs/images/SS5-CreateAttributesQuestions.PNG" width="60%">
<img src="docs/images/SS5-CreateAttributesQuestions2.PNG" width="60%">

---

## Creating and sending a survey

### 1. Create a survey

Open **Surveys** (`SU201000`) and compose one by picking components and questions. The Survey Preferences screen also exposes a **Create Demo Survey** button as a quick way to generate a sample.

<img src="docs/images/DemoSurveyCreate-S002.PNG" width="60%">
<img src="docs/images/DemoSurveyCreate-S003.PNG" width="60%">

### 2. Pick recipients

<img src="docs/images/SS5-RecipentsSelect.PNG" width="60%">
<img src="docs/images/SS5-RecipentsSelect2.PNG" width="60%">

### 3. Process & send

Go to **Process Surveys**, choose the survey ID, select recipients and click **Send**.

<img src="docs/images/Process-Survey1.PNG" width="60%">
<img src="docs/images/ProcessSurveyActionDropDown-SS02.PNG" width="60%">
<img src="docs/images/SS5-SendSurveySelectID.PNG" width="60%">
<img src="docs/images/SS5-SendSurveySend.PNG" width="60%">

---

## Answering a survey (recipient view)

Recipients receive a link and answer from any browser — no Acumatica login required. The same form works great on mobile:

<p>
<img src="docs/images/MobileSS-0.jpeg" width="22%">
<img src="docs/images/MobileSS-1.jpeg" width="22%">
<img src="docs/images/MobileSS-2.jpeg" width="22%">
<img src="docs/images/MobileSS-3.jpeg" width="22%">
</p>
<p>
<img src="docs/images/MobileSS-4.png" width="22%">
<img src="docs/images/MobileSS-5.png" width="22%">
<img src="docs/images/MobileSS-6.png" width="22%">
</p>

---

## Reviewing responses

Individual collector (one recipient’s submission):

<img src="docs/images/Survey-Response-View.PNG" width="60%">

Dashboards aggregate the answers:

<img src="docs/images/Survey-View-Dashboard.PNG" width="60%">
<img src="docs/images/DashboardSS-1.PNG" width="60%">
<img src="docs/images/MobileDashboardSS-1.jpg" width="30%">

---

## Automating delivery

You can schedule recurring sends using Acumatica’s **Automation Schedules** and **Business Events**:

<img src="docs/images/SS1-AutomationSchedules.PNG" width="60%">
<img src="docs/images/SS2-AutomationSchedulesNew.PNG" width="60%">
<img src="docs/images/SS3-AutomationSchedulesStartDate.PNG" width="60%">
<img src="docs/images/SS4-AutomationSchedulesStartTime.PNG" width="60%">
<img src="docs/images/SS5-AutomationSchedulesStatus.PNG" width="60%">

Business Events can trigger surveys from anything happening in Acumatica (e.g. a Case closed → send a satisfaction survey):

<img src="docs/images/SS1-BusinessEventsSearch.PNG" width="60%">
<img src="docs/images/SS2-BusinessEventsNew.PNG" width="60%">
<img src="docs/images/SS3-BusinessEventsAddTriggers.PNG" width="60%">
<img src="docs/images/SS2-BusinessEventsNewSubscriber.PNG" width="60%">

---

## Source layout

```
PX.Survey.Ext/
├── DAC/             # Data Access Classes (Survey, SurveyCollector, SurveyComponent, SurveyAnswer, …)
├── DACExt/          # DAC extensions for stock Acumatica entities
├── GraphExt/        # Graph extensions — plug surveys into Cases etc.
├── Attributes/      # Custom PX attributes (e.g. SelectorRecord)
├── Descriptor/      # Constants, enums, helpers (SurveyUtils)
├── Formula/         # Formula fields
├── WebHook/         # SurveyWebHookServer — public endpoint for respondents
├── SurveyMaint.cs              # Survey editing graph
├── SurveyComponentMaint.cs     # Component editing graph
├── SurveyCollectorMaint.cs     # Response graph
├── SurveySetupMaint.cs         # Preferences graph
├── SurveyProcess.cs            # Mass-send processing
├── SurveyGenerator.cs          # Builds the HTML page from components
└── SurveyDataLoader.cs         # Default data seeding

Pages/                 # ASPX screens:
                       #   SU101000 — Survey Preferences      (SurveySetupMaint)
                       #   SU201000 — Survey                  (SurveyMaint)
                       #   SU204003 — Survey Components       (SurveyComponentMaint)
                       #   SU301000 — Survey Collector        (SurveyCollectorMaint)
                       #   SU501000 — Process / Send Survey   (SurveyProcess)
Components/            # Prebuilt survey components as XML
Customizations/        # Packaged demo bundle
docs/images/           # All screenshots referenced in this README
```

---

## Support

- Questions: post on [Stack Overflow with the `acumatica` tag](https://stackoverflow.com/questions/tagged/acumatica).
- Bugs / feature requests: open an issue in this repo — see [Known Issues](https://github.com/Acumatica/Acumatica-Surveys/issues).
- Full install/setup walkthrough: **[Installation & Setup Guide.md](Installation%20&%20Setup%20Guide.md)**.
- Phase 3 feature notes: [Phase3Beta.md](Phase3Beta.md).
