using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {

    public class SurveyAction {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new[] { DefaultAction, NewOnly, RemindOnly,
                    ExpireOnly, ProcessAnswers },
                new[] { Messages.SurveyAction.Default, Messages.SurveyAction.NewOnly,
                    Messages.SurveyAction.RemindOnly, Messages.SurveyAction.ExpireOnly,
                    Messages.SurveyAction.ProcessAnswers}) { }
        }

        public const string DefaultAction = "D";
        public const string NewOnly = "N";
        public const string RemindOnly = "R";
        public const string ExpireOnly = "E";
        public const string ProcessAnswers = "P";

        public class _default : BqlString.Constant<_default> { public _default() : base(DefaultAction) { } }
        public class newOnly : BqlString.Constant<newOnly> { public newOnly() : base(NewOnly) { } }
        public class remindOnly : BqlString.Constant<remindOnly> { public remindOnly() : base(RemindOnly) { } }
        public class expireOnly : BqlString.Constant<expireOnly> { public expireOnly() : base(ExpireOnly) { } }
        public class processAnswers : BqlString.Constant<processAnswers> { public processAnswers() : base(ProcessAnswers) { } }
    }
}
