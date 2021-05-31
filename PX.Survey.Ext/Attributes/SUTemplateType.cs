﻿using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {

    public class SUTemplateType {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new[] { 
                    Survey, Header, PageHeader, QuestionPage,
                    ContentPage, PageFooter, Footer, BadRequest },
                new[] { Messages.SUTemplateType.Survey, Messages.SUTemplateType.Header,
                    Messages.SUTemplateType.PageHeader, Messages.SUTemplateType.QuestionPage, 
                    Messages.SUTemplateType.ContentPage, Messages.SUTemplateType.PageFooter, 
                    Messages.SUTemplateType.Footer, Messages.SUTemplateType.BadRequest}) { }
        }

        public class DetailListAttribute : PXStringListAttribute {
            public DetailListAttribute() : base(
                new[] { 
                    Header, PageHeader,
                    QuestionPage, ContentPage, 
                    PageFooter, Footer },
                new[] { Messages.SUTemplateType.Header, Messages.SUTemplateType.PageHeader,
                    Messages.SUTemplateType.QuestionPage, Messages.SUTemplateType.ContentPage,
                    Messages.SUTemplateType.PageFooter, Messages.SUTemplateType.Footer}) { }
        }

        public const string Survey = "SU";
        public const string Header = "HE";
        public const string PageHeader = "SA";
        public const string QuestionPage = "QU";
        public const string ContentPage = "CO";
        public const string PageFooter = "EA";
        public const string Footer = "FO";
        public const string BadRequest = "BR";

        public class survey : BqlString.Constant<survey> { public survey() : base(Survey) { } }
        public class header : BqlString.Constant<header> { public header() : base(Header) { } }
        public class pageHeader : BqlString.Constant<pageHeader> { public pageHeader() : base(PageHeader) { } }
        public class questionPage : BqlString.Constant<questionPage> { public questionPage() : base(QuestionPage) { } }
        public class contentPage : BqlString.Constant<contentPage> { public contentPage() : base(ContentPage) { } }
        public class pageFooter : BqlString.Constant<pageFooter> { public pageFooter() : base(PageFooter) { } }
        public class footer : BqlString.Constant<footer> { public footer() : base(Footer) { } }
        public class badRequest : BqlString.Constant<badRequest> { public badRequest() : base(BadRequest) { } }
    }
}