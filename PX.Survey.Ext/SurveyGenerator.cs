using PX.Data;
using Scriban;
using Scriban.Runtime;
using System.Collections.Generic;

namespace PX.Survey.Ext {
    public class SurveyGenerator {

        private SurveyMaint graph;

        public SurveyGenerator() : this(PXGraph.CreateInstance<SurveyMaint>()) {
        }

        public SurveyGenerator(SurveyMaint graph) {
            this.graph = graph;
        }

        public string GenerateSurvey(Survey survey, SurveyUser user) {
            graph.CurrentSurvey.Current = survey;
            var questions = graph.Mapping.Select().FirstTableItems;
            string templateText = survey.Template;
            if (string.IsNullOrEmpty(templateText?.Trim())) {
                throw new PXException(Messages.TemplateNeeded);
            }
            var template = Template.Parse(templateText);
            var context = GetContext(survey, user, questions);
            var rendered = template.Render(context);
            return rendered;
        }

        private TemplateContext GetContext(Survey survey, SurveyUser user, IEnumerable<Objects.CS.CSAttributeGroup> questions) {
            var context = new TemplateContext();
            var container = new ScriptObject {
                {"Survey", survey},
                {"User", user},
                {"Questions", questions},
            };
            //container.SetValue(AcuFunctions.PREFIX, new AcuFunctions(), true);
            //container.SetValue(JsonFunctions.PREFIX, new JsonFunctions(), true);
            context.PushGlobal(container);
            return context;
        }
    }
}
