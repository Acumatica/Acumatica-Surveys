using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Linq;

namespace PX.Survey.Ext {
    public class SurveyMaint : PXGraph<SurveyMaint, Survey> {

        public SelectFrom<Survey>.View Survey;

        [PXViewName(Objects.CR.Messages.Attributes)]
        public CSAttributeGroupList<Survey.surveyID, SurveyCollector> Mapping;

        public SelectFrom<SurveyUser>.Where<SurveyUser.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View Users;
        public SelectFrom<SurveyCollector>.
            InnerJoin<SurveyUser>.
            On<SurveyUser.userID.IsEqual<SurveyCollector.userID>>.
            Where<SurveyCollector.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View Collectors;

        public SelectFrom<SurveyCollectorData>.
            Where<SurveyCollectorData.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View CollectorDataRecords;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public PXSetup<SurveySetup> SurveySetup;

        //[PXCopyPasteHiddenView]
        //public SelectFrom<Contact>.
        //        Where<Contact.contactType.IsEqual<ContactTypesAttribute.employee>.
        //        And<Contact.isActive.IsEqual<True>>.
        //        And<Contact.userID.IsNotNull>>.
        //        OrderBy<Asc<Contact.displayName>>.View UsersForAddition;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View SurveyCollector;

        public SurveyMaint() {
            //SurveySetup Data = SurveySetup.Current;
            //UsersForAddition.Cache.AllowInsert = false;
            //UsersForAddition.Cache.AllowDelete = false;
        }

        //public PXAction<Survey> AddUsers;
        //[PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        //[PXButton(VisibleOnDataSource = false)]
        //public virtual IEnumerable addUsers(PXAdapter adapter) {
        //    var users = UsersForAddition.Select().Where(a => a.GetItem<Contact>().Selected == true).ToList();
        //    foreach (var user in users) {
        //        var surveyUser = new SurveyUser();
        //        surveyUser.Active = true;
        //        surveyUser.SurveyID = SurveyCurrent.Current.SurveyID;
        //        surveyUser.ContactID = user.GetItem<Contact>().ContactID;
        //        surveyUser = SurveyUsers.Insert(surveyUser);
        //    }
        //    return adapter.Get();
        //}

        //public PXAction<Survey> addRecipients;
        //[PXButton]
        //[PXUIField(DisplayName = "Add Recipients", MapViewRights = PXCacheRights.Select,
        //    MapEnableRights = PXCacheRights.Select)]
        //public virtual IEnumerable AddRecipients(PXAdapter adapter) {
        //    if (UsersForAddition.AskExt((graph, viewName) => {
        //        graph.Views[UsersForAddition.View.Name].Cache.Clear();
        //        graph.Views[viewName].Cache.Clear();
        //        graph.Views[viewName].Cache.ClearQueryCache();
        //        graph.Views[viewName].ClearDialog();
        //    }, true) != WebDialogResult.OK) return adapter.Get();
        //    return addUsers(adapter);
        //}


        #region SiteStatus Lookup
        public PXFilter<RecipientFilter> recipientfilter;
        [PXFilterable]
        [PXCopyPasteHiddenView]
        public RecipientLookup<RecipientSelected, RecipientFilter> recipients;

