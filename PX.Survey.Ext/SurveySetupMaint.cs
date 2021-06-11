using PX.Data;

namespace PX.Survey.Ext {
    public class SurveySetupMaint : PXGraph<SurveySetupMaint> {

        public PXSave<SurveySetup> Save;
        public PXCancel<SurveySetup> Cancel;
        public PXSelect<SurveySetup> surveySetup;
        public PXSelect<SurveySetupEntity> DefaultSurveys;

    }
}