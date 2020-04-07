using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Data.BQL.Fluent;

namespace Covid19.Lib
{
    public class SurveyCollectionMaint : PXGraph<SurveyCollectionMaint, SurveyCollector>
    {

        public PXSelect<SurveyCollector> Collections;

        public void AssignSetStatus(SurveyCollector c, bool isMassProcess = false)
        {
            Collections.Current = c;
            if (c.CollectorStatus != "N")
            {
                throw new PXException(String.Format(
                "Survey Collection Item {0} is not New.", c.CollectorStatus));
            }            
            c.CollectorStatus = "S";
            Collections.Update(c);
            Persist();
            if (isMassProcess)
            {
                PXProcessing.SetInfo(String.Format(
                "Survey  {0} has been successfully updated.", c.CollectorID));
            }
        }
        #region dead code
        //public SelectFrom<Surveys>.View CurrentSurvey;        
        //public PXSelectJoin<SurveyRecipients, 
        //    InnerJoin<Users,On<SurveyRecipients.userID,Equal<Users.pKID>>,
        //    InnerJoin<EPEmployee,On<SurveyRecipients.userID,Equal<EPEmployee.userID>>,
        //        InnerJoin<CRContact,On<EPEmployee.defContactID,Equal<CRContact.contactID>>>>>,
        //    Where<SurveyRecipients.surveyID, Equal<Current<Surveys.surveyID>>>> Recipients;

        //public SelectFrom<SurveyCollector>.View SurveyCollections;
        //#endregion

        //#region actions
        //#region ActionsMenu
        //public PXAction<Surveys> ActionsMenu;
        //[PXButton(SpecialType = PXSpecialButtonType.ActionsFolder, MenuAutoOpen = true, CommitChanges = true)]
        //[PXUIField(DisplayName = "Actions")]
        //protected virtual IEnumerable actionsMenu(PXAdapter adapter)
        //{
        //    return adapter.Get();
        //}
        //#endregion
        //#region CreateSurveys
        //public PXAction<Surveys> createSurveys;
        //[PXUIField(DisplayName = "Create Surveys", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        //[PXButton()]
        //public virtual IEnumerable CreateSurveys(PXAdapter adapter)
        //{
        //    Surveys currentSurvery = this.CurrentSurvey.Current;

        //    foreach(SurveyRecipients r in this.Recipients.Select())
        //    {
        //        if (r.Selected == true)
        //        {
        //            // push to survey collector table
        //            SurveyCollector collector = new SurveyCollector();
        //            collector.CollectorStatus = "Open";
        //            collector.SurveyID = currentSurvery.SurveyID;
        //            //collector.Userid = r.UserID;

        //            this.SurveyCollections.Insert(collector);
        //        }
        //    }

        //    this.Actions.PressSave();

        //    return adapter.Get();
        //}
        //#endregion
        //#endregion

        //#region cntr
        //public SurveyCollectionMaint()
        //{
        //    this.ActionsMenu.AddMenuAction(createSurveys);

        //}
        //#endregion


        //#region events
        //[PXSelector(typeof(Search<Surveys.surveyID>),
        //    typeof(Surveys.surveyName),
        //    typeof(Surveys.surveyDesc),
        //    SubstituteKey = typeof(Surveys.surveyName))]
        //[PXDBInt(IsKey = true)]
        //[PXUIField(DisplayName = "Survey ID")]
        //protected void _(Events.CacheAttached<Surveys.surveyID> e)
        //{            
        //}
        #endregion


    }
}