using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {

    public class TemplateType {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new[] { Header, PageHeader, QuestionPage,
                    ContentPage, PageFooter, Footer },
                new[] { Messages.TemplateType.Header, Messages.TemplateType.PageHeader,
                    Messages.TemplateType.QuestionPage, Messages.TemplateType.ContentPage, 
                    Messages.TemplateType.PageFooter, Messages.TemplateType.Footer}) { }
        }

        public const string Header = "HE";
        public const string PageHeader = "PH";
        public const string QuestionPage = "QU";
        public const string ContentPage = "CO";
        public const string PageFooter = "PF";
        public const string Footer = "FO";

        public class header : BqlString.Constant<header> { public header() : base(Header) { } }
        public class pageHeader : BqlString.Constant<pageHeader> { public pageHeader() : base(PageHeader) { } }
        public class questionPage : BqlString.Constant<questionPage> { public questionPage() : base(QuestionPage) { } }
        public class contentPage : BqlString.Constant<contentPage> { public contentPage() : base(ContentPage) { } }
        public class pageFooter : BqlString.Constant<pageFooter> { public pageFooter() : base(PageFooter) { } }
        public class footer : BqlString.Constant<footer> { public footer() : base(Footer) { } }
    }
}
