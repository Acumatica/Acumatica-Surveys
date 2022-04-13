using PX.Data;

namespace PX.Survey.Ext {
    public class SurveyComponentMaint : PXGraph<SurveyComponentMaint, SurveyComponent> {

        public PXSelect<SurveyComponent> SUComponent;
        public PXSelect<SurveyComponent, Where<SurveyComponent.componentID, Equal<Current<SurveyComponent.componentID>>>> CurrentSUComponent;

        public SurveyComponentMaint() {}
    }
}