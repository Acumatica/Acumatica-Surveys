using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Survey.Ext.WebHook;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using PX.Data.BQL.Fluent;
using PX.Objects.FS;
using PX.Api.Webhooks.DAC;
using PX.Data.BQL;
using PX.Objects.SO;


namespace PX.Survey.Ext {

    public class SurveyGenerator { 

        private SurveyMaint graph;

        private static string URL = "SurveyURL";
        private static string TOKEN = "Token";
        private static string INNER_CONTENT = "InnerContent";
        private static string INNER_CONTENT_LIST = INNER_CONTENT + "List";
        private static string CURRENT_PAGE_NBR = "CurrentPageNbr";
        private static string FIRST_PAGE_NBR = "FirstPageNbr";
        private static string LAST_PAGE_NBR = "LastPageNbr";
        private static string NB_PAGES = "Nbpages";
        private static string IS_SINGLE_PAGE = "IsSinglePage";
        private static string IS_FIRST_PAGE = "IsFirstPage";
        private static string IS_LAST_PAGE = "IsLastPage";
        private static string NEXT_IS_LAST = "NextIsLast";
        private static string NEXT_IS_QUES = "NextIsQuestion";
        private static string NEXT_IS_FOOT = "NextIsFooter";
        private static string ENTITY_ROW = "EntityRow";
        private static string ENTITY_TYPE = "EntityType";
        private static string ENTITY_NAME = "EntityName";
        private static string ENTITY_DESC = "EntityDesc";
        private static string ENTITY_FIELDS = "EntityFields";
        private static string _returnUrl;

        static SurveyGenerator() {
            GetReturnUrl();
        }

        private static string GetReturnUrl() {
            // Code taken from PX.Api.Webhooks.Owin.Configuration.ReturnUrl._get
            // Added here because WebHookMaint crashes when retrieving the WebHook URL in background thread
            // because HttpContext.Current is null
            //PXTrace.WriteInformation(HttpContext.Current.Request.GetWebsiteUrl());
            if (HttpContext.Current != null) {
                string applicationPath = HttpContext.Current.Request.ApplicationPath;
                var str = (applicationPath != null) ? applicationPath.Trim(new char[] { '/' }) : null;
                var str1 = string.IsNullOrEmpty(str) ? string.Empty : string.Concat(str, "/");
                //HttpContext.Current.Request.GetWebsiteUrl() 
                PXTrace.WriteInformation(HttpContext.Current.Request.GetWebsiteUrl());
                _returnUrl = string.Concat(HttpContext.Current.Request.GetWebsiteUrl(), str1, "Webhooks");
            }
            return _returnUrl;
        }

        public SurveyGenerator() : this(PXGraph.CreateInstance<SurveyMaint>()) {
        }

        public SurveyGenerator(SurveyMaint graph) {
            this.graph = graph;
            GetReturnUrl();
        }

        public string GenerateBadRequestPage(string token, string message) {
            var setup = graph.SurveySetup.Current;
            var pageTemplate = SurveyComponent.PK.Find(graph, setup.BadRequestID);
            var template = Template.Parse(pageTemplate.Body);
            var context = new TemplateContext();
            var container = new ScriptObject {
                {setup.GetType().Name, setup},
                {TOKEN, token},
                {"Message", message},
            };
            //container.SetValue(AcuFunctions.PREFIX, new AcuFunctions(), true);
            //container.SetValue(JsonFunctions.PREFIX, new JsonFunctions(), true);
            context.PushGlobal(container);
            var rendered = template.Render(context);
            return rendered;
        }

        public (string content, string token) GenerateSurveyPage(string token, int pageNbr) {
            var (survey, user, _, userCollector) = SurveyUtils.GetSurveyAndUser(graph, token);

            // Redirect with new token as anonymous surveys will pass the SurveyID as a token
            //userCollector.RefNoteID = new System.Guid();
            if (userCollector.Token != token) {
                return (null, userCollector.Token);
            }
            graph.Survey.Current = survey;
            var mainTemplateID = survey.TemplateID;
            var mainTemplate = SurveyComponent.PK.Find(graph, mainTemplateID);
            string mainTemplateText = mainTemplate.Body;
            if (string.IsNullOrEmpty(mainTemplateText?.Trim())) {
                throw new PXException(Messages.TemplateNeeded);
            }
            var template = Template.Parse(mainTemplateText);
            var context = GetSurveyContext(survey, user, token);
            FillEntityInfo(survey, user, userCollector, graph, context);
            var renderedComps = RenderComponentsForPage(survey, user, context, pageNbr);
            context.SetValue(new ScriptVariableGlobal(INNER_CONTENT_LIST), renderedComps);
            context.SetValue(new ScriptVariableGlobal(INNER_CONTENT), string.Join("\n", renderedComps));
            var rendered = template.Render(context);
            return (rendered, null);
        }

