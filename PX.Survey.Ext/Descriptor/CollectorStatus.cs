using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {
    public static class CollectorStatus {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new string[] { New, Rendered, Sent, Responded, Expired, Error },
                new string[] { 
                    Messages.CollectorStatus.New, Messages.CollectorStatus.Rendered,
                    Messages.CollectorStatus.Sent, Messages.CollectorStatus.Responded, 
                    Messages.CollectorStatus.Expired }) { }
        }

        public const string New = "N";
        public const string Rendered = "H";
        public const string Sent = "S";
        public const string Responded = "R";
        public const string Expired = "X";
        public const string Error = "E";

        public class _new : BqlString.Constant<_new> { public _new() : base(New) { } }
        public class rendered : BqlString.Constant<rendered> { public rendered() : base(Rendered) { } }
        public class sent : BqlString.Constant<sent> { public sent() : base(Sent) { } }
        public class responded : BqlString.Constant<responded> { public responded() : base(Responded) { } }
        public class expired : BqlString.Constant<expired> { public expired() : base(Expired) { } }
        public class error : BqlString.Constant<expired> { public error() : base(Error) { } }
    }
}
