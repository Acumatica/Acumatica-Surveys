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
                    Where<Contact.contactType.IsEqual<ContactTypesAttribute.employee>.
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
            Survey row = e.Row;
            if (row == null) { return; }

            using (new PXConnectionScope())
            {
                var collectorData = SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<@P.AsInt>.
                                        And<SurveyCollector.collectorStatus.IsNotEqual<SurveyResponseStatus.CollectorNewStatus>>>.
                                        View.SelectWindowed(this, 0, 1, row.SurveyID).TopFirst;
                row.IsSurveyInUse = (collectorData != null); 
            }
        }

        protected virtual void _(Events.RowSelected<Survey> e)
        {
            Survey currentSurvey = e.Row;
            if (currentSurvey == null) { return; }

            bool unlockSurvey = !(currentSurvey.IsSurveyInUse.GetValueOrDefault(false));

            e.Cache.AllowDelete = unlockSurvey;
            Mapping.Cache.AllowUpdate = unlockSurvey;
            Mapping.Cache.AllowInsert = unlockSurvey;
            Mapping.Cache.AllowDelete = unlockSurvey;            
            PXUIFieldAttribute.SetEnabled<Survey.surveyName>(e.Cache, currentSurvey, unlockSurvey);
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