using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {
    public static class CollectorDataStatus {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new string[] { New, Connected, Processed, Error },
                new string[] { 
                    Messages.CollectorDataStatus.New, Messages.CollectorDataStatus.Connected,
                    Messages.CollectorDataStatus.Processed, Messages.CollectorDataStatus.Error, 
                }) { }
        }

        public const string New = "N";
        public const string Connected = "C";
        public const string Processed = "P";
        public const string Error = "E";

        public class _new : BqlString.Constant<_new> { public _new() : base(New) { } }
        public class connected : BqlString.Constant<connected> { public connected() : base(Connected) { } }
        public class processed : BqlString.Constant<processed> { public processed() : base(Processed) { } }
        public class error : BqlString.Constant<error> { public error() : base(Error) { } }
    }
}
