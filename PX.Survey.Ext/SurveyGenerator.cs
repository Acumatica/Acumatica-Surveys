using PX.Data;
using PX.Objects.CS;
using Scriban;
using Scriban.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace PX.Survey.Ext {
    public class SurveyGenerator {

        private SurveyMaint graph;

        public SurveyGenerator() : this(PXGraph.CreateInstance<SurveyMaint>()) {
        }

        public SurveyGenerator(SurveyMaint graph) {
            this.graph = graph;
        }

        public string GenerateSurvey(Survey survey, SurveyUser user) {
            graph.Survey.Current = survey;
            var questions = graph.Questions.Select().FirstTableItems;
            string templateText = survey.Template;
            if (string.IsNullOrEmpty(templateText?.Trim())) {
                throw new PXException(Messages.TemplateNeeded);
            }
            var template = Template.Parse(templateText);
            var context = GetContext(survey, user, questions);
            var rendered = template.Render(context);
            return rendered;
        }

        private TemplateContext GetContext(Survey survey, SurveyUser user, IEnumerable<CSAttributeGroup> questions) {
            var context = new TemplateContext();
            var container = new ScriptObject {
                {"Survey", survey},
                {"User", user},
                {"Questions", GetQuestions(questions)},
            };
            //container.SetValue(AcuFunctions.PREFIX, new AcuFunctions(), true);
            //container.SetValue(JsonFunctions.PREFIX, new JsonFunctions(), true);
            context.PushGlobal(container);
            return context;
        }

        public class Question {

            private CSAttributeGroup _attrGroup;

            public Question(CSAttributeGroup attrGroup) {
                this._attrGroup = attrGroup;
            }

            public string DefaultValue => _attrGroup.DefaultValue;
            public bool Required => _attrGroup.Required == true;
            public string Description => _attrGroup.Description;
            public string AttributeID => _attrGroup.AttributeID;
            public int ControlType => _attrGroup.ControlType ?? 0;
            public IEnumerable<QuestionDetail> Details { get; set; }
            public bool HasDetails => Details != null && Details.Any();
        }

        public class QuestionDetail {
            public string ValueID { get; set; }
            public string Description { get; set; }
        }

        private IList<Question> GetQuestions(IEnumerable<CSAttributeGroup> attrGroups) {
            var questions = new List<Question>();
            var selectedGroups = attrGroups.Where(qu => qu.IsActive == true).OrderBy(qu => qu.SortOrder ?? 0);
            foreach (var selectedGroup in selectedGroups) {
                var question = new Question(selectedGroup);
                var controlType = selectedGroup.ControlType;
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
                        break;
                }
                questions.Add(question);
            }
            return questions;
        }
    }
}
