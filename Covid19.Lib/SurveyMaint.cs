using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;

namespace Covid19.Lib
{
    public class SurveyMaint : PXGraph<SurveyMaint>
    {
        public SelectFrom<Surveys>.View SurveyList;

        public PXSave<Surveys> Save;
        public PXCancel<Surveys> Cancel;

        public PXSetup<CRSetup> SurveySetup;

        public SurveyMaint()
        {
            CRSetup Data = SurveySetup.Current;
        }
    }
}