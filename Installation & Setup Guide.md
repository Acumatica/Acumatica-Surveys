# Acumatica Surveys Installation & Setup Guide #

This is an extensive step-by-step guide for customers to successfully install & setup the customization package. We have recently improved structure of our documentation by separating documenation of information such as release notes, [README](https://github.com/Acumatica/Acumatica-Surveys/blob/master/README.md), and documentation guides.  The [README](https://github.com/Acumatica/Acumatica-Surveys/blob/master/README.md) will serve as an introduction and table of content hyper-linked to easily navigate the information the reader needs at any particular time.

The changes will enhance the readibility and utility of the documentation itself.

## Setup Instructions for Installing the Acumatica Surveys package

**Step One:** Download the customization package (AcumaticaSurveyPkg.zip) appropriate for your version of Acumatica.  Curently, we support the following versions:

**Acumatica 2019R2 (Build 19.208.0051 onward)**

**Acumatica 2020R1 (Build 20.100.0095 onward)**

To download the customization package for 2019R2, click [here](https://github.com/Acumatica/Acumatica-Surveys/tree/2019R2).  For 2020R1, click [here](https://github.com/Acumatica/Acumatica-Surveys/tree/2020R1).

<img src="/docs/images/SS1-DownloadPackage.PNG" height="50%" width="50%">

**Step Two:** Go to your Project Customization Screen. You may just do a quick searach by typing *"Customization"* then selecting it as follows.

<img src="/docs/images/SS0-CustomizationProjects.PNG" height="50%" width="50%">

**Step Three:** Upload & import the package you downloaded to your **Acumatica 2019R2** instance by clicking on the *Customization* menu which you will now find in the upper-righthand of the Acumatica application.  Then select **Manage Customizations** from the drop down list as illustrated below.

<img src="/docs/images/SS1-ImportCustPackage.PNG" height="50%" width="50%">

Next, click on the **Import** tab and select *"Import New Project"* from the dropdown list as shown below.

<img src="/docs/images/SS2-ImportNewProject.PNG" height="50%" width="50%">

Then click on **Choose File** to open up the *File Manager* to select the **Acumatic Surveys** package.

<img src="/docs/images/SS2-UploadPackage.PNG" height="50%" width="50%">

**Step Four:** After the package is uploaded, you will need to publish it.  Select the package you just uploaded by clicking on its associated *checkbox* to the left and then click on the **Publish** tab to publish your package.  Once it's published, you are ready to create your fist Survey!

<img src="/docs/images/SS3-PublishPackage.PNG" height="50%" width="50%">



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

<img src="/docs/images/SS4SurveyTransacationsSurvey2.PNG" height="50%" width="50%">

Specify the survey name and click *save* to generates a new auto sequence number. From the details tab, click **+** and add the new *attributes* as questions to this survey

<img src="/docs/images/SS5-CreateAttributesQuestions.PNG" height="50%" width="50%">

Next, specify the sort order sequence, if a question is required or mandatory for recipients to answer. 

From the **Recipients tab** click on the *“Add Recipients”* and select the employees who will participate in the survey.

<img src="/docs/images/SS5-RecipentsSelect2.PNG" height="50%" width="50%">

***Note: You need to ensure all employees are linked to the User profile and their user accounts are Sync’d with their mobile phones and that they are able to use the Acumatica Mobile application***



## Send Survey

From the “Surveys” workspace, click “Send Survey” to process and activate the survey and send *Push Notifications* to mobile phones.

<img src="/docs/images/SS5-ProcessSendSurvey.PNG" height="50%" width="50%">

Next, click on the magnifying glass icon to search for your survey and select it.

<img src="/docs/images/SS5-SendSurveySelectID2.PNG" height="50%" width="50%">

Then Send your Survey and wait for your responses.

<img src="/docs/images/SS5-SendSurveySend2.PNG" height="50%" width="50%">

***Please note: you have to sync your device by connecting before sending surveys. Only registered mobile phones will receive the push notification, others will error out.***

Those records will be activated for survey responses with status as “New” and those who receive the push notification will have records with a status of “Sent.”

Users can select specific recipients from the list or click **SEND ALL.** Selected records could be also be sent with the “SEND” option.

## Adminstration/Maintenence of Surveys (Process Survey Menu)

Now that you have created a survey and sent it out, you have additional functionality to manage & maintain your surveys.  This new functionality provides addtional *options* which we added in phase 2 our our community development efforts. You will find these new options in the **Actions** dropdown box under the Processing Survey page ( including *Send New, Remind Un-Answered, Expire Un-Answered, and ALL (New, Remind, Expire).

<img src="/docs/images/SS5-SendSurveySend2.PNG" height="50%" width="50%">

The new actions are defined as follows:

**Send New:** Process the selected Survey as a "new" survey for your recipients - even if it has been sent out previously or has been expired.
**Remind Un-Answered:** Send out a reminder to all the recipients who have not responded to a selected survey.
**Expired Un-Answered:** Expire all the "unswered" or unresponsive surveys that recipients have failed to complete for a particular survey.
**All (New, Remind, Expire):** Process all actions listed.

Note, you may *expire* a given survey automatically at a specific time by setting the **Expire After** option which you will find below the **Actions** option. You can do this down to the minute.

## Using Automation Schedules in Acumatica to Set the Schedule Frequency & Duration ##
The ability to schedule surveys are an important feature to automate when surveys are sent out and the frequency that occur. You can easily levarage the built-in Acumatica Automation Schedule engine.

To create a scheudule for a survey, enter **Schedule** in the *Search* field and select **Automation Schedules** as depicted in the screenshot below.

<img src="/docs/images/SS1-AutomationSchedules.PNG" height="50%" width="50%">

Then enter the text for the description field for the new schedule and choose the start date.

<img src="/docs/images/SS3-AutomationSchedulesStartDate.PNG" height="50%" width="50%">

Afterwards, click on the **Schedule tab** and choose the time you would like the survey to be sent as shown below.

<img src="/docs/images/SS4-AutomationSchedulesStartTime.PNG" height="50%" width="50%">

In the Schedule tab shown above, you can also set the frequency: *hourly, daily, and monthly.* You can set other parameters as well, such as whether your schedule expires and when.  No need to go into any detail here, since the interface is standard & intuitive.

Once you have set up the schedule for a particular survey and saved it, you can view and montitor your schedules and adjust as necessary.

To view the statuses of any of the automated schedules, type in *Automation* in the search field and select **Automation Schedule Statuses* as shown in the sreenshot.

<img src="/docs/images/SS5-AutomationSchedulesStatus.PNG" height="50%" width="50%">

After clicking on the menu item, you will see all the *Automation Schedules.* Notice the example schedules at end of the tabulated list above.  You can see the surveys that have executed and the one created in our example that is pending to go out on 4/20/2020 at 8am.

Use the scheduling engine as it will save you time and energy in automating sending out schedules that are sent on a periodic basis.


## Survey Response
Recipients will receive notifications and can click on a link to login to the Acumatica mobile application on Android. Make sure that user install the latest version of the iOS mobile client as there was a problem wrapping survey question text that has since been resolved. The current version as of 13 May 2020 is 18.200.35 (1266) corrected the problem.

Upon clicking the survey, the user will need to click on **View Questions** and review all their survey questions and answer them. After answering, will select the back button **←**. 

For each employee user of the survey that you created, they should have a **Surveys** icon as shown in the screenshot below.

<img src="/docs/images/MobileSS-1.jpeg" height="25%" width="25%"> 

The user may need to scroll down to find the icon or they can simply click the ellipse **[...]** - the *three dots* or the three stacked lines to access the menu option depending on the mobile operating system. 

<img src="/docs/images/MobileSS-2.jpeg" height="25%" width="25%">

After the user selects the **Surveys** application icon, they will see a screen similar to this one below.

<img src="/docs/images/MobileSS-3.png" height="25%" width="25%">

Each survey has a status, indicating a surveys current status and whether any action is required of the user.  When a user has completed a specific survey in thier list, the status will change from *“Awaiting Response”* to *“Responded”.*  In the screenshot above, you can see the user has *responded* to all of his surveys, except the one at the bottom of the screen.

For the user to start answering questions, they will need to select the survey with the status, *Awaiting Response*. Then subsequently select, *View Questions*.

<img src="/docs/images/MobileSS-0.jpeg" height="25%" width="25%">

The survey questions they need to answer & complete in our example are listed as follows.

<img src="/docs/images/MobileSS-4.png" height="25%" width="25%"> 

In the case of selecting the question asking them about temperature, we constrain the choices using radio buttons in our example. Of course, you can choice a drop-down list or allow them to enter the value freely.

<img src="/docs/images/MobileSS-5.png" height="25%" width="25%"> 

After answering all the required questions of the survey, the user will need to send it. Simply completing each of the questions does not send the survey. They complete the process by simply clicking on the **"paper airplane" icon** as shown below.

<img src="/docs/images/MobileSS-6.png" height="25%" width="25%">

Once sent, they are done until the next survey arrives.

## Suvey Dashboards ##
What's the point of a survey without metrics to meaure?  So yes, we built a beautiful dashboard which you can of course customize yourself to suit you needs.

In Acumatica on the web, we have a default dashboard as shown below.

<img src="/docs/images/DashboardSS-1.PNG" height="50%" width="50%">

and in the mobile application it renders as follows.

<img src="/docs/images/MobileDashboardSS-1.jpg" height="25%" width="25%">

So yes, some eye-candy for you visual and analytical folks out there.

## Using Business Events (draft section)

A business event is a data change or a set of conditions checked for on a schedule. 

It may include the following information:

- The general information about the business event (such as its name and type)
- The trigger conditions of the business event
- The schedule of the business event (if the conditions of the business event are checked for on a schedule)
- The generic inquiry parameters (if any parameter values have been specified for the business event)
- The email notification templates (if the business event has email notification templates as subscribers)

For our example scenario, we are interested in creating a *Business Event* & *email Notification* that is sent to the survey respondent's manager when they their employee reports symptoms such as a temperature that is not normal.

First, before creating the *Business Event*, we need to create a **Generic Inquiry** which returns a *Results Grid* with the survey data records with the "COVTEMP_Attributes" field/attribute that subsequently is used to "trigger" the business event when the condition we set is met.

To navigate to the *Generic Inquiry* screen, just type in *Generic* in **Search** and click on *Generic Inquiry* under Customization **Profiles** as illustrated below.

<img src="/docs/images/SS1-GITempMonitor0.PNG" height="50%" width="50%">

Next type in the name of your generic inquiry (TempMonitor) and click on the *checkbox* below the name you just entered to make it visible to the UI.

<img src="/docs/images/SS1-GITempMonitor1.PNG" height="50%" width="50%">

Then add the SurveyCollector Table.  You can search for it by typing "Survey" and then select it to include it in your generic inquiry.


<img src="/docs/images/SS1-GITempMonitor2.PNG" height="50%" width="50%">

After selecting the table, it will display as follows.

<img src="/docs/images/SS1-GITempMonitor3.PNG" height="50%" width="50%">

Next save your GI and then select the Results Grid tab to make sure it returns the data you will be needing.  As you can see from the screenshot below that the Temperature field we need is present, **COVTEMP_Attributes** which will be used to trigger our Business Event.

<img src="/docs/images/SS1-GITempMonitor4.PNG" height="50%" width="50%">

Now it's time to create our Business Event.

To configure the Acumatica to use a *business event process* to trigger an email notification, navigate to the **Business Events** form by typing Business Events in the search field.  

<img src="/docs/images/SS1-BusinessEventsSearch.PNG" height="50%" width="50%">

You can define a business event that relates to this business event process - in our case, Acumtica Surveys related processes - that instructs the system to perform an action or multiple actions in Acumatica itself. 

After navigating to the **Business Events**, form, click the "+" sign to create a new business event.

<img src="/docs/images/SS2-BusinessEventsNew.PNG" height="50%" width="50%">

You can create a business event that is triggered by a change to a specific *record change* for *survey responses* as an example. One that notifies the the company that one of thier employees has specific symptoms, such as a high temperature.

<img src="/docs/images/SS2-BusinessEventsNew2.PNG" height="50%" width="50%">

We will use our Generic Inquiry we created earlier for our *Event ID*. Just type in the the first part of the name of the GI in *Event ID* field search, *"Temp"* and select it.

<img src="/docs/images/SS2-BusinessEventsNew0.PNG" height="50%" width="50%">

You can see that the Screen Name & Screen ID were filled in for you and we are ready to add our trigger that fires our business event.  

<img src="/docs/images/SS2-BusinessEventsNew3.PNG" height="50%" width="50%">

Now we can build our trigger conditions that fire the event when survey respondent records a temperature is "not normal".


To do this, you simply choose the values for *Operation*, *Table Name*, *Field Name*, *Condition* as displayed below and save it.

<img src="/docs/images/SS2-BusinessEventsNew4.PNG" height="50%" width="50%">

You can now add a notification to the Business Event, such as an eMail Notification, Mobile Push, and Mobile SMS subcribers. This can be done by clicking on the Subscriber tab as shown below.

<img src="/docs/images/SS2-BusinessEventsNewSubscriber.PNG" height="50%" width="50%">

For more information on *Business Events*, click on the following links:

**[Business Events Help Topic](https://help-2019r2.acumatica.com/(W(1))/Help?ScreenId=ShowWiki&pageid=920e13d8-387c-404f-8b33-c200ac66df98)**

**[Mobile Notifications and Business Events Video](https://youtu.be/lCQhZwcOays)**
