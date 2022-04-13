using PX.Data;
using PX.Data.BQL;
using System.Linq;

namespace PX.Survey.Ext {

    public class SUComponentType {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new[] { 
                    Survey, Header, PageHeader, 
                    QuestionPage, CommentPage, ContentPage, 
                    PageFooter, Footer, BadRequest },
                new[] { Messages.SUComponentType.Survey, Messages.SUComponentType.Header, Messages.SUComponentType.PageHeader, 
                    Messages.SUComponentType.QuestionPage, Messages.SUComponentType.CommentPage, Messages.SUComponentType.ContentPage,
                    Messages.SUComponentType.PageFooter, Messages.SUComponentType.Footer, Messages.SUComponentType.BadRequest}) { }
        }

        public class DetailListAttribute : PXStringListAttribute {
            public DetailListAttribute() : base(
                new[] { 
                    Header, PageHeader,
                    QuestionPage, CommentPage, ContentPage, 
                    PageFooter, Footer },
                new[] { Messages.SUComponentType.Header, Messages.SUComponentType.PageHeader,
                    Messages.SUComponentType.QuestionPage, Messages.SUComponentType.CommentPage, Messages.SUComponentType.ContentPage,
                    Messages.SUComponentType.PageFooter, Messages.SUComponentType.Footer}) { }
        }

        public const string Survey = "SU";
        public const string Header = "HE";
        public const string PageHeader = "PH";
        public const string QuestionPage = "QU";
        public const string CommentPage = "CM";
        public const string ContentPage = "CO";
        public const string PageFooter = "PF";
        public const string Footer = "FO";
        public const string BadRequest = "BR";

        private static string[] needsSurrounding = new[] { QuestionPage, CommentPage, ContentPage };
        
        public static bool NeedsSurrounding(string templateType) {
            return templateType != null && needsSurrounding.Contains(templateType);
        }

        public class survey : BqlString.Constant<survey> { public survey() : base(Survey) { } }
        public class header : BqlString.Constant<header> { public header() : base(Header) { } }
        public class pageHeader : BqlString.Constant<pageHeader> { public pageHeader() : base(PageHeader) { } }
        public class questionPage : BqlString.Constant<questionPage> { public questionPage() : base(QuestionPage) { } }
        public class commentPage : BqlString.Constant<commentPage> { public commentPage() : base(CommentPage) { } }
        public class contentPage : BqlString.Constant<contentPage> { public contentPage() : base(ContentPage) { } }
        public class pageFooter : BqlString.Constant<pageFooter> { public pageFooter() : base(PageFooter) { } }
        public class footer : BqlString.Constant<footer> { public footer() : base(Footer) { } }
        public class badRequest : BqlString.Constant<badRequest> { public badRequest() : base(BadRequest) { } }
    }
}
