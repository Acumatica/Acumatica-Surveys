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

        public class user : BqlString.Constant<user> { public user() : base(User) { } }
        public class contact : BqlString.Constant<contact> { public contact() : base(Contact) { } }
        public class anonymous : BqlString.Constant<anonymous> { public anonymous() : base(Anonymous) { } }
        public class entity : BqlString.Constant<entity> { public entity() : base(Entity) { } }
    }
}
