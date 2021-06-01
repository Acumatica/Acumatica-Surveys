﻿using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace PX.Survey.Ext {
    public class SurveyGenerator {

        private SurveyMaint graph;

        private static string INNER_CONTENT = "InnerContent";
        private static string INNER_CONTENT_LIST = INNER_CONTENT + "List";
        private static string FIRST_PAGE_NBR = "FirstPageNbr";
        private static string LAST_PAGE_NBR = "LastPageNbr";
        private static string NB_PAGES = "Nbpages";
        private static string IS_FIRST_PAGE = "IsFirstPage";
        private static string IS_LAST_PAGE = "IsLastPage";

        public SurveyGenerator() : this(PXGraph.CreateInstance<SurveyMaint>()) {
        }

        public SurveyGenerator(SurveyMaint graph) {
            this.graph = graph;
        }

        public string GenerateBadRequestPage(string message) {
            var setup = graph.SurveySetup.Current;
            var pageTemplate = SurveyTemplate.PK.Find(graph, setup.TemplateID);
            var template = Template.Parse(pageTemplate.Body);
            var context = new TemplateContext();
            var container = new ScriptObject {
                {setup.GetType().Name, setup},
                {"Message", message},
            };
            //container.SetValue(AcuFunctions.PREFIX, new AcuFunctions(), true);
            //container.SetValue(JsonFunctions.PREFIX, new JsonFunctions(), true);
            context.PushGlobal(container);
            var rendered = template.Render(context);
            return rendered;
        }

        public string GenerateSurveyPage(string token, int pageNbr) {
            (var survey, var user) = SurveyUtils.GetSurveyAndUser(graph, token);
            return GenerateSurveyPage(survey, user, pageNbr);
        }

        public string GenerateSurveyPage(Survey survey, SurveyUser user, int pageNbr) {
            graph.Survey.Current = survey;
            var mainTemplateID = survey.TemplateID;
            var mainTemplate = SurveyTemplate.PK.Find(graph, mainTemplateID);
            string mainTemplateText = mainTemplate.Body;
            if (string.IsNullOrEmpty(mainTemplateText?.Trim())) {
                throw new PXException(Messages.TemplateNeeded);
            }
            var template = Template.Parse(mainTemplateText);
            var mainContext = GetSurveyContext(survey, user);
            var renderedPage = GetRenderedPage(survey, user, mainContext, pageNbr);
            FillRenderedPages(mainContext, renderedPage);
            var rendered = template.Render(mainContext);
            return rendered;
        }

        private void FillRenderedPages(TemplateContext context, IEnumerable<string> renderedPage) {
            var container = new ScriptObject {
                {INNER_CONTENT_LIST, renderedPage},
                {INNER_CONTENT, string.Join("\n", renderedPage)},
            };
            context.PushGlobal(container);
        }

        private TemplateContext GetSurveyContext(Survey survey, SurveyUser user) {
            var setup = graph.SurveySetup.Current;
            var context = new TemplateContext();
            var webHook = PXSelect<Api.Webhooks.DAC.WebHook,
                Where<Api.Webhooks.DAC.WebHook.webHookID,
                Equal<Required<Api.Webhooks.DAC.WebHook.webHookID>>>>.Select(graph, survey.WebHookID);
            var container = new ScriptObject {
                {survey.GetType().Name, survey},
                {setup.GetType().Name, setup},
                {user.GetType().Name, user},
                {webHook.GetType().Name, webHook},
            };
            //container.SetValue(AcuFunctions.PREFIX, new AcuFunctions(), true);
            //container.SetValue(JsonFunctions.PREFIX, new JsonFunctions(), true);
            context.PushGlobal(container);
            return context;
        }

        private IEnumerable<string> GetRenderedPage(Survey survey, SurveyUser user, TemplateContext context, int pageNbr) {
            graph.Survey.Current = survey;
            var activePages = graph.Details.Select().FirstTableItems.Where(det => det.Active == true);
            var selectedPages = SurveyUtils.SelectPages(survey, activePages, pageNbr);
            var allRendered = new List<string>();
            foreach (var selectedPage in selectedPages) {
                var pageTemplateID = selectedPage.TemplateID;
                var pageTemplate = SurveyTemplate.PK.Find(graph, pageTemplateID);
                var template = Template.Parse(pageTemplate.Body);
                AddDetailContext(context, selectedPage, pageTemplate);
                FillPageInfo(context, activePages, selectedPage);
                var rendered = template.Render(context);
                allRendered.Add(rendered);
            }
            return allRendered;
        }

        //private TemplateContext GetContext(Survey survey, SurveyUser user, IEnumerable<SurveyDetail> questions, int pageNbr) {
        //    var context = new TemplateContext();
        //    var container = new ScriptObject {
        //        {"Survey", survey},
        //        {"User", user},
        //        {"Questions", GetQuestions(questions)},
        //    };
        //    //container.SetValue(AcuFunctions.PREFIX, new AcuFunctions(), true);
        //    //container.SetValue(JsonFunctions.PREFIX, new JsonFunctions(), true);
        //    context.PushGlobal(container);
        //    return context;
        //}

        private void AddDetailContext(TemplateContext context, SurveyDetail detail, SurveyTemplate template) {
            var container = new ScriptObject {
                {detail.GetType().Name, detail},
                {template.GetType().Name, template},
            };
            if (detail.IsQuestion == true) {
                var question = GetQuestion(detail.AttributeID);
                container.Add(question.GetType().Name, question);
            }
            //container.SetValue(AcuFunctions.PREFIX, new AcuFunctions(), true);
            //container.SetValue(JsonFunctions.PREFIX, new JsonFunctions(), true);
            context.PushGlobal(container);
        }

        //public class Question {

            //    private readonly CSAttributeGroup _attrGroup;

            //    public Question(CSAttributeGroup attrGroup) {
            //        this._attrGroup = attrGroup;
            //    }

            //    public string DefaultValue => _attrGroup.DefaultValue;
            //    public bool Required => _attrGroup.Required == true;
            //    public string Description => _attrGroup.Description;
            //    public string AttributeID => _attrGroup.AttributeID;
            //    public string ID => _attrGroup.AttributeID;
            //    public string Name => _attrGroup.AttributeID;
            //    public int MaxLength { get; set; } = -1;
            //    public int ControlType => _attrGroup.ControlType ?? 0;
            //    public IEnumerable<QuestionDetail> Details { get; set; } = Enumerable.Empty<QuestionDetail>();
            //    public bool HasDetails => Details != null && Details.Any();
            //}
        private void FillPageInfo(TemplateContext context, IEnumerable<SurveyDetail> details, SurveyDetail selectedPage) {
            var nbPages = details.Select(det => det.PageNbr).Distinct().Count();
            var min = details.Min(det => det.PageNbr);
            var max = details.Max(det => det.PageNbr);
            var current = selectedPage.PageNbr;
            context.SetValue(new ScriptVariableGlobal(FIRST_PAGE_NBR), min);
            context.SetValue(new ScriptVariableGlobal(LAST_PAGE_NBR), max);
            context.SetValue(new ScriptVariableGlobal(NB_PAGES), nbPages);
            context.SetValue(new ScriptVariableGlobal(IS_FIRST_PAGE), current == min);
            context.SetValue(new ScriptVariableGlobal(IS_LAST_PAGE), current == max);
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

        //private IList<Question> GetQuestions(IEnumerable<CSAttributeGroup> attrGroups) {
        //    var questions = new List<Question>();
        //    var selectedGroups = attrGroups.Where(qu => qu.IsActive == true).OrderBy(qu => qu.SortOrder ?? 0);
        //    foreach (var selectedGroup in selectedGroups) {
        //        Question question = GetQuestion(selectedGroup);
        //        questions.Add(question);
        //    }
        //    return questions;
        //}

        //private Question GetQuestion(CSAttributeGroup selectedGroup) {
        //    var question = new Question(selectedGroup);
        //    var controlType = selectedGroup.ControlType;
        //    var attrID = question.AttributeID;
        //    switch (controlType) {
        //        case 2: // Combo
        //        case 6: // Multi Select Combo
        //            question.Details = SurveyUtils.
        //                GetAttributeDetails(graph, attrID).
        //                OrderBy(det => det.SortOrder ?? 0).
        //                Select(x => new QuestionDetail() { ValueID = x.ValueID, Description = x.Description });
        //            break;
        //        case 7: // Selector
        //                // TODO
        //            break;
        //    }

        //    return question;
        //}

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
