using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {
    public static class SurveyTarget {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new string[] { User, Contact, Anonymous, Entity },
                new string[] { 
                    Messages.SurveyTarget.User, Messages.SurveyTarget.Contact,
                    Messages.SurveyTarget.Anonymous, Messages.SurveyTarget.Entity,
                }) { }
        }

        public const string User = "U";
        public const string Contact = "C";
        public const string Anonymous = "A";
        public const string Entity = "E";

        //public class _new : BqlString.Constant<_new> { public _new() : base(New) { } }
        //public class connected : BqlString.Constant<connected> { public connected() : base(Connected) { } }
        //public class processed : BqlString.Constant<processed> { public processed() : base(Processed) { } }
        public class entity : BqlString.Constant<entity> { public entity() : base(Entity) { } }
    }
}
