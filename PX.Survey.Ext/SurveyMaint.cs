using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using System.Collections;
using System.Linq;

namespace PX.Survey.Ext {
    public class SurveyMaint : PXGraph<SurveyMaint, Survey> {

        public SelectFrom<Survey>.View Survey;

        [PXViewName(Objects.CR.Messages.Attributes)]
        public CSAttributeGroupList<Survey.surveyID, SurveyCollector> Questions;

        public SurveyDetailSelect Details;

        public SelectFrom<SurveyUser>.Where<SurveyUser.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View Users;
        public SelectFrom<SurveyCollector>.
            LeftJoin<SurveyUser>.
                On<SurveyUser.surveyID.IsEqual<SurveyCollector.surveyID>.
                And<SurveyUser.lineNbr.IsEqual<SurveyCollector.userLineNbr>>>.
            Where<SurveyCollector.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View Collectors;

        public SelectFrom<SurveyCollectorData>.
            Where<SurveyCollectorData.surveyID.IsEqual<Survey.surveyID.FromCurrent>.
            And<SurveyCollectorData.token.IsEqual<SurveyCollector.token.FromCurrent>>>.View CollectorDataRecords;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public PXSetup<SurveySetup> SurveySetup;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View SurveyCollector;

        public SurveyMaint() {
        }


        #region RecipientSelected Lookup
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

        #region TemplateSelected Lookup
        public PXFilter<TemplateFilter> templatefilter;
        [PXFilterable]
        [PXCopyPasteHiddenView]
        public TemplateLookup<TemplateSelected, TemplateFilter> templates;

        public PXAction<Survey> addTemplates;
        [PXUIField(DisplayName = "Add Templates", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable AddTemplates(PXAdapter adapter) {
            if (templatefilter.Current == null) {
                templatefilter.Current = templatefilter.Insert(new TemplateFilter());
            }
            if (templates.AskExt() == WebDialogResult.OK) {
                return AddSelectedTemplate(adapter);
            }
            recipients.Cache.Clear();
            return adapter.Get();
        }

        public PXAction<Survey> addSelectedTemplate;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable AddSelectedTemplate(PXAdapter adapter) {
            Details.Cache.ForceExceptionHandling = true;
            foreach (TemplateSelected template in templates.Cache.Cached) {
                if (template.Selected == true) {
                    var surveyDetail = new SurveyDetail();
                    surveyDetail.SurveyID = Survey.Current.SurveyID;
                    surveyDetail.TemplateID = template.TemplateID;
                    Details.Update(surveyDetail);
                }
            }
            templates.Cache.Clear();
            return adapter.Get();
        }
        #endregion

        protected virtual void _(Events.RowSelecting<Survey> e) {
            Survey row = e.Row;
            if (row == null) { return; }
            using (new PXConnectionScope()) {
                var collectors = SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<@P.AsInt>>.
                                        View.SelectWindowed(this, 0, 1, row.SurveyID);
                row.IsSurveyInUse = collectors.Any();
            }
        }

        protected virtual void _(Events.RowSelected<Survey> e) {
            var row = e.Row;
            if (row == null) { return; }
            bool unlockedSurvey = !(row.IsSurveyInUse == true);
            e.Cache.AllowDelete = unlockedSurvey;
            Questions.Cache.AllowUpdate = unlockedSurvey;
            Questions.Cache.AllowInsert = unlockedSurvey;
            Questions.Cache.AllowDelete = unlockedSurvey;
            PXUIFieldAttribute.SetEnabled<Survey.name>(e.Cache, row, unlockedSurvey);
            PXUIFieldAttribute.SetEnabled<Survey.surveyType>(e.Cache, row, unlockedSurvey);
        }

        public void _(Events.FieldUpdated<SurveyCollector, SurveyCollector.collectorID> e) {
            e.Cache.SetDefaultExt<SurveyCollector.token>(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<SurveyCollector, SurveyCollector.token> e) {
            var row = e.Row;
            if (row == null || row.CollectorID == null) {
                return;
            }
            e.NewValue = Net_Utils.ComputeMd5(row.CollectorID.ToString(), true);
            e.Cancel = true;
        }

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