        private IEnumerable<string> RenderComponentsForPage(Survey survey, SurveyUser user, TemplateContext context, int pageNbr) {
            graph.Survey.Current = survey;
            var activeComponents = graph.Details.Select().FirstTableItems.Where(det => det.Active == true);
            var selectedComponents = SurveyUtils.SelectPages(activeComponents, pageNbr);
            var firstOfSelected = selectedComponents.FirstOrDefault();
            var renderedComponents = new List<string>();
            if (firstOfSelected == null) {
                return renderedComponents;
            }
            var selectedPageNbr = firstOfSelected?.PageNbr.Value ?? 99999;
            var nextComponents = SurveyUtils.SelectPages(activeComponents, ++selectedPageNbr);
            FillPageInfoLookAhead(context, activeComponents, nextComponents);
            var url = GetUrl(context, firstOfSelected.PageNbr.Value);
            context.SetValue(new ScriptVariableGlobal(URL), url);
            foreach (var detail in selectedComponents) {
                var component = SurveyComponent.PK.Find(graph, detail.ComponentID);
                var template = Template.Parse(component.Body);
                AddDetailContext(context, detail, component);
                FillPageInfo(context, activeComponents, detail);
                var rendered = template.Render(context);
                renderedComponents.Add(rendered);
            }
            // TODO Handle nothing rendered
            return renderedComponents;
        }

        private void FillPageInfoLookAhead(TemplateContext context, IEnumerable<SurveyDetail> details, IEnumerable<SurveyDetail> nextPages) {
            var max = details.Max(det => det.PageNbr);
            var hasQues = nextPages.Any(det => det.ComponentType == SUComponentType.QuestionPage);
            var hasFoot = nextPages.Any(det => det.ComponentType == SUComponentType.Footer);
            var nextNbr = nextPages.FirstOrDefault()?.PageNbr.Value ?? -1;
            var isLast = nextNbr == max;
            context.SetValue(new ScriptVariableGlobal(NEXT_IS_QUES), hasQues);
            context.SetValue(new ScriptVariableGlobal(NEXT_IS_FOOT), hasFoot);
            context.SetValue(new ScriptVariableGlobal(NEXT_IS_LAST), isLast);
        }

        private void AddDetailContext(TemplateContext context, SurveyDetail detail, SurveyComponent component) {
            context.SetValue(new ScriptVariableGlobal(detail.GetType().Name), detail);
            context.SetValue(new ScriptVariableGlobal(component.GetType().Name), component);
            if (detail.IsQuestion == true) {
                var question = GetQuestion(detail.AttributeID);
                context.SetValue(new ScriptVariableGlobal(question.GetType().Name), question);
            }
        }


        private void FillPageInfo(TemplateContext context, IEnumerable<SurveyDetail> details, SurveyDetail selectedPage) {
            var nbPages = details.Select(det => det.PageNbr).Distinct().Count();
            var min = details.Min(det => det.PageNbr);
            var max = details.Max(det => det.PageNbr);
            var current = selectedPage.PageNbr;
            context.SetValue(new ScriptVariableGlobal(CURRENT_PAGE_NBR), current);
            context.SetValue(new ScriptVariableGlobal(FIRST_PAGE_NBR), min);
            context.SetValue(new ScriptVariableGlobal(LAST_PAGE_NBR), max);
            context.SetValue(new ScriptVariableGlobal(NB_PAGES), nbPages);
            context.SetValue(new ScriptVariableGlobal(IS_FIRST_PAGE), current == min);
            context.SetValue(new ScriptVariableGlobal(IS_LAST_PAGE), current == max);
        }

        private TemplateContext GetSurveyContext(Survey survey, SurveyUser user, string token) {
            var setup = graph.SurveySetup.Current;
            var context = new TemplateContext();
            context.MemberRenamer = MyMemberRenamerDelegate;
            context.MemberFilter = MyMemberFilterDelegate;
            var url = GetUrl(survey, token, null);
            var container = new ScriptObject {
                {survey.GetType().Name, survey},
                {setup.GetType().Name, setup},
                {user.GetType().Name, user},
                {IS_SINGLE_PAGE, survey.Layout == SurveyLayout.SinglePage},
                {TOKEN, token},
                {URL, url},
            };
            //container.SetValue(AcuFunctions.PREFIX, new AcuFunctions(), true);
            //container.SetValue(JsonFunctions.PREFIX, new JsonFunctions(), true);
            context.PushGlobal(container);
            return context;
        }

        private void FillEntityInfo(Survey survey, SurveyUser user, SurveyCollector collector, SurveyMaint graph, TemplateContext context) {
            // This collector might be an anonymous collector without the RefNoteID, let's find the real collector
            if (collector.RefNoteID == null && survey.KeepAnswersAnonymous == true) {
                collector = SurveyCollector.UK.ByToken.Find(graph, collector.Token);//?? collector; 
            }
            if (collector.RefNoteID == null) {
                return;
            }
            var noteID = collector.RefNoteID;
            var eh = new EntityHelper(graph);
            var entityRow = eh.GetEntityRow(noteID);
            var entityType = entityRow.GetType();
            var entityName = eh.GetFriendlyEntityName(noteID);
            var fvp = eh.GetFieldValuePairs(entityRow, entityType);
            var desc = eh.GetEntityDescription(noteID, entityType);
            context.SetValue(new ScriptVariableGlobal(ENTITY_ROW), entityRow);
            context.SetValue(new ScriptVariableGlobal(ENTITY_TYPE), entityType);
            context.SetValue(new ScriptVariableGlobal(ENTITY_NAME), entityName);
            context.SetValue(new ScriptVariableGlobal(ENTITY_DESC), desc);
            context.SetValue(new ScriptVariableGlobal(ENTITY_FIELDS), fvp);
        }

