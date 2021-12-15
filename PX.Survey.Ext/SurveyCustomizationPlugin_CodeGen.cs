using PX.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable IdentifierTypo

namespace PX.Survey.Ext
{
    public partial class SurveyCustomizationPlugin
    {

        #region SurveyComponents

        #region SUBADREQUEST
        public SurveyComponent scSUBADREQUEST = new SurveyComponent
        {
            ComponentID = "SUBADREQUEST",
            ComponentType = "BR",
            Active = true,
            Description = "Bad Request",
            Body = @"&lt;div class=&quot;px-5&quot;&gt;&#xA;    &lt;h1 class=&quot;title has-text-centered is-2&quot;&gt;{{Message}}&lt;/h1&gt;&#xA;&lt;/div&gt;&#xA;&lt;footer class=&quot;footer has-background-white&quot;&gt;&#xA;    &lt;div class=&quot;content has-text-centered&quot;&gt;&#xA;        &lt;button type=&quot;submit&quot; class=&quot;button is-info is-medium&quot; name=&quot;action&quot; value=&quot;start&quot;&gt;Start Over&lt;/button&gt;&#xA;    &lt;/div&gt;&#xA;&lt;/footer&gt;",
            NoteID = Guid.Parse("ce90e734-d4b2-eb11-a389-3c6aa7cb36ca")
        };
        #endregion //SUBADREQUEST

        #region SUCHECKBOXES
        public SurveyComponent scSUCHECKBOXES = new SurveyComponent
        {
            ComponentID = "SUCHECKBOXES",
            ComponentType = "QU",
            Active = true,
            Description = "SurveyCheckBoxes",
            Body = @"{{ PNbr = SurveyDetail.PageNbr }}&#xA;{{ QNbr = SurveyDetail.QuestionNbr }}&#xA;{{ LNbr = SurveyDetail.LineNbr }}&#xA;&lt;div class=&quot;px-5&quot;&gt;&#xA;                    &lt;div class=&quot;field mb-5&quot;&gt;&#xA;                        &lt;label class=&quot;label&quot;&gt;* {{QNbr}}. {{SurveyDetail.Description}}&lt;/label&gt;&#xA;{{ for QDet in Question.Details }}&#xA;    {{ IDNbr = QDet.ValueID }}&#xA;    {{ UniqueID = PNbr + '.' + QNbr + '.' + LNbr + '.' + IDNbr }}&#xA;                        &lt;div class=&quot;field&quot;&gt;&#xA;                            &lt;input type=&quot;checkbox&quot; class=&quot;is-checkbox is-black&quot; id=&quot;control.checkbox.{{UniqueID}}&quot; name=&quot;{{PNbr}}.{{QNbr}}.{{LNbr}}&quot; aria-labelledby=&quot;control.checkbox.label.{{UniqueID}}&quot; value=&quot;{{IDNbr}}&quot; {{ if SurveyDetail.Required }}{{~ required=&quot;required&quot; ~}}&quot; {{ end }}/&gt;&#xA;                            &lt;label id=&quot;control.checkbox.label.{{UniqueID}}&quot; for=&quot;control.checkbox.{{UniqueID}}&quot;&gt;{{IDNbr}} = {{QDet.Description}}&lt;/label&gt;&#xA;                        &lt;/div&gt;&#xA;{{ end # for }}&#xA;                    &lt;/div&gt;&#xA;                &lt;/div&gt;",
            NoteID = Guid.Parse("792c53d5-21c5-eb11-a38d-3c6aa7cb36ca")
        };
        #endregion //SUCHECKBOXES

