using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {
    public static class CollectorStatus {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new string[] { 
                    New, Sent, 
                    Partially, Completed, 
                    Reminded, Expired, 
                    Error, Processed },
                new string[] { 
                    Messages.CollectorStatus.New, Messages.CollectorStatus.Sent, 
                    Messages.CollectorStatus.Partially, Messages.CollectorStatus.Completed,
                    Messages.CollectorStatus.Reminded, Messages.CollectorStatus.Expired, 
                    Messages.CollectorStatus.Error, Messages.CollectorStatus.Processed }) { }
        }

        public const string New = "N";
        public const string Sent = "S";
        public const string Reminded = "M";
        public const string Partially = "Y";
        public const string Completed = "C";
        public const string Expired = "X";
        public const string Error = "E";
        public const string Processed = "P";

        public class _new : BqlString.Constant<_new> { public _new() : base(New) { } }
        public class sent : BqlString.Constant<sent> { public sent() : base(Sent) { } }
        public class reminded : BqlString.Constant<reminded> { public reminded() : base(Reminded) { } }
        public class partially : BqlString.Constant<partially> { public partially() : base(Partially) { } }
        public class completed : BqlString.Constant<completed> { public completed() : base(Completed) { } }
        public class expired : BqlString.Constant<expired> { public expired() : base(Expired) { } }
        public class error : BqlString.Constant<error> { public error() : base(Error) { } }
        public class processed : BqlString.Constant<processed> { public processed() : base(Processed) { } }
    }
}
