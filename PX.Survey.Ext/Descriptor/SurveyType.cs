using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {
    public static class SurveyType {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new string[] { User, Contact, Anonymous, Device},
                new string[] { 
                    Messages.SurveyType.User, Messages.SurveyType.Contact,
                    Messages.SurveyType.Anonymous, Messages.SurveyType.Device, 
                }) { }
        }

        public const string User = "U";
        public const string Contact = "C";
        public const string Anonymous = "A";
        public const string Device = "D";

        //public class _new : BqlString.Constant<_new> { public _new() : base(New) { } }
        //public class connected : BqlString.Constant<connected> { public connected() : base(Connected) { } }
        //public class processed : BqlString.Constant<processed> { public processed() : base(Processed) { } }
        //public class error : BqlString.Constant<error> { public error() : base(Error) { } }
    }
}