        #region SUMAIN
        public SurveyComponent scSUMAIN = new SurveyComponent
        {
            ComponentID = "SUMAIN",
            ComponentType = "SU",
            Active = true,
            Description = "SurveyMainTemplate",
            Body = @"&lt;!DOCTYPE html&gt;&#xA;&lt;html&gt;&#xA;    &lt;head&gt;&#xA;        &lt;meta charset=&quot;utf-8&quot; /&gt;&#xA;        &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1&quot; /&gt;&#xA;        &lt;title&gt;{{Survey.Title}}&lt;/title&gt;&#xA;        &lt;link rel=&quot;stylesheet&quot; href=&quot;https://cdn.jsdelivr.net/npm/bulma@0.9.2/css/bulma.min.css&quot; /&gt;&#xA;        &lt;link rel=&quot;stylesheet&quot; href=&quot;https://gitcdn.link/cdn/Wikiki/bulma-checkradio/38c95578f465f4c3805f578490925f337b87404d/dist/css/bulma-checkradio.min.css&quot; /&gt;&#xA;        &lt;style&gt;&lt;/style&gt;&#xA;    &lt;/head&gt;&#xA;    &lt;body class=&quot;has-background-light&quot; style=&quot;height: 100%; min-height: 100vh&quot;&gt;&#xA;        &lt;section class=&quot;section container is-max-desktop py-5&quot;&gt;&#xA;            &lt;form class=&quot;is-max-desktop has-background-white px-0 py-5 container is-fluid&quot; name=&quot;{{Survey.FormName}}&quot; action=&quot;{{SurveyURL}}&quot; method=&quot;post&quot; enctype=&quot;application/x-www-form-urlencoded&quot;&gt;&#xA;                {{InnerContent}}&#xA;            &lt;/form&gt;&#xA;        &lt;/section&gt;&#xA;    &lt;/body&gt;&#xA;&lt;/html&gt;",
            NoteID = Guid.Parse("43076712-5abf-eb11-a38b-3c6aa7cb36ca")
        };
        #endregion //SUMAIN

        #region SUPREVNEXT
        public SurveyComponent scSUPREVNEXT = new SurveyComponent
        {
            ComponentID = "SUPREVNEXT",
            ComponentType = "PF",
            Active = true,
            Description = "SurveyPrevNext",
            Body = @"",
            NoteID = Guid.Parse("29eb7700-5bbf-eb11-a38b-3c6aa7cb36ca")
        };
        #endregion //SUPREVNEXT

        #region SUPROGRESS
        public SurveyComponent scSUPROGRESS = new SurveyComponent
        {
            ComponentID = "SUPROGRESS",
            ComponentType = "PH",
            Active = true,
            Description = "SurveyProgress",
            Body = @"&lt;header class=&quot;hero&quot;&gt;&#xA;    &lt;a class=&quot;px-5&quot; target=&quot;_blank&quot; href=&quot;https://www.acumatica.com&quot;&gt;&lt;img src=&quot;https://cdn.acumatica.com/content/themes/acumatica/assets/img/svg/logo-dark.svg&quot; alt=&quot;Acumatica&quot; class=&quot;image&quot; style=&quot;width: 250px&quot; /&gt;&lt;/a&gt;&#xA;    &lt;div class=&quot;hero-body py-4 mt-4 px-0&quot;&gt;&#xA;        &lt;h1 class=&quot;title has-background-info-dark has-text-white px-4 py-4 mb-4&quot;&gt;{{Survey.Name}}&lt;/h1&gt;&#xA;        &lt;h2 class=&quot;subtitle has-background-info has-text-white px-4 py-2 mb-2&quot;&gt;&lt;/h2&gt;&#xA;        &lt;div class=&quot;columns is-mobile my-4&quot;&gt;&#xA;            &lt;div class=&quot;column is-three-fifths is-offset-one-fifth&quot;&gt;&#xA;                &lt;progress class=&quot;progress progress-text is-primary&quot; value=&quot;{{CurrentPageNbr}}&quot; max=&quot;{{Nbpages}}&quot;&gt;&lt;/progress&gt;&#xA;            &lt;/div&gt;&#xA;        &lt;/div&gt;&#xA;    &lt;/div&gt;&#xA;&lt;/header&gt;",
            NoteID = Guid.Parse("808815ce-6abf-eb11-a38b-3c6aa7cb36ca")
        };
        #endregion //SUPROGRESS

