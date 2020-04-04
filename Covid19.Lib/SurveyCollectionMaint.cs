using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.AP;

namespace Covid19.Lib
{
    public class SurveyCollectionMaint : PXGraph<SurveyCollectionMaint>
    {
        #region views
        public PXCancel<Surveys> Cancel;
        public PXSelect<Surveys> CurrentSurvey;
        public PXSelect<EPEmployee, Where<EPEmployee.status, Equal<EPEmployee.status.active>>> Employees;
        #endregion

        #region actions
        public PXAction<Surveys> createSurveys;
        [PXUIField(DisplayName = "Create Surveys", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton()]
        public virtual IEnumerable CreateSurveys(PXAdapter adapter)
        {
            return adapter.Get();
        }
        #endregion

        #region events
        [PXSelector(typeof(Search<Surveys.surveyID>),
            typeof(Surveys.surveyName),
            typeof(Surveys.surveyDesc),
            SubstituteKey = typeof(Surveys.surveyName))]
        protected void _(Events.CacheAttached<Surveys.surveyID> e)
        {            
        }
        #endregion


    }
}