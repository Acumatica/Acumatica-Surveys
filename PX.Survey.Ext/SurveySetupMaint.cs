using PX.Data;

namespace PX.Survey.Ext {
    public class SurveySetupMaint : PXGraph<SurveySetupMaint> {
        #region views
        public PXSave<SurveySetup> Save;
        public PXCancel<SurveySetup> Cancel;
        public PXSelect<SurveySetup> surveySetup;
        #endregion
    }
}