        #region SURADIOBUTTONS
        public SurveyComponent scSURADIOBUTTONS = new SurveyComponent
        {
            ComponentID = "SURADIOBUTTONS",
            ComponentType = "QU",
            Active = true,
            Description = "SurveyRadioButtons",
            Body = @"{{ PNbr = SurveyDetail.PageNbr }}&#xA;{{ QNbr = SurveyDetail.QuestionNbr }}&#xA;{{ LNbr = SurveyDetail.LineNbr }}&#xA;&lt;div class=&quot;px-5&quot;&gt;&#xA;                    &lt;div class=&quot;field mb-5&quot;&gt;&#xA;                        &lt;label class=&quot;label&quot;&gt;* {{QNbr}}. {{SurveyDetail.Description}}&lt;/label&gt;&#xA;{{ for QDet in Question.Details }}&#xA;    {{ IDNbr = QDet.ValueID }}&#xA;    {{ UniqueID = PNbr + '.' + QNbr + '.' + LNbr + '.' + IDNbr }}&#xA;                        &lt;div class=&quot;field&quot;&gt;&#xA;                            &lt;input type=&quot;radio&quot; class=&quot;is-checkradio is-black&quot; id=&quot;control.radio.{{UniqueID}}&quot; name=&quot;{{PNbr}}.{{QNbr}}.{{LNbr}}&quot; aria-labelledby=&quot;control.radio.label.{{UniqueID}}&quot; value=&quot;{{IDNbr}}&quot; {{ if SurveyDetail.Required }}{{~ required=&quot;required&quot; ~}}{{ end }}/&gt;&#xA;                            &lt;label id=&quot;control.radio.label.{{UniqueID}}&quot; for=&quot;control.radio.{{UniqueID}}&quot;&gt;{{IDNbr}} = {{QDet.Description}}&lt;/label&gt;&#xA;                        &lt;/div&gt;&#xA;{{ end # for }}&#xA;                    &lt;/div&gt;&#xA;                &lt;/div&gt;",
            NoteID = Guid.Parse("c60f9902-d0c1-eb11-a38b-3c6aa7cb36ca")
        };
        #endregion //SURADIOBUTTONS

        #region SUTABLEBODYRB
        public SurveyComponent scSUTABLEBODYRB = new SurveyComponent
        {
            ComponentID = "SUTABLEBODYRB",
            ComponentType = "QU",
            Active = true,
            Description = "SurveyTableTBodyRadioButtons",
            Body = @"",
            NoteID = Guid.Parse("61e1ae8b-17c3-eb11-a38b-3c6aa7cb36ca")
        };
        #endregion //SUTABLEBODYRB

        #region SUTABLEHEAD
        public SurveyComponent scSUTABLEHEAD = new SurveyComponent
        {
            ComponentID = "SUTABLEHEAD",
            ComponentType = "QU",
            Active = true,
            Description = "SurveyTableTHead",
            Body = @"{{ PNbr = SurveyDetail.PageNbr }}&#xA;{{ QNbr = SurveyDetail.QuestionNbr }}&#xA;{{ LNbr = SurveyDetail.LineNbr }}&#xA;                    &lt;div class=&quot;field mb-5&quot;&gt;&#xA;                        &lt;label class=&quot;label&quot; id=&quot;control.radio.label.{{PNbr}}&quot;&gt;* {{SurveyDetail.Description}}&lt;/label&gt;&#xA;                        &lt;table class=&quot;table is-striped is-hoverable is-fullwidth&quot;&gt;&#xA;                            &lt;thead&gt;&#xA;                                &lt;tr&gt;&#xA;                                    &lt;th class=&quot;is-size-7&quot;&gt;&lt;/th&gt;&#xA;{{ for QDet in Question.Details }}&#xA;     {{ IDNbr = QDet.ValueID }}&#xA;                                    &lt;th class=&quot;is-size-7&quot;&gt;{{IDNbr}} = {{QDet.Description}}&lt;/th&gt;&#xA;{{ end # for }}&#xA;                                &lt;/tr&gt;&#xA;                            &lt;/thead&gt;&#xA;                        &lt;/table&gt;&#xA;                    &lt;/div&gt;",
            NoteID = Guid.Parse("a3a8851c-16c3-eb11-a38b-3c6aa7cb36ca")
        };
        #endregion //SUTABLEHEAD

