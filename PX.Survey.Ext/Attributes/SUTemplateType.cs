using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {

    public class SUTemplateType {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new[] { 
                    Survey, Header, StartArea, QuestionPage,
                    ContentPage, EndArea, Footer, BadRequest },
                new[] { Messages.SUTemplateType.Survey, Messages.SUTemplateType.Header,
                    Messages.SUTemplateType.StartArea, Messages.SUTemplateType.QuestionPage, 
                    Messages.SUTemplateType.ContentPage, Messages.SUTemplateType.EndArea, 
                    Messages.SUTemplateType.Footer, Messages.SUTemplateType.BadRequest}) { }
        }

        public class DetailListAttribute : PXStringListAttribute {
            public DetailListAttribute() : base(
                new[] { 
                    Header, StartArea,
                    QuestionPage, ContentPage, 
                    EndArea, Footer },
                new[] { Messages.SUTemplateType.Header, Messages.SUTemplateType.StartArea,
                    Messages.SUTemplateType.QuestionPage, Messages.SUTemplateType.ContentPage,
                    Messages.SUTemplateType.EndArea, Messages.SUTemplateType.Footer}) { }
        }

        public const string Survey = "SU";
        public const string Header = "HE";
        public const string StartArea = "SA";
        public const string QuestionPage = "QU";
        public const string ContentPage = "CO";
        public const string EndArea = "EA";
        public const string Footer = "FO";
        public const string BadRequest = "BR";

        public class survey : BqlString.Constant<survey> { public survey() : base(Survey) { } }
        public class header : BqlString.Constant<header> { public header() : base(Header) { } }
        public class startArea : BqlString.Constant<startArea> { public startArea() : base(StartArea) { } }
        public class questionPage : BqlString.Constant<questionPage> { public questionPage() : base(QuestionPage) { } }
        public class contentPage : BqlString.Constant<contentPage> { public contentPage() : base(ContentPage) { } }
        public class endArea : BqlString.Constant<endArea> { public endArea() : base(EndArea) { } }
        public class footer : BqlString.Constant<footer> { public footer() : base(Footer) { } }
        public class badRequest : BqlString.Constant<badRequest> { public badRequest() : base(BadRequest) { } }
    }
}
