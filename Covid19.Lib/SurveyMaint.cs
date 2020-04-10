using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;

namespace Covid19.Lib
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Todo:   I suspect this is a redundant graph where SurveyQuizSettings seems to have the bulk of the work
    ///         Im going to assume that we want to not use the word Quiz in the name.   
    /// </remarks>
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