        #region SUTEXTAREA
        public SurveyComponent scSUTEXTAREA = new SurveyComponent
        {
            ComponentID = "SUTEXTAREA",
            ComponentType = "CM",
            Active = true,
            Description = "SurveyTextArea",
            Body = @"{{ PNbr = SurveyDetail.PageNbr }}&#xA;{{ QNbr = SurveyDetail.QuestionNbr }}&#xA;{{ LNbr = SurveyDetail.LineNbr }}&#xA;{{ UniqueID = PNbr + '.' + QNbr + '.' + LNbr }}&#xA;&lt;div class=&quot;field mb-5&quot;&gt;&#xA;                        &lt;label class=&quot;label&quot; id=&quot;control.textarea.label.{{UniqueID}}&quot; for=&quot;control.textarea.{{UniqueID}}&quot;&gt;{{QNbr}}. {{SurveyDetail.Description}}&lt;/label&gt;&#xA;                        &lt;div class=&quot;control&quot;&gt;&#xA;                            &lt;textarea class=&quot;textarea&quot; maxlength=&quot;{{SurveyDetail.MaxLength}}&quot; placeholder=&quot;&quot; rows=&quot;{{SurveyDetail.NbrOfRows}}&quot; id=&quot;control.textarea.{{UniqueID}}&quot; name=&quot;{{UniqueID}}&quot; aria-labelledby=&quot;control.textarea.label.{{UniqueID}}&quot; {{ if SurveyDetail.Required }}{{~ required=&quot;required&quot; ~}}{{ end }}&gt;&lt;/textarea&gt;&#xA;                        &lt;/div&gt;&#xA;                    &lt;/div&gt;",
            NoteID = Guid.Parse("c92705eb-4dc2-eb11-a38b-3c6aa7cb36ca")
        };
        #endregion //SUTEXTAREA

        #region SUTHANKYOU
        public SurveyComponent scSUTHANKYOU = new SurveyComponent
        {
            ComponentID = "SUTHANKYOU",
            ComponentType = "FO",
            Active = true,
            Description = "SurveyThankYouPage",
            Body = @"&lt;div class=&quot;px-5&quot;&gt;&#xA;    &lt;h1 class=&quot;title has-text-centered is-2&quot;&gt;{{SurveyDetail.Description}}&lt;/h1&gt;&#xA;&lt;/div&gt;",
            NoteID = Guid.Parse("bf5226ca-4fc2-eb11-a38b-3c6aa7cb36ca")
        };
        #endregion //SUTHANKYOU

        #region SUWELCOME
        public SurveyComponent scSUWELCOME = new SurveyComponent
        {
            ComponentID = "SUWELCOME",
            ComponentType = "HE",
            Active = true,
            Description = "SurveyWelcomePage",
            Body = @"&lt;div class=&quot;px-5&quot;&gt;&#xA;    &lt;h1 class=&quot;title has-text-centered is-2&quot;&gt;{{SurveyDetail.Description}}&lt;/h1&gt;&#xA;&lt;/div&gt;",
            NoteID = Guid.Parse("d7f112c2-4fc2-eb11-a38b-3c6aa7cb36ca")
        };
        #endregion //SUWELCOME

