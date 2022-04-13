using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {
    public static class CollectorDataStatus {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new string[] { New, Updated, Ignored, Processed, Error },
                new string[] { 
                    Messages.CollectorDataStatus.New, Messages.CollectorDataStatus.Updated, Messages.CollectorDataStatus.Ignored,
                    Messages.CollectorDataStatus.Processed, Messages.CollectorDataStatus.Error, 
                }) { }
        }

        public const string New = "N";
        public const string Updated = "U";
        public const string Ignored = "I";
        public const string Processed = "P";
        public const string Error = "E";

        public class _new : BqlString.Constant<_new> { public _new() : base(New) { } }
        public class updated : BqlString.Constant<updated> { public updated() : base(Updated) { } }
        public class ignored : BqlString.Constant<ignored> { public ignored() : base(Ignored) { } }
        public class processed : BqlString.Constant<processed> { public processed() : base(Processed) { } }
        public class error : BqlString.Constant<error> { public error() : base(Error) { } }
    }
}