        public string GetUrl(TemplateContext context, int? pageNbr) {
            var survey = (Survey)context.GetValue(new ScriptVariableGlobal(typeof(Survey).Name));
            var token = (string)context.GetValue(new ScriptVariableGlobal(TOKEN));
            return GetUrl(survey, token, pageNbr);
        }

        public string GetUrl(string token, int? pageNbr) {
            var (survey, _, answerCollector, _) = SurveyUtils.GetSurveyAndUser(graph, token);
            return GetUrl(survey, answerCollector.Token, pageNbr);
        }

        public string GetUrl(Survey survey, string token, int? pageNbr) {
            graph.Survey.Current = survey;
            string webHookUrl = GetUrl(survey);
            var pageParam = pageNbr.HasValue ? $"{SurveyWebhookServerHandler.PAGE_PARAM}={pageNbr}&" : "";
            var url = $"{webHookUrl}?{pageParam}{SurveyWebhookServerHandler.TOKEN_PARAM}={token}";
            return url;
        }

        private string GetUrl(Survey survey) {
            string webHookUrl = "";
            var setup = graph.SurveySetup.Current;
            if (setup.WebHookID.HasValue) {
                string str = (graph.CompanyService.IsMultiCompany ? PXAccess.GetCompanyName() : graph.CompanyService.GetSingleCompanyLoginName());
                string[] returnUrl = new string[] { _returnUrl, "/", str, "/", null };
                returnUrl[4] = setup.WebHookID.ToString();
                webHookUrl = string.Concat(returnUrl);
            }
            return webHookUrl;
            //Api.Webhooks.DAC.WebHook webHook = GetWebHook(survey);
            //return webHook.Url;
            //return survey.BaseURL;
        }

        //private Api.Webhooks.DAC.WebHook GetWebHook(Survey survey) {
        //    Api.Webhooks.Graph.WebhookMaint whGraph = PXGraph.CreateInstance<Api.Webhooks.Graph.WebhookMaint>();
        //    //Api.Webhooks.DAC.WebHook wh = PXSelect<Api.Webhooks.DAC.WebHook,
        //    //        Where<Api.Webhooks.DAC.WebHook.webHookID, Equal<Required<Api.Webhooks.DAC.WebHook.webHookID>>>>.Select(whGraph, survey.WebHookID);
        //    //whGraph.Webhook.Current = wh;
        //    whGraph.Webhook.Current = whGraph.Webhook.Search<Api.Webhooks.DAC.WebHook.webHookID>(survey.WebHookID);
        //    return whGraph.Webhook.Current;
        //}

        
        public static string MyMemberRenamerDelegate(MemberInfo member) {
            return member.Name;
        }

        public static bool MyMemberFilterDelegate(MemberInfo member) {
            return true;
        }


        public class Question {

            private readonly CSAttribute _attr;

            public Question(CSAttribute attr) {
                this._attr = attr;
            }

            //public string DefaultValue => _attr.DefaultValue;
            //public bool Required => _attr.Required == true;
            public string Description => _attr.Description;
            public string AttributeID => _attr.AttributeID;
            public string ID => _attr.AttributeID;
            public string Name => _attr.AttributeID;
            public int MaxLength { get; set; } = -1;
            public int ControlType => _attr.ControlType ?? 0;
            public IEnumerable<QuestionDetail> Details { get; set; } = Enumerable.Empty<QuestionDetail>();
            public bool HasDetails => Details != null && Details.Any();
        }

        public class QuestionDetail {
            public string ValueID { get; set; }
            public string Description { get; set; }
        }

        public class CSAttributePK : PrimaryKeyOf<CSAttribute>.By<CSAttribute.attributeID> {
            public static CSAttribute Find(PXGraph graph, string attributeID) => FindBy(graph, attributeID);
        }

        private Question GetQuestion(string attributeID) {
            var attr = CSAttributePK.Find(graph, attributeID);
            var question = new Question(attr);
            var controlType = attr.ControlType;
            var attrID = question.AttributeID;
            switch (controlType) {
                case 2: // Combo
                case 6: // Multi Select Combo
                    question.Details = SurveyUtils.
                        GetAttributeDetails(graph, attrID).
                        OrderBy(det => det.SortOrder ?? 0).
                        Select(x => new QuestionDetail() { ValueID = x.ValueID, Description = x.Description });
                    break;
                case 7: // Selector
                        // TODO
                    break;
            }
            return question;
        }
    }
}