        #region InitializeSurveyComponents
        public void InitializeSurveyComponents()
        {
            InitializeSurveyComponent(scSUBADREQUEST);
            InitializeSurveyComponent(scSUCHECKBOXES);
            InitializeSurveyComponent(scSUMAIN);
            InitializeSurveyComponent(scSUPREVNEXT);
            InitializeSurveyComponent(scSUPROGRESS);
            InitializeSurveyComponent(scSURADIOBUTTONS);
            InitializeSurveyComponent(scSUTABLEBODYRB);
            InitializeSurveyComponent(scSUTABLEHEAD);
            InitializeSurveyComponent(scSUTEXTAREA);
            InitializeSurveyComponent(scSUTHANKYOU);
            InitializeSurveyComponent(scSUWELCOME);

        }
        #endregion //InitializeSurveyComponents
        #endregion //SurveyComponents
        #region Attributes

        #region SUCOMMENT
        public PX.Objects.CS.CSAttribute csSUCOMMENT = new PX.Objects.CS.CSAttribute
        {
            AttributeID = "SUCOMMENT",
            Description = "Comment",
            ControlType = 1,
            IsInternal = false,
            NoteID = Guid.Parse("1046eda4-4dc2-eb11-a38b-3c6aa7cb36ca")
        };

        public List<PX.Objects.CS.CSAttributeDetail> csSUCOMMENTDetail = new List<PX.Objects.CS.CSAttributeDetail>
        {

        };
        #endregion //SUCOMMENT

        #region SUEASYNESS
        public PX.Objects.CS.CSAttribute csSUEASYNESS = new PX.Objects.CS.CSAttribute
        {
            AttributeID = "SUEASYNESS",
            Description = "Easyness",
            ControlType = 2,
            IsInternal = false,
            NoteID = Guid.Parse("f067f8d8-cfc1-eb11-a38b-3c6aa7cb36ca")
        };

