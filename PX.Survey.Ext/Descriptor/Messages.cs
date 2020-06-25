using PX.Common;

namespace PX.Survey.Ext
{
    [PXLocalizable(Prefix)]
    public static class Messages
    {
        public const string Prefix = "Acumatica Survey";

        #region CacheNames
        public const string SurveyCollectorCacheName = "Survey Collector";
        public const string SurveySetupCacheName = "Survey Setup";
        public const string SurveyCacheName = "Survey";
        public const string SurveyUserCacheName = "Survey User";
        public const string SurveyFilterCacheName = "Survey Filter";
        #endregion

        #region Survey Response Status

        public const string CollectorNew = "New";
        public const string CollectorSent = "Awaiting Response";
        public const string CollectorResponded = "Responded";
        public const string CollectorExpired = "Expired";

        #endregion

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

        //Until issue is resolved with ios app
        public const string PushNotificationMessageBodySurveyIOS = "You have new Survey # {0} to complete";
        public const string SurveyActionNotRecognised = "Survey Action Not Recognised";
        public const string SurveyReminderSent = "Survey Reminder Sent";
        public const string SurveyReminderFailed = "Survey Reminder Failed";
        public const string SettingTheExpirationForUserID_0_Failed = "Setting the expiration for userID:{0} failed";
        public const string SetExpirationSuccess = "Set Expiration Success";
        public const string SetExpirationFailed = "Set Expiration Failed";


        #region Survey Action

        public const string SurveyActionDefault = "All (New, Remind, Expire)";
        public const string SurveyActionNewOnly = "Send New";
        public const string SurveyActionRemindOnly = "Remind Un-Answered";
        public const string SurveyActionExpireOnly = "Expire Un-Answered";
        public const string AnErrorOccuredTryingToResendANotificationForUserID_0 =
            "An Error Occured Trying to resend a notification for UserID:{0}";

        #endregion

    }
}