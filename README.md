# Acumatica Surveys README

## Setup Instructions
Upload the Acumatica Survey Wellness package to your **Acumatica 2019R2** instance 
Select and publish the package on your instance.

### Wellness Package Instructions

### Create Survey
Once the package is installed you should see a new workspace called *“Surveys”*
Click on the *Survey Preferences menu* option
**Survey preferences** - Surveys require a numbering sequence. Therefore, please set up a new number sequence called *“Survey ID”* and an *auto generating number*. Example: **SV10001**
Select the numbering sequence you created in the **Survey Preferences** *“Survey Numbering ID:”*
Surveys utilizes common *attributes* to create questions and answers, please setup new question/answers as *attributes*
After you have setup required attributes, you are now we are ready to create our first survey!
Click on Surveys workspace and click *Survey* under **Transactions** 
The survey primary list screen **(SV2010PL)** loads with empty records, click on **+** to create a new survey which opens the new screen **SV201000**
Specify the survey name and click *save* to generates a new auto sequence number.
From the details tab click **+** and add the new *attributes* as questions to this survey
Specify the sort order sequence and if the question is required/mandatory for recipients to answer. 
From the **Recipients tab** click on the *“Add Recipients”* and select the employees who will participate in the survey.

**Note: You need to ensure all employees are linked to the User profile and their user account is Sync’d with their mobile phones and they are able to use the mobile application**

### Process Survey
From the workspace “Surveys” click “Process Survey” to process/activate the survey and send the push notification to mobile phones 
Please note you have to sync your device by connecting before sending surveys, only registered mobile phones will receive the push notification others will error out.
Those records will be activated for survey response with status as “New” and those who receive the push notification those records will have status of “Sent”
Users can select specific recipients from the list or click SEND ALL, selected records could be also sent with “SEND” option

### Survey Response
All recipients will receive notifications and can click on login to Acumatica mobile app on Android or iPhone/iOS and complete the survey 
Upon clicking the survey you will have to click “QUESTIONS” and review all the survey questions and answer them
Upon answering click back button ← and on the survey page on mobile you will have to click [.] three dots to access the menu option and click “SUBMIT” to submit the survey 
By submitting the survey the status changes from “Sent” to “Responded” 

## Known Limitations and Constraints
Ability to delete the Survey when it’s still active but not processed/published
Survey when published or user has responded can’t be deleted 
Ability to copy the survey from an old survey, what gets copied? Survey questions, recipients?
We have tested only employees list
Testing to be done on iOS device