        public List<PX.Objects.CS.CSAttributeDetail> csSUEASYNESSDetail = new List<PX.Objects.CS.CSAttributeDetail>
            {
                            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "5",
                Description = "Very Easy",
                SortOrder = 1,
                Disabled = false,
                NoteID = Guid.Parse("e867f8d8-cfc1-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "4",
                Description = "Easy",
                SortOrder = 2,
                Disabled = false,
                NoteID = Guid.Parse("ea67f8d8-cfc1-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "3",
                Description = "Neutral (Neither Easy nor Difficult)",
                SortOrder = 3,
                Disabled = false,
                NoteID = Guid.Parse("ec67f8d8-cfc1-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "2",
                Description = "Difficult",
                SortOrder = 4,
                Disabled = false,
                NoteID = Guid.Parse("ee67f8d8-cfc1-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "1",
                Description = "Very Difficult",
                SortOrder = 5,
                Disabled = false,
                NoteID = Guid.Parse("f067f8d8-cfc1-eb11-a38b-3c6aa7cb36ca")
            },
            };
        #endregion //SUEASYNESS

        #region SUHELPRES
        public PX.Objects.CS.CSAttribute csSUHELPRES = new PX.Objects.CS.CSAttribute
        {
            AttributeID = "SUHELPRES",
            Description = "Help Resources",
            ControlType = 2,
            IsInternal = false,
            NoteID = Guid.Parse("8c5ea96d-cec1-eb11-a38b-3c6aa7cb36ca")
        };

        public List<PX.Objects.CS.CSAttributeDetail> csSUHELPRESDetail = new List<PX.Objects.CS.CSAttributeDetail>
            {
                            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "6",
                Description = "Acumatica KB",
                SortOrder = 6,
                Disabled = false,
                NoteID = Guid.Parse("d5269589-cec1-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "5",
                Description = "Acumatica Technical Support",
                SortOrder = 5,
                Disabled = false,
                NoteID = Guid.Parse("d1269589-cec1-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "4",
                Description = "Training materials",
                SortOrder = 4,
                Disabled = false,
                NoteID = Guid.Parse("5c732983-cec1-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "3",
                Description = "Pdf User Guides",
                SortOrder = 3,
                Disabled = false,
                NoteID = Guid.Parse("84232a79-cec1-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "2",
                Description = "In-product help",
                SortOrder = 2,
                Disabled = false,
                NoteID = Guid.Parse("935ea96d-cec1-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "1",
                Description = "Help of colleagues",
                SortOrder = 1,
                Disabled = false,
                NoteID = Guid.Parse("8c5ea96d-cec1-eb11-a38b-3c6aa7cb36ca")
            },
            };
        #endregion //SUHELPRES

        #region SURESOLVE
        public PX.Objects.CS.CSAttribute csSURESOLVE = new PX.Objects.CS.CSAttribute
        {
            AttributeID = "SURESOLVE",
            Description = "Issue resolved",
            ControlType = 2,
            IsInternal = false,
            NoteID = Guid.Parse("1e015864-33c2-eb11-a38b-3c6aa7cb36ca")
        };

        public List<PX.Objects.CS.CSAttributeDetail> csSURESOLVEDetail = new List<PX.Objects.CS.CSAttributeDetail>
            {
                            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "2",
                Description = "Yes, with multiple contacts to Acumatica Support",
                SortOrder = 2,
                Disabled = false,
                NoteID = Guid.Parse("1c015864-33c2-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "1",
                Description = "Yes, with only one contact to Acumatica Support",
                SortOrder = 1,
                Disabled = false,
                NoteID = Guid.Parse("1a015864-33c2-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "0",
                Description = "No, my issue was not resolved",
                SortOrder = 3,
                Disabled = false,
                NoteID = Guid.Parse("1e015864-33c2-eb11-a38b-3c6aa7cb36ca")
            },
            };
        #endregion //SURESOLVE

        #region SUSATISFAC
        public PX.Objects.CS.CSAttribute csSUSATISFAC = new PX.Objects.CS.CSAttribute
        {
            AttributeID = "SUSATISFAC",
            Description = "Satisfaction",
            ControlType = 2,
            IsInternal = false,
            NoteID = Guid.Parse("ac725d33-70bf-eb11-a38b-3c6aa7cb36ca")
        };

        public List<PX.Objects.CS.CSAttributeDetail> csSUSATISFACDetail = new List<PX.Objects.CS.CSAttributeDetail>
            {
                            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "5",
                Description = "Very Satisfied",
                SortOrder = 1,
                Disabled = false,
                NoteID = Guid.Parse("29768a51-70bf-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "4",
                Description = "Satisfied",
                SortOrder = 2,
                Disabled = false,
                NoteID = Guid.Parse("8baa2446-70bf-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "3",
                Description = "Neutral (Neither Satisfied Nor Dissatisfied)",
                SortOrder = 3,
                Disabled = false,
                NoteID = Guid.Parse("87aa2446-70bf-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "2",
                Description = "Dissatisfied",
                SortOrder = 4,
                Disabled = false,
                NoteID = Guid.Parse("aec2e83f-70bf-eb11-a38b-3c6aa7cb36ca")
            },            new PX.Objects.CS.CSAttributeDetail
            {
                ValueID = "1",
                Description = "Very Dissatisfied",
                SortOrder = 5,
                Disabled = false,
                NoteID = Guid.Parse("ac725d33-70bf-eb11-a38b-3c6aa7cb36ca")
            },
            };
        #endregion //SUSATISFAC

        #region InitializeSurveyAttributes
        public void InitializeSurveyAttributes()
        {
            InitializeSurveyAttribute(csSUCOMMENT, csSUCOMMENTDetail);
            InitializeSurveyAttribute(csSUEASYNESS, csSUEASYNESSDetail);
            InitializeSurveyAttribute(csSUHELPRES, csSUHELPRESDetail);
            InitializeSurveyAttribute(csSURESOLVE, csSURESOLVEDetail);
            InitializeSurveyAttribute(csSUSATISFAC, csSUSATISFACDetail);
        }
        #endregion //InitializeSurveyAttributes
        #endregion //Attributes
    }
}
