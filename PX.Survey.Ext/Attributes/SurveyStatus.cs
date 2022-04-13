using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {

    public static class SurveyStatus {

        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new string[] { 
                    Preparing, Started, 
                    InProgress, Closed },
                new string[] {
                    Messages.SurveyStatus.Preparing, Messages.SurveyStatus.Started,
                    Messages.SurveyStatus.InProgress, Messages.SurveyStatus.Closed,
                }) { }
        }

        public const string Preparing = "P";
        public const string Started = "S";
        public const string InProgress = "W";
        public const string Closed = "C";

        public class closed : BqlString.Constant<closed> { public closed() : base(Closed) { } }
        public class preparing : BqlString.Constant<preparing> { public preparing() : base(Preparing) { } }

        public static bool IsLocked(string status) {
            return !IsUnlocked(status);
        }

        public static bool IsUnlocked(string status) {
            return status == null || status == Preparing;
        }
    }
}
