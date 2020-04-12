using System;
using System.Collections;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Survey.Ext
{
    public class SurveyMaint : PXGraph<SurveyMaint, Survey>
    {
        public SelectFrom<Survey>.View SurveyCurrent;

        [PXViewName(PX.Objects.CR.Messages.Attributes)]
        public CSAttributeGroupList<Survey.surveyID, SurveyCollector> Mapping;

        public SelectFrom<SurveyUser>.Where<SurveyUser.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View SurveyUsers;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public PXSetup<SurveySetup> SurveySetup;

        [PXCopyPasteHiddenView]
        public SelectFrom<Contact>.
                    Where<Contact.contactType.IsIn<ContactTypesAttribute.employee, ContactTypesAttribute.person>.
                        And<Contact.isActive.IsEqual<True>>.
                        And<Contact.userID.IsNotNull>>.OrderBy<Asc<Contact.displayName>>.View UsersForAddition;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View SurveyCollector;


        public SurveyMaint()
        {
            SurveySetup Data = SurveySetup.Current;

            UsersForAddition.Cache.AllowInsert = false;
            UsersForAddition.Cache.AllowDelete = false;
        }

        public PXAction<Survey> CreateSurvey;
        [PXButton]
        [PXUIField(DisplayName = "Create Survey", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable createSurvey(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, delegate ()
            {
                CreateSurveys();
            });

            return adapter.Get();
        }

        private void CreateSurveys()
        {
            var activeUsers = SurveyUsers.Select().Where(a => a.GetItem<SurveyUser>().Active == true).ToList();
            var alreadyInserted = SurveyCollector.Select().ToList().Select(a => a.GetItem<SurveyCollector>().UserID).ToList();
            var allUsers = activeUsers.Select(a => a.GetItem<SurveyUser>().UserID).ToList();
            var idsForInsertion = allUsers.Except(alreadyInserted);

            activeUsers = activeUsers.Where(u => idsForInsertion.Contains(u.GetItem<SurveyUser>().UserID)).ToList();

            foreach (PXResult<SurveyUser> activeUser in activeUsers)
            {
                var user = activeUser.GetItem<SurveyUser>();
                var collector = SurveyCollector.Insert(new SurveyCollector());
                collector.CollectorName = String.Format("{0} {1}", SurveyCurrent.Current.SurveyName, PXTimeZoneInfo.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                collector.SurveyID = SurveyCurrent.Current.SurveyID;
                collector.UserID = user.UserID;
                collector.CollectedDate = null;
                collector.ExpirationDate = null;
                collector.CollectorStatus = "N";
                collector = SurveyCollector.Update(collector);
            }

            this.Persist();
        }

        public PXAction<Survey> ClearSurvey;

        [PXButton]
        [PXUIField(DisplayName = "Clear Survey",
                   MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable clearSurvey(PXAdapter adapter)
        {
            if (SurveyCurrent.Ask(Messages.Delete, Messages.AreYouSure, Messages.AreYouSureYouWantToDeleteAllNewSurveys,
                    MessageButtons.YesNo) != WebDialogResult.Yes) { return adapter.Get(); }

            PXLongOperation.StartOperation(this, delegate ()
            {
                ClearSurveys();
            });
            return adapter.Get();
        }

        private void ClearSurveys()
        {
            var newSurveys = SurveyCollector.Select().Where(s => s.GetItem<SurveyCollector>().CollectorStatus == "N")
                .ToList();

            foreach (var newSurvey in newSurveys)
            {
                SurveyCollector.Delete(newSurvey);
            }

            Persist();
        }

        public PXAction<Survey> AddUsers;

        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(VisibleOnDataSource = false)]
        public virtual IEnumerable addUsers(PXAdapter adapter)
        {
            var users = UsersForAddition.Select().Where(a => a.GetItem<Contact>().Selected == true).ToList();

            foreach (var user in users)
            {
                var surveyUser = new SurveyUser();
                surveyUser.Active = true;
                surveyUser.SurveyID = SurveyCurrent.Current.SurveyID;
                surveyUser.ContactID = user.GetItem<Contact>().ContactID;
                surveyUser = SurveyUsers.Insert(surveyUser);
            }

            return adapter.Get();
        }

        public PXAction<Survey> AddRecipients;

        [PXButton]
        [PXUIField(DisplayName = "Add Recipients", MapViewRights = PXCacheRights.Select,
            MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable addRecipients(PXAdapter adapter)
        {
            if (UsersForAddition.AskExt((graph, viewName) =>
            {
                graph.Views[UsersForAddition.View.Name].Cache.Clear();
                graph.Views[viewName].Cache.Clear();
                graph.Views[viewName].Cache.ClearQueryCache();
                graph.Views[viewName].ClearDialog();
            }, true) != WebDialogResult.OK) return adapter.Get();

            return addUsers(adapter);
        }

        protected virtual void _(Events.RowSelecting<Survey> e)
        {
            var row = e.Row;
            if (row == null)
                return;

            using (new PXConnectionScope())
            {
                var firstCollector =
                    SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<@P.AsInt>.
                        And<SurveyCollector.collectorStatus.
                            IsNotEqual<SurveyResponseStatus.CollectorNewStatus>>>.View.SelectWindowed(this, 0, 1,
                        row.SurveyID).TopFirst;
                if (firstCollector != null && firstCollector.CollectorStatus != SurveyResponseStatus.CollectorNew)
                {
                    row.NotNew = true;
                }
                else
                {
                    row.NotNew = false;
                }
            }
        }

        protected virtual void _(Events.RowSelected<Survey> e)
        {
            var selectedSurvey = e.Row;
            if (selectedSurvey == null)
                return;

            var status = !selectedSurvey.NotNew == true;

            Mapping.Cache.AllowUpdate = status;
            Mapping.Cache.AllowInsert = status;
            Mapping.Cache.AllowDelete = status;
            PXUIFieldAttribute.SetEnabled<Survey.surveyName>(e.Cache, selectedSurvey, status);
            PXUIFieldAttribute.SetEnabled<Survey.surveyDesc>(e.Cache, selectedSurvey, status);

            
            CreateSurvey.SetEnabled(selectedSurvey.Active != false);

        }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [MobileAppEnabled(typeof(Contact.userID))]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "Visibility", PXUIVisibility.SelectorVisible)]
        protected virtual void _(Events.CacheAttached<ContactSurveyExt.usrUsingMobileApp> e) { }

        [PXMergeAttributes]
        [PXParent(typeof(Select<Survey, Where<Survey.surveyID, Equal<Current<CSAttributeGroup.entityClassID>>>>), LeaveChildren = true)]
        [PXDBLiteDefault(typeof(Survey.surveyIDStringID))]
        protected virtual void _(Events.CacheAttached<CSAttributeGroup.entityClassID> e) { }
    }
}