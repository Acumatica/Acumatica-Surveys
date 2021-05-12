using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using System.Collections;
using System.Linq;
using System.Text;

namespace PX.Survey.Ext {
    public class SurveyMaint : PXGraph<SurveyMaint, Survey> {

        public SelectFrom<Survey>.View Survey;

        //[PXViewName(Objects.CR.Messages.Attributes)]
        //public CSAttributeGroupList<Survey.surveyID, SurveyCollector> Questions;

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
        [PXUIField(DisplayName = "Add Pages", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
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
                    surveyDetail.TemplateType = template.TemplateType;
                    surveyDetail.Description = template.Description;
                    surveyDetail.TemplateID = template.TemplateID;
                    Details.Update(surveyDetail);
                }
            }
            templates.Cache.Clear();
            return adapter.Get();
        }
        #endregion

        public PXAction<Survey> generateSample;
        [PXUIField(DisplayName = "Generate Sample", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable GenerateSample(PXAdapter adapter) {
            var list = adapter.Get<Survey>().ToList();
            Save.Press();
            var graph = CreateInstance<SurveyMaint>();
            foreach (var survey in list) {
                var row = PXCache<Survey>.CreateCopy(survey);
                var user = graph.GenerateSampleUser(row);
                graph.DoGenerateSample(row, user);
            }
            return adapter.Get();
        }

        private void DoGenerateSample(Survey survey, SurveyUser user) {
            var pages = GetPageNumbers(survey);
            var generator = new SurveyGenerator();
            foreach (var pageNbr in pages) {
                var pageContent = generator.GenerateSurveyPage(survey, user, pageNbr);
                SaveContentToAttachment($"Survey-{survey.SurveyID.Value}-Page-{pageNbr}.html", pageContent);
            }
            Actions.PressSave();
        }

        public int[] GetPageNumbers(Survey survey) {
            Survey.Current = survey;
            var details = Details.Select().
                FirstTableItems.
                Where(pd => pd.PageNbr != null && pd.PageNbr > 0);
            var pages = details.
                Select(sd => sd.PageNbr.Value).
                Distinct().OrderBy(pageNbr => pageNbr).ToArray();
            return pages;
        }

        private void SaveContentToAttachment(string name, string content) {
            var bytes = Encoding.UTF8.GetBytes(content);
            var file = new SM.FileInfo(name, null, bytes);
            var fm = CreateInstance<SM.UploadFileMaintenance>();
            fm.SaveFile(file, SM.FileExistsAction.CreateVersion);
            PXNoteAttribute.SetFileNotes(Survey.Cache, Survey.Current, file.UID.Value);
        }

        private SurveyUser GenerateSampleUser(Survey survey) {
            var setup = SurveySetup.Current;
            if (!setup.ContactID.HasValue) {
                throw new PXSetPropertyException(Messages.ContactNotSetup, Messages.SUSetup);
            }
            var user = new SurveyUser() {
                Active = true,
                ContactID = setup.ContactID,
                SurveyID = survey.SurveyID
            };
            return user;
        }

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
            //Questions.Cache.AllowUpdate = unlockedSurvey;
            //Questions.Cache.AllowInsert = unlockedSurvey;
            //Questions.Cache.AllowDelete = unlockedSurvey;
            //PXUIFieldAttribute.SetEnabled<Survey.name>(e.Cache, row, unlockedSurvey);
            PXUIFieldAttribute.SetEnabled<Survey.target>(e.Cache, row, unlockedSurvey);
            PXUIFieldAttribute.SetEnabled<Survey.layout>(e.Cache, row, unlockedSurvey);
        }

        protected virtual void _(Events.RowSelected<SurveyDetail> e) {
            var row = e.Row;
            if (row == null || Survey.Current == null || Survey.Current.Layout == null) {
                return;
            }
            var isMulti = Survey.Current.Layout == SurveyLayout.MultiPage;
            PXUIFieldAttribute.SetEnabled<SurveyDetail.pageNbr>(e.Cache, row, isMulti);
        }

        protected virtual void _(Events.FieldUpdated<SurveyDetail, SurveyDetail.templateID> e) {
            e.Cache.SetDefaultExt<SurveyDetail.description>(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.description> e) {
            var row = e.Row;
            if (row == null || row.TemplateID == null) {
                return;
            }
            SurveyTemplate st = SurveyTemplate.PK.Find(this, row.TemplateID);
            e.NewValue = st.Description;
            e.Cancel = e.NewValue != null;
        }

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.pageNbr> e) {
            var row = e.Row;
            if (row == null || Survey.Current == null || Survey.Current.Layout == null) {
                return;
            }
            var isMulti = Survey.Current.Layout == SurveyLayout.MultiPage;
            if (isMulti) { 
                e.NewValue = GetMaxPage(Survey.Current.SurveyID) + 1;
            } else {
                e.NewValue = 1;
            }
            e.Cancel = true;
        }

        private int GetMaxPage(int? surveyID) {
            var maxPage = PXSelect<SurveyDetail, Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>>>.Select(this, surveyID).FirstTableItems.Select(sd => sd.PageNbr).Max();
            return maxPage ?? 0;
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