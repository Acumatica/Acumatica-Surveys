using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PX.Survey.Ext {
    public class SurveyMaint : PXGraph<SurveyMaint, Survey> {

        //[PXCopyPasteHiddenFields(typeof(Survey.surveyID))]
        public SelectFrom<Survey>.View Survey;

        public SurveyDetailSelect Details;
        public PXSelect<SurveyDetail,
            Where<SurveyDetail.surveyID, Equal<Current<Survey.surveyID>>,
            And<SurveyDetail.active, Equal<True>>>,
            OrderBy<Asc<SurveyDetail.sortOrder, Asc<SurveyDetail.lineNbr>>>> ActivePages;

        public SelectFrom<SurveyUser>.Where<SurveyUser.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View Users;

        [PXCopyPasteHiddenView]
        public SelectFrom<SurveyCollector>.
            LeftJoin<SurveyUser>.
                On<SurveyUser.surveyID.IsEqual<SurveyCollector.surveyID>.
                And<SurveyUser.lineNbr.IsEqual<SurveyCollector.userLineNbr>>>.
            Where<SurveyCollector.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View Collectors;

        [PXCopyPasteHiddenView]
        public PXSelect<SurveyCollectorData,
            Where<SurveyCollectorData.surveyID, Equal<Current<Survey.surveyID>>,
            And<SurveyCollectorData.token, Equal<Current<SurveyCollector.token>>>>,
            OrderBy<Asc<SurveyCollectorData.createdDateTime>>> CollectorDataRecords;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public PXSetup<SurveySetup> SurveySetup;

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

        public PXAction<Survey> checkCreateParams;
        [PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable CheckCreateParams(PXAdapter adapter) {
            return adapter.Get();
        }

        public PXFilter<CreateSurveyFilter> createSurveyFilter;
        public PXAction<Survey> createSurvey;
        [PXUIField(DisplayName = "Load Survey Pages", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable CreateSurvey(PXAdapter adapter) {
            List<Survey> list = adapter.Get<Survey>().ToList();
            WebDialogResult dialogResult = createSurveyFilter.AskExt(setStateFilter, true);
            if ((dialogResult == WebDialogResult.OK || (this.IsContractBasedAPI && dialogResult == WebDialogResult.Yes)) && createSurveyFilter.Current.NbQuestions > 0) {
                Save.Press();
                Survey order = PXCache<Survey>.CreateCopy(Survey.Current);
                DoCreateSurvey(order, createSurveyFilter.Current);
                List<Survey> rs = new List<Survey> { Survey.Current };
                return rs;
            }
            return list;
        }

        private void setStateFilter(PXGraph aGraph, string ViewName) {
            checkCreateParams.SetEnabled(createSurveyFilter.Current.NbQuestions.HasValue);
        }

        private void DoCreateSurvey(Survey survey, CreateSurveyFilter filter) {
            Survey.Current = survey;
            createSurveyFilter.Current = filter;
            var setup = SurveySetup.Current;
            DoResetPageNumbers(survey);
            var nbQuestions = filter.NbQuestions ?? 10;
            InsertMissing(survey, 1, SUTemplateType.Header, setup.DefHeaderID);
            InsertMissing(survey, 1, SUTemplateType.PageFooter, setup.DefPageFooterID, 1);
            var pageNumbers = Enumerable.Range(2, nbQuestions);
            foreach (var pageNumber in pageNumbers) {
                InsertMissings(survey, pageNumber);
            }
            InsertMissing(survey, 9999, SUTemplateType.Footer, setup.DefFooterID);
            Actions.PressSave();
        }

        private void InsertMissings(Survey survey, int pageNumber) {
            var setup = SurveySetup.Current;
            InsertMissing(survey, pageNumber, SUTemplateType.PageHeader, setup.DefPageHeaderID, 0);
            InsertMissing(survey, pageNumber, SUTemplateType.QuestionPage, setup.DefQuestionID, 1, setup.DefQuestAttrID);
            InsertMissing(survey, pageNumber, SUTemplateType.CommentPage, setup.DefCommentID, 2, setup.DefCommAttrID);
            InsertMissing(survey, pageNumber, SUTemplateType.PageFooter, setup.DefPageFooterID, 3);
        }

        private void InsertMissing(Survey survey, int pageNumber, string templateType, int? templateID, int offset = 0, string attrID = null) {
            SurveyDetail page = PXSelect<SurveyDetail,
                Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>,
                And<SurveyDetail.pageNbr, Equal<Required<SurveyDetail.pageNbr>>,
                And<SurveyDetail.templateType, Equal<Required<SurveyDetail.templateType>>>>>>.Select(this, survey.SurveyID, pageNumber, templateType);
            int? questionNbr;
            // Based on a) Question comes first, b) 1 Question, 1 Comment per page
            switch (templateType) {
                case SUTemplateType.QuestionPage:
                    questionNbr = ((pageNumber - 1) * 2) - 1;
                    break;
                case SUTemplateType.CommentPage:
                    questionNbr = (pageNumber - 1) * 2;
                    break;
                default:
                    questionNbr = null;
                    break;
            }
            if (page == null) {
                page = new SurveyDetail() {
                    SurveyID = survey.SurveyID,
                    PageNbr = pageNumber,
                    SortOrder = (pageNumber * 10) + offset,
                    TemplateType = templateType,
                    TemplateID = templateID,
                    QuestionNbr = questionNbr,
                    AttributeID = attrID
                };
                page = Details.Insert(page);
            }
        }

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

        public PXAction<Survey> resetPageNumbers;
        [PXUIField(DisplayName = "Reset Page Numbers", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable ResetPageNumbers(PXAdapter adapter) {
            var list = adapter.Get<Survey>().ToList();
            Save.Press();
            var graph = CreateInstance<SurveyMaint>();
            foreach (var survey in list) {
                var row = PXCache<Survey>.CreateCopy(survey);
                graph.DoResetPageNumbers(row);
            }
            return adapter.Get();
        }

        private void DoResetPageNumbers(Survey survey) {
            if (survey.Layout == SurveyLayout.MultiPage) {
                DoResetMultiplePageNumbers(survey);
            } else {
                DoResetSinglePageNumber(survey);
            }
        }

        private void DoResetMultiplePageNumbers(Survey survey) {
            // TODO - Check if all Page 1
            Survey.Current = survey;
            var surveyID = survey.SurveyID;
            SurveyDetail page = GetPage(surveyID, SUTemplateType.Header);
            if (page != null) {
                UpdatePageNbr(page, 1, 0); // PageNbr = 1, SortOrder = 10;
            }
            page = GetPage(surveyID, SUTemplateType.Footer);
            if (page != null) {
                UpdatePageNbr(page, 9999, 0); // PageNbr = 9999, SortOrder = 99990;
            }
            var currentPageNumbers = GetPageNumbers(survey, SurveyUtils.EXCEPT_HF);
            var newPageNumbers = Enumerable.Range(2, currentPageNumbers.Length);
            var mappings = currentPageNumbers.Zip(newPageNumbers, (currPage, newPage) => new Tuple<int, int>(currPage, newPage));
            var pages = GetRegularPages(surveyID);
            int offset = 0;
            var lastPageNbr = 0;
            foreach (SurveyDetail regPage in pages) {
                var mapping = mappings.First(tu => tu.Item1 == regPage.PageNbr);
                if (regPage.PageNbr != lastPageNbr) {
                    offset = 0;
                } else {
                    offset++;
                }
                var newPageNbr = mapping.Item2;
                UpdatePageNbr(regPage, newPageNbr, offset);// PageNbr = 2, SortOrder = 20, 21, 22;
                lastPageNbr = mapping.Item1;
            }
            Actions.PressSave();
            Details.View.RequestRefresh();
        }

        private void DoResetSinglePageNumber(Survey survey) {
            Survey.Current = survey;
            var details = Details.Select();
            int offset = 0;
            foreach (SurveyDetail detail in details) {
                UpdatePageNbr(detail, 1, offset);// PageNbr = 2, SortOrder = 20, 21, 22;
                offset++;
            }
            Actions.PressSave();
            Details.View.RequestRefresh();
        }

        public PXAction<Survey> generateSample;
        [PXUIField(DisplayName = "Generate Sample", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable GenerateSample(PXAdapter adapter) {
            var list = adapter.Get<Survey>().ToList();
            Save.Press();
            var graph = CreateInstance<SurveyMaint>();
            foreach (var survey in list) {
                var row = PXCache<Survey>.CreateCopy(survey);
                graph.DoGenerateSample(row);
            }
            return adapter.Get();
        }

        private void DoGenerateSample(Survey survey) {
            Survey.Current = survey;
            var user = DoInsertSampleUser(survey);
            var collector = DoInsertCollector(survey, user, null);
            var pages = GetPageNumbers(survey, SurveyUtils.ACTIVE_ONLY);
            var generator = new SurveyGenerator();
            foreach (var pageNbr in pages) {
                var pageContent = generator.GenerateSurveyPage(collector.Token, pageNbr);
                SaveContentToAttachment($"Survey-{survey.SurveyID}-Page-{pageNbr}.html", pageContent);
            }
            Actions.PressSave();
        }

        public SurveyCollector DoInsertCollector(Survey survey, SurveyUser user, Guid? refNoteID) {
            var collector = new SurveyCollector {
                SurveyID = survey.SurveyID,
                UserLineNbr = user.LineNbr,
                RefNoteID = refNoteID
            };
            var inserted = Collectors.Insert(collector);
            Actions.PressSave();
            return inserted;
        }

        public int[] GetPageNumbers(Survey survey, Func<SurveyDetail, bool> predicate) {
            Survey.Current = survey;
            var details = Details.Select().
                FirstTableItems.
                Where(pd => predicate(pd) && pd.PageNbr != null && pd.PageNbr > 0);
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

        private SurveyUser DoInsertSampleUser(Survey survey) {
            var setup = SurveySetup.Current;
            if (!setup.ContactID.HasValue) {
                throw new PXSetPropertyException(Messages.ContactNotSetup, Messages.SUSetup);
            }
            return InsertOrFindUser(survey, setup.ContactID);
        }

        public SurveyUser InsertOrFindUser(Survey survey, int? contactID) {
            SurveyUser user = SelectFrom<SurveyUser>.
                Where<SurveyUser.surveyID.IsEqual<@P.AsString>.
                And<SurveyUser.contactID.IsEqual<@P.AsInt>>>.
                View.SelectWindowed(this, 0, 1, survey.SurveyID, contactID);
            if (user == null) {
                user = new SurveyUser() {
                    Active = true,
                    ContactID = contactID,
                    SurveyID = survey.SurveyID
                };
                user = Users.Insert(user);
                //cache.SetDefaultExt<SurveyUser.lineNbr>(user);
                Actions.PressSave();
            }
            return user;
        }

        public PXAction<Survey> insertSampleCollector;
        [PXUIField(DisplayName = "Insert Sample Collector", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable InsertSampleCollector(PXAdapter adapter) {
            if (Survey.Current != null) {
                Save.Press();
                var user = DoInsertSampleUser(Survey.Current);
                var collector = DoInsertCollector(Survey.Current, user, null);
                Actions.PressSave();
                Collectors.View.RequestRefresh();
            }
            return adapter.Get();
        }

        public PXAction<Survey> redirectToSurvey;
        [PXUIField(DisplayName = "Run Survey", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable RedirectToSurvey(PXAdapter adapter) {
            if (Survey.Current != null && Collectors.Current != null) {
                Save.Press();
                var graph = CreateInstance<SurveyMaint>();
                var survey = PXCache<Survey>.CreateCopy(Survey.Current);
                var collector = PXCache<SurveyCollector>.CreateCopy(Collectors.Current);
                graph.DoRedirectToSurvey(survey, collector);
            }
            return adapter.Get();
        }

        private void DoRedirectToSurvey(Survey survey, SurveyCollector collector) {
            Survey.Current = survey;
            var generator = new SurveyGenerator();
            var activePages = ActivePages.Select().FirstTableItems;
            var firstPage = activePages.Min(det => det.PageNbr);
            var url = generator.GetUrl(collector.Token, firstPage.Value);
            throw new PXRedirectToUrlException(url, "Survey") {
                Mode = PXBaseRedirectException.WindowMode.NewWindow
            };
        }

        public PXAction<Survey> ViewEntity;
        [PXLookupButton(Tooltip = "View Reference Entity", OnClosingPopup = PXSpecialButtonType.Refresh)]
        [PXUIField(DisplayName = "View Entity", Visible = false)]
        protected virtual void viewEntity() {
            SurveyCollector current = this.Collectors.Current;
            if (current == null) {
                return;
            }
            (new EntityHelper(this)).NavigateToRow(current.RefNoteID, PXRedirectHelper.WindowMode.New);
        }

        protected virtual void _(Events.RowSelecting<Survey> e) {
            Survey row = e.Row;
            if (row == null) { return; }
            using (new PXConnectionScope()) {
                var collectors = SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<@P.AsString>>.
                                        View.SelectWindowed(this, 0, 1, row.SurveyID);
                row.IsSurveyInUse = collectors.Any();
            }
        }

        protected virtual void _(Events.RowSelected<Survey> e) {
            var row = e.Row;
            if (row == null) { return; }
            bool unlockedSurvey = !(row.IsSurveyInUse == true);
            e.Cache.AllowDelete = unlockedSurvey;
            PXUIFieldAttribute.SetEnabled<Survey.target>(e.Cache, row, unlockedSurvey);
            PXUIFieldAttribute.SetEnabled<Survey.layout>(e.Cache, row, unlockedSurvey);
            PXUIFieldAttribute.SetEnabled<Survey.entityType>(e.Cache, row, unlockedSurvey);
        }

        protected virtual void _(Events.FieldUpdated<Survey, Survey.layout> e) {
            var row = e.Row;
            if (row == null) { return; }
        }

        //protected virtual void _(Events.RowPersisted<Survey> e) {
        //    var row = e.Row;
        //    if (row == null)
        //        return;
        //    if (e.TranStatus == PXTranStatus.Completed) {
        //        //DoResetPageNumbers(row);
        //        //DoResetQuestionNumbers(row);
        //    }
        //}

        //protected virtual void _(Events.RowPersisted<SurveyDetail> e) {
        //    var row = e.Row;
        //    if (row == null)
        //        return;
        //    if (e.TranStatus == PXTranStatus.Completed) {
        //        DoInsertMissing(row);
        //    }
        //}

        //private void DoInsertMissing(SurveyDetail row) {
        //    var setup = SurveySetup.Current;
        //    //if (SUTemplateType.NeedsSurrounding(row.TemplateType)) {
        //    //    DoInsertMissing(row, SUTemplateType.PageHeader, -1, setup.DefPageHeaderID);
        //    //    DoInsertMissing(row, SUTemplateType.PageFooter, 1, setup.DefPageFooterID);
        //    //}
        //}

        //private void DoInsertMissing(SurveyDetail row, string templateType, int offset, int? templateID) {
        //    if (!templateID.HasValue) {
        //        return;
        //    }
        //    var page = GetPage(row.SurveyID, row.PageNbr, templateType);
        //    if (page == null) {
        //        page = new SurveyDetail {
        //            SurveyID = row.SurveyID,
        //            TemplateType = templateType,
        //            TemplateID = templateID,
        //            Active = true
        //        };
        //        page = Details.Insert(page);
        //        UpdatePageNbr(page, row.PageNbr ?? 1, offset);
        //    }
        //}

        private void UpdatePageNbr(SurveyDetail page, int pageNbr, int offset) {
            page.PageNbr = pageNbr;
            page.SortOrder = (pageNbr * 10) + offset;
            Details.Update(page);
        }

        private SurveyDetail GetPage(string surveyID, string templateType) {
            return PXSelect<SurveyDetail,
                    Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>,
                    And<SurveyDetail.templateType, Equal<Required<SurveyDetail.templateType>>>>>.Select(this, surveyID, templateType);
        }

        private SurveyDetail GetPage(string surveyID, int? pageNbr, string templateType) {
            return PXSelect<SurveyDetail,
                    Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>,
                    And<SurveyDetail.pageNbr, Equal<Required<SurveyDetail.pageNbr>>,
                    And<SurveyDetail.templateType, Equal<Required<SurveyDetail.templateType>>>>>>.Select(this, surveyID, pageNbr, templateType);
        }

        private PXResultset<SurveyDetail> GetRegularPages(string surveyID) {
            return PXSelect<SurveyDetail,
                    Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>,
                    And<SurveyDetail.templateType, In3<SUTemplateType.pageHeader, SUTemplateType.questionPage, SUTemplateType.contentPage, SUTemplateType.pageFooter>>>,
                    OrderBy<Asc<SurveyDetail.pageNbr, Asc<SurveyDetail.sortOrder>>>>.Select(this, surveyID);
        }

        protected virtual void _(Events.RowSelected<SurveyDetail> e) {
            var row = e.Row;
            if (row == null || Survey.Current == null || Survey.Current.Layout == null) {
                return;
            }
            var isMulti = Survey.Current.Layout == SurveyLayout.MultiPage;
            PXUIFieldAttribute.SetEnabled<SurveyDetail.pageNbr>(e.Cache, row, isMulti);
        }

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.description> e) {
            var row = e.Row;
            if (row == null || row.TemplateID == null || !string.IsNullOrEmpty(row.Description)) {
                return;
            }
            SurveyTemplate st = SurveyTemplate.PK.Find(this, row.TemplateID);
            if (st == null) {
                return;
            }
            switch (st.TemplateType) {
                case SUTemplateType.PageHeader:
                case SUTemplateType.PageFooter:
                    break;
                case SUTemplateType.Header:
                    e.NewValue = "WELCOME YOU";
                    break;
                case SUTemplateType.QuestionPage:
                    e.NewValue = "ASK YOU";
                    break;
                case SUTemplateType.CommentPage:
                    e.NewValue = "TELL ME MORE";
                    break;
                case SUTemplateType.ContentPage:
                    e.NewValue = "SHOW YOU";
                    break;
                case SUTemplateType.Footer:
                    e.NewValue = "THANK YOU";
                    break;
            }
            e.Cancel = e.NewValue != null;
        }

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.templateID> e) {
            var row = e.Row;
            if (row == null || row.TemplateType == null) {
                return;
            }
            var setup = SurveySetup.Current;
            switch (row.TemplateType) {
                case SUTemplateType.Header:
                    e.NewValue = setup.DefHeaderID;
                    break;
                case SUTemplateType.PageHeader:
                    e.NewValue = setup.DefPageHeaderID;
                    break;
                case SUTemplateType.PageFooter:
                    e.NewValue = setup.DefPageFooterID;
                    break;
                case SUTemplateType.QuestionPage:
                    e.NewValue = setup.DefQuestionID;
                    break;
                case SUTemplateType.CommentPage:
                    e.NewValue = setup.DefCommentID;
                    break;
                case SUTemplateType.Footer:
                    e.NewValue = setup.DefFooterID;
                    break;
            }
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

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.nbrOfRows> e) {
            var row = e.Row;
            if (row == null || row.TemplateType == null || row.TemplateType != SUTemplateType.CommentPage) {
                return;
            }
            var setup = SurveySetup.Current;
            e.NewValue = setup?.DefNbrOfRows;
            e.Cancel = e.NewValue != null;
        }

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.maxLength> e) {
            var row = e.Row;
            if (row == null || row.TemplateType == null || row.TemplateType != SUTemplateType.CommentPage) {
                return;
            }
            var setup = SurveySetup.Current;
            e.NewValue = setup?.DefMaxLength;
            e.Cancel = e.NewValue != null;
        }

        //protected virtual void _(Events.FieldUpdated<SurveyDetail, SurveyDetail.pageNbr> e) {
        //    e.Cache.SetDefaultExt<SurveyDetail.description>(e.Row);
        //}

        private int GetMaxPage(string surveyID) {
            var maxPage = PXSelect<SurveyDetail, Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>>>.Select(this, surveyID).FirstTableItems.Select(sd => sd.PageNbr).Max();
            return maxPage ?? 0;
        }

        public void _(Events.FieldUpdated<SurveyCollector, SurveyCollector.collectorID> e) {
            e.Cache.SetDefaultExt<SurveyCollector.token>(e.Row);
        }


        protected virtual void _(Events.FieldDefaulting<SurveyCollector, SurveyCollector.name> e) {
            //var row = e.Row;
            //if (row == null || row.SurveyID == null) {
            //    return;
            //}
            if (Survey.Current == null) {
                return;
            }
            var survey = Survey.Current;
            e.NewValue = $"{survey.SurveyID}-{PXTimeZoneInfo.Now:yyyy-MM-dd hh:mm:ss}";
            e.Cancel = true;
        }

        //protected virtual void _(Events.FieldDefaulting<SurveyCollector, SurveyCollector.token> e) {
        //    var row = e.Row;
        //    if (row == null || row.CollectorID == null) {
        //        return;
        //    }
        //    e.NewValue = Net_Utils.ComputeMd5(row.CollectorID.ToString(), true);
        //    e.Cancel = true;
        //}

        [PXCacheName("CreateSurveyFilter")]
        [Serializable]
        public class CreateSurveyFilter : IBqlTable {

            #region NbQuestions
            public abstract class nbQuestions : BqlInt.Field<nbQuestions> { }
            [PXInt]
            [PXUnboundDefault(10)]
            [PXUIField(DisplayName = "Number of Questions")]
            public virtual int? NbQuestions { get; set; }
            #endregion
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

        //[PXMergeAttributes]
        //[PXParent(typeof(Select<Survey, Where<Survey.surveyID, Equal<Current<CSAttributeGroup.entityClassID>>>>), LeaveChildren = true)]
        //[PXDBDefault(typeof(Survey.surveyIDStringID))]
        //protected virtual void _(Events.CacheAttached<CSAttributeGroup.entityClassID> e) { }
    }
}