using PX.Data;

namespace Covid19.Lib
{
    public static class SurveyResponseStatus
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(
                new string[] { CollectorNew, CollectorSent, CollectorResponded, CollectorExpired },
                new string[] { Messages.CollectorNew, Messages.CollectorSent, Messages.CollectorResponded, Messages.CollectorExpired })
            { }
        }

        public const string CollectorNew = "N";
        public const string CollectorSent = "S";
        public const string CollectorResponded = "R";
        public const string CollectorExpired = "E";

        public class CollectorNewStatus : PX.Data.BQL.BqlString.Constant<CollectorNewStatus> { public CollectorNewStatus() : base(CollectorNew) { } }
        public class CollectorSentStatus : PX.Data.BQL.BqlString.Constant<CollectorSentStatus> { public CollectorSentStatus() : base(CollectorSent) { } }
        public class CollectorRespondedStatus : PX.Data.BQL.BqlString.Constant<CollectorRespondedStatus> { public CollectorRespondedStatus() : base(CollectorResponded) { } }
        public class CollectorExpiredStatus : PX.Data.BQL.BqlString.Constant<CollectorExpiredStatus> { public CollectorExpiredStatus() : base(CollectorExpired) { } }
    }
}
