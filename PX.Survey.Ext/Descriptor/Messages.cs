using PX.Common;

namespace PX.Survey.Ext {
    [PXLocalizable(Prefix)]
    public static class Messages {
        public const string Prefix = "Acumatica Survey";
        public const string ContactNotSetup = "Sample Contact is not entered in " + SUSetup;
        public const string SUSetup = "Survey Preferences";
        public const string GraphCannotBefound = "'{0}' with a value of '{1}' cannot be found in the system.";

        #region CacheNames
        public class CacheNames {
            public const string SurveyCollector = "Survey Collector";
            public const string SurveyCollectorData = "Survey Collector Data";
            public const string SurveySetup = "Survey Setup";
            public const string SurveySetupEntity = "Survey Entity Setup";
            public const string Survey = "Survey";
            public const string SurveyUser = "Survey User";
            public const string SurveyFilter = "Survey Filter";
        }
        #endregion

        public class Command {
            public const string SetupPasteLine = "Setup Paste Line";
            public const string SetupResetOrder = "Setup Reset Order";
        }

        public class CollectorStatus {
            public const string New = "New";
            public const string Sent = "Awaiting Response";
            public const string Partially = "Partially";
            public const string Completed = "Completed";
            public const string Reminded = "Reminded";
            public const string Expired = "Expired";
            public const string Error = "Error";
            public const string Processed = "Processed";
            public const string Deleted = "Deleted";
            public const string Incomplete = "Incomplete";
        }

        public class CollectorDataStatus {
            public const string New = "New";
            public const string Updated = "Updated";
            public const string Ignored = "Ignored";
            public const string Processed = "Processed";
            public const string Error = "Error";
        }

        public class SurveyTarget {
            public const string User = "User";
            public const string Contact = "Contact";
            public const string Anonymous = "Anonymous";
            public const string Entity = "Entity";
        }

        public class SUComponentType {
            public const string Survey = "Survey";
            public const string Header = "Header";
            public const string PageHeader = "Page Header";
            public const string QuestionPage = "Question";
            public const string CommentPage = "Comment";
            public const string ContentPage = "Content";
            public const string PageFooter = "Page Footer";
            public const string Footer = "Footer";
            public const string BadRequest = "Bad Request";
        }

        public class SurveyLayout {
            public const string SinglePage = "Single Page";
            public const string MultiPage = "Multi Page";
        }

        public class SurveyStatus {
            public const string Preparing = "Preparing";
            public const string Started = "Started";
            public const string InProgress = "In Progress";
            public const string Closed = "Closed";
        }

        public const string Send = "Process";
        public const string SendAll = "Process All";
        public const string DetailPasteLineCommand = "Detail Paste Line";
        public const string DetailResetOrderCommand = "Detail Reset Order";

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
        public const string UserNotFound = "Cannot find a user with Line Nbr. {0}";
        public const string AnswersNotfound = "No answers found";
        public const string SurveyQuestionNotFound = "A Survey Question named '{0}' does not exist";
        public const string SurveyDetailNotFound = "A Survey Detail encoder as '{0}' does not exist";
        public const string SurveyDetailPageNbrDoesNotMatch = "The Survey Detail Page Nbr found {0} does not match the data found {1}";
        public const string SurveyDetailQuesNbrDoesNotMatch = "The Survey Detail Question Nbr found {0} does not match the data found {1}";
        public const string ValueCannotBefound = "'{0}' with a value of '{1}' cannot be found in the system.";
        public const string TokenNoFound = "Cannot find a collector for token {0}";
        public const string TokenNoSurvey = "Token {0} is connected to a non existing survey";
        public const string TokenNoUser = "Token {0} is connected to a non existing survey user";

        #region SurveyAction
        public class SurveyAction {
            public const string Default = "All (New, Remind, Expire)";
            public const string RenderOnly = "Render";
            public const string NewOnly = "Send New";
            public const string RemindOnly = "Remind Un-Answered";
            public const string ExpireOnly = "Expire Un-Answered";
            public const string ProcessAnswers = "Process Answers";
            public const string ClearAnswers = "Clear Answers";
            public const string ClearAnswersHeader = "Please confirm";
            public const string ClearAnswersPrompt = "This will clear all processed answers. Do you wish to continue?";
        }
        #endregion


    }
}