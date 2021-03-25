using PX.Common;

namespace PX.Survey.Ext {
    [PXLocalizable(Prefix)]
    public static class Messages {
        public const string Prefix = "Acumatica Survey";

        #region CacheNames
        public class CacheNames {
            public const string SurveyCollector = "Survey Collector";
            public const string SurveyCollectorData = "Survey Collector Data";
            public const string SurveySetup = "Survey Setup";
            public const string Survey = "Survey";
            public const string SurveyUser = "Survey User";
            public const string SurveyFilter = "Survey Filter";
        }
        #endregion

        public class CollectorStatus {
            public const string New = "New";
            public const string Rendered = "Rendered";
            public const string Sent = "Awaiting Response";
            public const string Responded = "Responded";
            public const string Reminded = "Reminded";
            public const string Expired = "Expired";
            public const string Error = "Error";
        }

        public class CollectorDataStatus {
            public const string New = "New";
            public const string Connected = "Connected";
            public const string Processed = "Processed";
            public const string Error = "Error";
        }

        public const string Send = "Process";
        public const string SendAll = "Process All";

        public const string PushNotificationTitleSurvey = "Complete Survey";
        public const string PushNotificationMessageBodySurvey = "Tap to complete Survey";
        public const string NoDeviceError = "User has not setup Acumatica mobile app.";

        public const string SurveySent = "Survey has been sent";
        public const string SurveyError = "At least one Survey hasn't been processed.";

        public const string Question = "Question";
        public const string Answer = "Answer";
        public const string Submit = "Submit";
        public const string ReOpen = "Re-open";

        public const string AnswerReqiredQuestions = "Answers should be specified for all required questions.";
        public const string TemplateNeeded = "You need to create a template to render a survey";

        public const string PushNotificationMessageBodySurveyIOS = "You have new Survey # {0} to complete";
        public const string SurveyActionNotRecognised = "Survey Action Not Recognised";
        public const string SurveyReminderSent = "Survey Reminder Sent";
        public const string SurveyReminderFailed = "Survey Reminder Failed";
        public const string SettingTheExpirationForUserID_0_Failed = "Setting the expiration for userID:{0} failed";
        public const string SetExpirationSuccess = "Set Expiration Success";
        public const string SetExpirationFailed = "Set Expiration Failed";
        public const string AnErrorOccuredTryingToResendANotificationForUserID_0 =
            "An Error Occured Trying to resend a notification for UserID:{0}";

        public const string NoteIDNotFound = "No NoteID on object of type '{0}'";
        public const string CollectorNotFound = "Cannot find a collector with ID {0}";
        public const string AnswersNotfound = "No answers found";
        public const string SurveyQuestionNotFound = "A Survey Question with AttributeID '{0}' does not exist";


        #region Demo Survey
        public const string COVSYMPTOM = "Are you experiencing any of these symptoms? (fever, cough, shortness of breath, sore throat or diarrhea)";
        public const string COVCONTACT = "Contact with individuals diagnosed with COVID-19?";
        public const string COVTEMP = "Self Temperature";
        public const string COVTRAVEL = "Travel Locations";
        public const string DEMOCOVID = "COVID-19 Wellness Survey";
        #endregion


        #region SurveyAction
        public class SurveyAction {
            public const string Default = "All (New, Remind, Expire)";
            public const string RenderOnly = "Render";
            public const string NewOnly = "Send New";
            public const string RemindOnly = "Remind Un-Answered";
            public const string ExpireOnly = "Expire Un-Answered";
            public const string ProcessAnswers = "Process Answers";
        }
        #endregion


    }
}