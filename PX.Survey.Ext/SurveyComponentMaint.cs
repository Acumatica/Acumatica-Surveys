using PX.Data;
using System;

namespace PX.Survey.Ext {
    public class SurveyComponentMaint : PXGraph<SurveyComponentMaint, SurveyComponent> {

        public PXSelect<SurveyComponent> SUComponent;

        [PXCopyPasteHiddenFields(new Type[] { typeof(SurveyComponent.body) })]
        public PXSelect<SurveyComponent, Where<SurveyComponent.componentID, Equal<Current<SurveyComponent.componentID>>>> CurrentSUComponent;

        public SurveyComponentMaint() {
        }
    }
}