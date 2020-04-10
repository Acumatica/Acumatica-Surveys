using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;


namespace Covid19.Lib
{    
    public class SurveySetupMaint : PXGraph<SurveySetupMaint>
    {
        public PXSave<SurveySetup> Save;
        public PXCancel<SurveySetup> Cancel;
        public PXSelect<SurveySetup> SVSetup;
    }
}