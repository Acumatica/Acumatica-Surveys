using System;
using PX.Data;
using PX.Api.Mobile.PushNotifications;
using PX.Data.Automation;
using PX.Api.Mobile.PushNotifications.DAC;

using Microsoft.Practices.ServiceLocation;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using PX.Objects.CR;
using PX.SM;

namespace Covid19.Lib
{
    //DHIREN'S NOTE - REQUIRES CLEAN UP
    public class SurveyCollectionMaint : PXGraph<SurveyCollectionMaint, SurveyCollector>
    {

        public PXSelectJoin<SurveyCollector,
            InnerJoin<Contact,On<SurveyCollector.userid,Equal<Contact.userID>>>> Collections;

        
        
        public void AssignSetStatus(SurveyCollector c,  bool isMassProcess = false)
        {
            Collections.Current = c;
            if (c.CollectorStatus != "N")
            {
                throw new PXException(String.Format(
                "Survey Collection Item {0} status is not New.", c.CollectorStatus));
            }            
            c.CollectorStatus = "S";
            Collections.Update(c);
            Persist();

            //now send out the SMS push notification

            if (Collections.Cache.GetStatus(Collections.Current) == PXEntryStatus.Inserted ||
                Collections.Cache.GetStatus(Collections.Current) == PXEntryStatus.InsertedDeleted) 
            {
                //for now don't do anything
            }
            else
            {
                var pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();

                List<Guid> userIds = new List<Guid>();
                //Users u = PXSelect<Users, Where<Users.contactID, Equal<Required<Users.contactID>>>>.Select(this, c.ContactID);

                userIds.Add(c.Userid.Value);
                //Check if User is using Acumatica Mobile App
                //var activeTokens = pushNotificationSender.CountActiveTokens(userIds);

                MobileDevice device = PXSelect<MobileDevice,
                    Where<MobileDevice.userID, Equal<Required<MobileDevice.userID>>>>.Select(this, c.Userid.Value);
                if (device == null)
                {
                    throw new PXException("User has no mobile device");
                }

                //string surveyID = c.SurveyID;
                //string sScreenID = Accessinfo.ScreenID.Replace(".", "");
                string sScreenID = "CV301010";
                Guid noteID = c.NoteID.Value;

                PXLongOperation.StartOperation(this, () =>
                {
                    try
                    {
                        pushNotificationSender.SendNotificationAsync(
                                            userIds: userIds,
                                            // Push Notification Title
                                            //title: Messages.PushNotificationTitle,
                                            title: "Complete Survey",
                                            // Push Notification Message Body
                                            //text: $"{ Messages.PushNotificationMessageBody } { sScreenID }.",
                                            text: $"{ "" } { sScreenID }.",
                                            // Link to Screen to open upon tap with Sales Order data associated to NoteID
                                            link: (sScreenID, noteID),
                                            cancellation: CancellationToken.None);
                    }
                    catch (AggregateException ex)
                    {
                        var message = string.Join(";", ex.InnerExceptions.Select(e => e.Message));
                        throw new InvalidOperationException(message);
                    }
                });


            }

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