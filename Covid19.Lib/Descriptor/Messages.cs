using PX.Common;

namespace Covid19.Lib
{
    [PXLocalizable(Prefix)]
    public static class Messages
    {
        public const string Prefix = "Acumatica Survey";

        #region
        public const string SurveyCollectorCacheName = "Survey Collector";
        #endregion

        #region Survey Response Status

        public const string CollectorNew = "New";
        public const string CollectorSent = "Sent";
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
        public const string surveyInvalidForSent = "Invalid Survey Status.";
    }
}