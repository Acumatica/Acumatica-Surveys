# Acumatica Surveys README

## Setup Instructions for Installing the Acumatica Surveys package

**Step One:** Download the customization package (AcumaticaSurveyPkg.zip) as is shown below in the screenshot below.

![Screenshot](/docs/images/SS1-DownloadPackage.PNG)

**Step Two:** Go to your Project Customization Screen. You may just do a quick searach by typing *"Customization"* then selecting it as follows.

![Screenshot](/docs/images/SS0-CustomizationProjects.PNG)

**Step Three:** Upload & import the package you downloaded to your **Acumatica 2019R2** instance by clicking on the *Customization* menu which you will now find in the upper-righthand of the Acumatica application.  Then select **Manage Customizations** from the drop down list as illustrated below.

![Screenshot](/docs/images/SS1-ImportCustPackage.PNG)

Next, click on the **Import** tab and select *"Import New Project"* from the dropdown list as shown below.

![Screenshot](/docs/images/SS2-ImportNewProject.PNG)

Then click on **Choose File** to open up the *File Manager* to select the **Acumatic Surveys** package.

![Screenshot](/docs/images/SS2-UploadPackage.PNG)

**Step Four:** After the package is uploaded, you will need to publish it.  Select the package you just uploaded by clicking on its associated *checkbox* to the left and then click on the **Publish** tab to publish your package.  Once it's published, you are ready to create your fist Survey!

![Screenshot](/docs/images/SS3-PublishPackage.PNG)



## Number Sequence Setup for your Surveys

Once the package is installed, you should see a new workspace called *“Surveys”* as shown here below.

![Screenshot](/docs/images/Survey-Workspace.PNG)

Click on the *Survey Preferences menu* option.

**Survey preferences** - Surveys require a numbering sequence. Therefore, please set up a new number sequence called *“Survey ID”* and an *auto generating number,* such as: **SV10001**

![Screenshot](/docs/images/SS4-SurveyPreferencesNumberingID.PNG)

Select the numbering sequence you created in the **Survey Preferences** *“Survey Numbering ID:”*
Surveys utilizes common *attributes* to create questions and answers. Therefore, please setup any new questions/answers as *attributes.*

After you have setup required attributes, you are now ready to create your first survey!



## Create a Survey

Click on Surveys in the workspace and click *Survey* under **Transactions.** 

![Screenshot](/docs/images/SS4SurveyTransacationsSurvey.PNG)

The survey primary list screen **(SV2010PL)** loads with empty records. Click on the **+** to create a new survey which opens the new screen **SV201000**

![Screenshot](/docs/images/SS4SurveyTransacationsSurvey2.PNG)

Specify the survey name and click *save* to generates a new auto sequence number. From the details tab, click **+** and add the new *attributes* as questions to this survey

![Screenshot](/docs/images/SS5-CreateAttributesQuestions.PNG)

Next, specify the sort order sequence, if a question is required or mandatory for recipients to answer. 

From the **Recipients tab** click on the *“Add Recipients”* and select the employees who will participate in the survey.

![Screenshot](/docs/images/SS5-RecipentsSelect.PNG)

***Note: You need to ensure all employees are linked to the User profile and their user accounts are Sync’d with their mobile phones and that they are able to use the Acumatica Mobile application***



## Send Survey
From the “Surveys” workspace, click “Send Survey” to process and activate the survey and send *Push Notifications* to mobile phones.

![Screenshot](/docs/images/SS5-ProcessSendSurvey.PNG)

Next, click on the magnifying glass icon to search for your survey and select it.

![Screenshot](/docs/images/SS5-SendSurveySelectID.PNG)

Then Send your Survey and wait for your responses.

![Screenshot](/docs/images/SS5-SendSurveySend.PNG)

***Please note: you have to sync your device by connecting before sending surveys. Only registered mobile phones will receive the push notification, others will error out.***

Those records will be activated for survey responses with status as “New” and those who receive the push notification will have records with a status of “Sent.”

Users can select specific recipients from the list or click **SEND ALL.** Selected records could be also be sent with the “SEND” option.



## Survey Response
All recipients will receive notifications and can click on a link to login to the Acumatica mobile application on Android or iPhone/iOS and complete the survey. However, there are limitations with iOS at this time.  See the section below on **Known Limitations and Contraints** in the next section of this README.

Upon clicking the survey, you will need to click on **“QUESTIONS”** and review all the survey questions and answer them. After answering, click The back button **←**. 

For user recipients of the survey that you create, they should have a **Surveys** icon as shown in the screenshot below.

![Screenshot](/docs/images/MobileSS-1.jpeg)

The user may need to scroll down to find the icon or they can simply click the ellipse **[...]** - the *three dots* or the three stacked linesto access the menu option and click **“SUBMIT”** to send the survey responses back to the system. By submitting the sur as you can see in the next screenshot.

![Screenshot](/docs/images/MobileSS-2.jpeg)

After clicking on Surveys, the user will see a screen similar to this one below will a list of the surveys they have to respond to or have had already responded.

Once they complete the survey and send it, the status will change from *“Sent”* to *“Responded”.* 

![Screenshot](/docs/images/MobileSS-3.jpeg)

Once the user clicks on a particular survey, they can view the questions and respond accordingly.

![Screenshot](/docs/images/MobileSS-4.png)

And here you can see a question as a "multiple-choice" response via radio buttons on screen.

![Screenshot](/docs/images/MobileSS-5.png)

## Suvey Dashboards ##
What's the point of a survey without metrics to meaure?  So yes, we built a beautiful dashboard which you can of course customize yourself to suit you needs.

In Acumatica on the web, we have a default dashboard as shown below.

![Screenshot](/docs/images/DashboardSS-1.PNG)

and in the mobile application it renders as follows.

![Screenshot](/docs/images/MobileDashboardSS-1.jpg)

So yes, some eye-candy for you visual and analytical folks out there.

## Known Limitations and Constraints
The ability to delete the survey when it’s still active, but has not been processed/published.

A published survey that a user has responded cannot be deleted.
or
The ability to clone or copy a survey from an existing survey in the system.  

What gets copied? Survey questions, recipients? Thus far, we have only tested the employees list.

Please note that there is an issue with iOS devices with respect to attributes.  We are working on resolving the problem and will updating our notes to reflect this in the next several days.  For now, iOS users can use the web to participate in any survey they are selected as recipients.

## More Information
To see a demonstration of *Acumatica Surveys*, click on the following link: https://youtu.be/RV7jsTgsVNE.