        public PXAction<Survey> addRecipients;
        [PXUIField(DisplayName = "Add Recipients", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable AddRecipients(PXAdapter adapter) {
            if (recipientfilter.Current == null) {
                recipientfilter.Current = recipientfilter.Insert(new RecipientFilter());
            }
            if (recipients.AskExt() == WebDialogResult.OK) {
                return AddSelectedRecipients(adapter);
            }
            recipients.Cache.Clear();
            return adapter.Get();
        }

        public PXAction<Survey> addSelectedRecipients;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable AddSelectedRecipients(PXAdapter adapter) {
            Users.Cache.ForceExceptionHandling = true;
            foreach (RecipientSelected recipient in recipients.Cache.Cached) {
                if (recipient.Selected == true) {
                    var surveyUser = new SurveyUser();
                    surveyUser.Active = true;
                    surveyUser.SurveyID = Survey.Current.SurveyID;
                    surveyUser.ContactID = recipient.ContactID;
                    Users.Update(surveyUser);
                }
            }
            recipients.Cache.Clear();
            return adapter.Get();
        }
        #endregion
        protected virtual void _(Events.RowSelecting<Survey> e) {
            Survey row = e.Row;
            if (row == null) { return; }
            using (new PXConnectionScope()) {
                var collectorData = SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<@P.AsInt>>.
                                        View.SelectWindowed(this, 0, 1, row.SurveyID).TopFirst;
                row.IsSurveyInUse = (collectorData != null);
            }
        }

        protected virtual void _(Events.RowSelected<Survey> e) {
            Survey currentSurvey = e.Row;
            if (currentSurvey == null) { return; }
            bool unlockSurvey = !(currentSurvey.IsSurveyInUse.GetValueOrDefault(false));
            e.Cache.AllowDelete = unlockSurvey;
            Mapping.Cache.AllowUpdate = unlockSurvey;
            Mapping.Cache.AllowInsert = unlockSurvey;
            Mapping.Cache.AllowDelete = unlockSurvey;
            PXUIFieldAttribute.SetEnabled<Survey.surveyName>(e.Cache, currentSurvey, unlockSurvey);
        }

        //public PXAction<Survey> render;
        //[PXUIField(DisplayName = "Render", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        //[PXButton]
        //public virtual IEnumerable Render(PXAdapter adapter) {
        //    Save.Press();
        //    var list = adapter.Get<Survey>().ToList();
        //    PXLongOperation.StartOperation(this, delegate () {
        //        var docgraph = CreateInstance<SurveyMaint>();
        //        foreach (var survey in list) {
        //            try {
        //                if (adapter.MassProcess) {
        //                    PXProcessing<Survey>.SetCurrentItem(survey);
        //                }
        //                docgraph.DoProcessSurvey(survey, adapter.MassProcess);
        //            } catch (Exception ex) {
        //                if (!adapter.MassProcess) {
        //                    throw;
        //                }
        //                PXProcessing<Survey>.SetError(ex);
        //            }
        //        }
        //    });
        //    return list;
        //}

        //public virtual void DoProcessSurvey(Survey survey, bool massProcess) {
        //    var generator = new SurveyGenerator();
        //    var surveySays = generator.GenerateSurvey(this, survey);
        //    survey.Rendered = surveySays;
        //    //survey.Status = state.Status;
        //    CurrentSurvey.Update(survey);
        //    Save.Press();
        //    CurrentSurvey.View.RequestRefresh();
        //}

        //[PXMergeAttributes(Method = MergeMethod.Append)]
        //[PXFormula(typeof(MobileAppDeviceOS<Contact.userID>))]
        //[PXDependsOnFields(typeof(Contact.contactID), typeof(Contact.userID))]
        //[PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "Visibility", PXUIVisibility.SelectorVisible)]
        //protected virtual void Contact_UsrMobileAppDeviceOS_CacheAttached(PXCache sender) { }

        //[PXMergeAttributes(Method = MergeMethod.Append)]
        //[PXFormula(typeof(IIf<Where<ContactSurveyExt.usrMobileAppDeviceOS, IsNull>, False, True>))]
        //[PXDependsOnFields(typeof(ContactSurveyExt.usrMobileAppDeviceOS))]
        //[PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "Visibility", PXUIVisibility.SelectorVisible)]
        //protected virtual void Contact_UsrUsingMobileApp_CacheAttached(PXCache sender) { }

        [PXMergeAttributes]
        [PXParent(typeof(Select<Survey, Where<Survey.surveyID, Equal<Current<CSAttributeGroup.entityClassID>>>>), LeaveChildren = true)]
        [PXDBDefault(typeof(Survey.surveyIDStringID))]
        protected virtual void _(Events.CacheAttached<CSAttributeGroup.entityClassID> e) { }
    }
}