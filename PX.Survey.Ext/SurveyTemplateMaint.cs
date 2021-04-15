using PX.Data;
using System;

namespace PX.Survey.Ext {
    public class SurveyTemplateMaint : PXGraph<SurveyTemplateMaint, SurveyTemplate> {

        public PXSelect<SurveyTemplate> SUTemplate;

        [PXCopyPasteHiddenFields(new Type[] { typeof(SurveyTemplate.body) })]
        public PXSelect<SurveyTemplate, Where<SurveyTemplate.templateID, Equal<Current<SurveyTemplate.templateID>>>> CurrentSUTemplate;

        public SurveyTemplateMaint() {
        }
    }
}