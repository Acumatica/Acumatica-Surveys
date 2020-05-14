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

        public const string Send = "Send";
        public const string SendAll = "Send All";

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

        #region Demo Survey
        public const string COVSYMPTOM = "Are you experiencing any of these symptoms? (fever, cough, shortness of breath, sore throat or diarrhea)";
        public const string COVCONTACT = "Contact with individuals diagnosed with COVID-19?";
        public const string COVTEMP = "Self Temperature";
        public const string COVTRAVEL = "Travel Locations";
        public const string DEMOCOVID = "COVID-19 Wellness Survey";
        #endregion
    }
}