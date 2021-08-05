# Acumatica Surveys Phase 3 (Beta) README #

*August 3rd 2021*

Please note that this release is in large part a "re-architecture" of the Surveys project as we have expanded the functionality greatly, requiring rewriting a large portion of code.

## Setup Instructions for Installing the Phase 3 Acumatica Surveys package

**Step One:** Download the customization package (AcumaticaSurveyPkg.zip) appropriate for your version of Acumatica.  Curently, we support the following versions:

**Acumatica 2020 R2 Build 20.212.0038 onward**

To download the customization package for 2020R2, click [here](https://github.com/Acumatica/Acumatica-Surveys/blob/Stephane2020R2/Customizations/AcumaticaSurvey20R2.v.3.0.9.zip). *Needs updating.

<img src="/docs/images/SS01 - Surveys Install Phase3 Customization.PNG" height="75%" width="75%">

**Step Two:** Go to your Project Customization Screen. You may just do a quick searach by typing *"Customization"* then selecting it as follows.

<img src="/docs/images/SS0-CustomizationProjects.PNG" height="75%" width="75%">

**Step Three:** Upload & import the package you downloaded to your **Acumatica 2019R2** instance by clicking on the *Customization* menu which you will now find in the upper-righthand of the Acumatica application.  Then select **Manage Customizations** from the drop down list as illustrated below.

<img src="/docs/images/SS1-ImportCustPackage.PNG" height="100%" width="100%">

Next, click on the **Import** tab and select *"Import New Project"* from the dropdown list as shown below.

<img src="/docs/images/SS2-ImportNewProject.PNG" height="100%" width="100%">

Then click on **Choose File** to open up the *File Manager* to select the **Acumatic Surveys** package.

<img src="/docs/images/SS2-UploadPackage.PNG" height="100%" width="100%">

**Step Four:** After the package is uploaded, you will need to publish it.  Select the package you just uploaded by clicking on its associated *checkbox* to the left and then click on the **Publish** tab to publish your package.  Once it's published, you are ready to create your fist Survey!

<img src="docs/images/SS03 - Surveys Install Phase3 Customization.PNG" height="100%" width="100%">

After the validation process has successfully completed, click on the "Publish" button in the Compilation window:

<img src="docs/images/SS02 - Surveys Install Phase3 Customization.PNG" height="100%" width="100%">

If successful, you will see that the website was updated and you can continue.

<img src="docs/images/SS04 - Surveys Install Phase3 Customization.PNG" height="100%" width="100%">

At this point you will see that Surveys now appears at the bottom of the left pane.  Click on it and you will see all the elements for mananging and creating new Surveys.

<img src="docs/images/SS05 - Surveys Install Phase3 Customization.PNG" height="100%" width="100%">

You should see the following items as seen in the screenshot above:

   **Surveys** *(Transactions)*

   **Survey Collectors** *(Transactions)*

   **Process Surveys** *(Processes)*

   **Survey Response View** *(Inquiries)*

   **Survey Answers** *(Inquiries)*

   **Survey Preferences** *(Preferences)*

   **Survey Compnents** *(Preferences)*
   
   **Survey View** *(Dashboards)*

We'll discuss all these items later in the documentation as appropriate.

## Importing/Copying the Base Survey Components

*If you have installed previous versions of Acumatica Surveys, this is a new step for you in setting up Surveys. These components are the initial building blocks you will need to use and create Acumatica Surveys.  After installing these, you will be able to make copies and alter them as you please, futher customizing any surveys you choose to create.  This can be done either yourself, or by someone else in or outside of your organization.  These components provide nearly limitless flexibility for you to control the "look and feel" - along with the functionality beyond a simple survey.  More on this later.*

Assuming you were successfully published the Acumatica Surveys package, click on Surveys in the left pane and then click on Survey Components.  You will notice you don't have any components at this point.

<img src="docs/images/SS02 - Surveys Components.PNG" height="100%" width="100%">

To install the base components, navigate to the Components folder [here](https://github.com/Acumatica/Acumatica-Surveys/blob/Stephane2020R2/Components).

<img src="docs/images/SS01 - Surveys Components.PNG" height="100%" width="100%">



## Number Sequence Setup for your Surveys

Once the package is installed, you should see a new workspace called *“Surveys”* as shown here below.

<img src="/docs/images/Survey-Workspace.PNG" height="50%" width="50%">

Click on the *Survey Preferences menu* option.

**Survey preferences** - Surveys require a numbering sequence. Therefore, please set up a new number sequence called *“Survey ID”* and an *auto generating number,* such as: **SV10001**

<img src="/docs/images/SS4-SurveyPreferencesNumberingID.PNG" height="50%" width="50%">

Select the numbering sequence you created in the **Survey Preferences** *“Survey Numbering ID:”*
Surveys utilizes common *attributes* to create questions and answers. Therefore, please setup any new questions/answers as *attributes.*

After you have setup required attributes, you are now ready to create your first survey!



## Create a Survey

Click on Surveys in the workspace and click *Survey* under **Transactions.** 

<img src="/docs/images/SS4SurveyTransacationsSurvey.PNG" height="50%" width="50%">

The survey primary list screen **(SV2010PL)** loads with empty records. Click on the **+** to create a new survey which opens the new screen **SV201000**
