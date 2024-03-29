﻿using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace PX.Survey.Ext {
    public class SurveyMaint : PXGraph<SurveyMaint, Survey> {

        public SelectFrom<Survey>.View Survey;
        public PXSelect<Survey, Where<Survey.surveyID, Equal<Current<Survey.surveyID>>>> CurrentSurvey;

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
            Where<SurveyCollector.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.
            OrderBy<SurveyCollector.lastModifiedDateTime.Desc>.View Collectors;

        [PXCopyPasteHiddenView]
        public PXSelect<SurveyCollectorData,
            Where<SurveyCollectorData.surveyID, Equal<Current<Survey.surveyID>>,
            And<SurveyCollectorData.token, Equal<Current<SurveyCollector.token>>>>,
            OrderBy<Asc<SurveyCollectorData.createdDateTime>>> CollectorDataRecords;

        public PXSelect<SurveyCollectorData,
            Where<SurveyCollectorData.surveyID, Equal<Current<Survey.surveyID>>,
            And<SurveyCollectorData.status, NotEqual<CollectorDataStatus.processed>>>,
            OrderBy<Asc<SurveyCollectorData.createdDateTime>>> UnprocessedCollectorDataRecords;

        [PXCopyPasteHiddenView]
        public PXSelect<SurveyAnswer,
            Where<SurveyAnswer.surveyID, Equal<Current<Survey.surveyID>>>,
            OrderBy<Asc<SurveyAnswer.createdDateTime>>> Answers;

        [PXCopyPasteHiddenView]
        public PXSelectJoinGroupBy<SurveyAnswer,
            LeftJoin<SurveyDetail,
                On<SurveyDetail.surveyID, Equal<SurveyAnswer.surveyID>,
                And<SurveyDetail.lineNbr, Equal<SurveyAnswer.detailLineNbr>>>>,
            Where<SurveyAnswer.surveyID, Equal<Current<Survey.surveyID>>,
            And<SurveyDetail.componentType, Equal<SUComponentType.questionPage>,
            And<SurveyAnswer.value, IsNotNull,
            And<SurveyAnswer.value, NotEqual<Empty>>>>>,
            Aggregate<
                GroupBy<SurveyDetail.pageNbr,
                GroupBy<SurveyDetail.questionNbr,
                GroupBy<SurveyAnswer.value,
                Count<SurveyAnswer.lineNbr>>>>>,
            OrderBy<Asc<SurveyDetail.pageNbr, Asc<SurveyDetail.questionNbr, Desc<SurveyAnswer.value>>>>> AnswerSummary;

        [PXCopyPasteHiddenView]
        public PXSelectJoin<SurveyAnswer,
            LeftJoin<SurveyDetail,
                On<SurveyDetail.surveyID, Equal<SurveyAnswer.surveyID>,
                And<SurveyDetail.lineNbr, Equal<SurveyAnswer.detailLineNbr>>>>,
            Where<SurveyAnswer.surveyID, Equal<Current<Survey.surveyID>>,
            And<SurveyDetail.componentType, Equal<SUComponentType.commentPage>,
            And<SurveyAnswer.value, IsNotNull,
            And<SurveyAnswer.value, NotEqual<Empty>>>>>,
            OrderBy<Asc<SurveyDetail.pageNbr, Asc<SurveyDetail.questionNbr, Asc<SurveyAnswer.createdDateTime>>>>> Comments;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public PXSetup<SurveySetup> SurveySetup;

        public PXFilter<OperationParam> Operations;

        //[PXFilterable(new Type[] { })]
        //[PXImport(typeof(Survey))]
        //[PXViewName("Survey Members")]
        //public SurveyMembersList CampaignMembers;

        [InjectDependency]
        public Api.Services.ICompanyService CompanyService { get; set; }

        public SurveyMaint() {
        }

        [SurveyID(IsKey = true)]
        [PXDBDefault(typeof(Survey.surveyID))]
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXParent(typeof(Select<Survey, Where<Survey.surveyID, Equal<Current<SurveyMember.surveyID>>>>))]
        [PXUIField(DisplayName = "Survey ID")]
        protected virtual void _(Events.CacheAttached<SurveyMember.surveyID> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Member Since", Enabled = false)]
        protected virtual void _(Events.CacheAttached<SurveyMember.createdDateTime> e) { }

        #region RecipientSelected Lookup
        public PXFilter<RecipientFilter> recipientfilter;
        [PXFilterable]
        [PXCopyPasteHiddenView]
        public PXSelect<RecipientSelected> recipients;

        public PXAction<Survey> addRecipients;
        [PXUIField(DisplayName = "Add Recipients", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable AddRecipients(PXAdapter adapter) {
            if (recipientfilter.Current == null) {
                recipientfilter.Current = recipientfilter.Insert(new RecipientFilter());
            }
            recipients.View.RequestRefresh();
            if (recipients.AskExt() == WebDialogResult.OK) {
                return AddSelectedRecipients(adapter);
            }
            return adapter.Get();
        }

        public PXAction<Survey> addSelectedRecipients;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXButton]
        public virtual IEnumerable AddSelectedRecipients(PXAdapter adapter) {
            Users.Cache.ForceExceptionHandling = true;
            foreach (RecipientSelected recipient in recipients.Cache.Cached) {
                if (recipient.Selected == true) {
                    var surveyUser = new SurveyUser();
                    surveyUser.Active = true;
                    surveyUser.SurveyID = Survey.Current.SurveyID;
                    surveyUser.ContactID = recipient.ContactID;
                    Users.Insert(surveyUser);
                }
            }
            recipients.View.RequestRefresh();
            Users.View.RequestRefresh();
            return adapter.Get();
        }

        #endregion

        public PXAction<Survey> checkCreateParams;
        [PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable CheckCreateParams(PXAdapter adapter) {
            return adapter.Get();
        }

        public PXFilter<CreateSurveyFilter> createSurveyFilter;
        public PXAction<Survey> createSurvey;
        [PXUIField(DisplayName = "Load Template", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
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
            var nbQuestions = filter.NbQuestions ?? 10;
            var questionSeries = Enumerable.Range(1, nbQuestions);
            var prevSeries = 0;
            var offset = 0;
            var isSingle = survey.Layout == SurveyLayout.SinglePage;
            InsertMissing(survey, 0, prevSeries, SUComponentType.Header, setup.DefHeaderID, offset);
            if (!isSingle) {
                InsertMissing(survey, 0, prevSeries, SUComponentType.PageFooter, setup.DefPageFooterID, offset);
            }
            foreach (var series in questionSeries) {
                InsertMissings(survey, series, prevSeries, offset);
                prevSeries = series;
            }
            if (isSingle) {
                InsertMissing(survey, nbQuestions + 1, prevSeries, SUComponentType.PageFooter, setup.DefPageFooterID, offset);
            }
            InsertMissing(survey, nbQuestions + 1, prevSeries, SUComponentType.Footer, setup.DefFooterID, offset);
            Actions.PressSave();
        }

        private void InsertMissings(Survey survey, int series, int prevSeries, int offset) {
            var setup = SurveySetup.Current;
            var isSingle = survey.Layout == SurveyLayout.SinglePage;
            if (!isSingle) {
                InsertMissing(survey, series, prevSeries, SUComponentType.PageHeader, setup.DefPageHeaderID, offset);
            }
            InsertMissing(survey, series, prevSeries, SUComponentType.QuestionPage, setup.DefQuestionID, offset, setup.DefQuestAttrID);
            InsertMissing(survey, series, prevSeries, SUComponentType.CommentPage, setup.DefCommentID, offset, setup.DefCommAttrID);
            if (!isSingle) {
                InsertMissing(survey, series, prevSeries, SUComponentType.PageFooter, setup.DefPageFooterID, offset);
            }
        }

        private void InsertMissing(Survey survey, int series, int prevSeries, string templateType, string templateID, int offset, string attrID = null) {
            var isSingle = survey.Layout == SurveyLayout.SinglePage;
            var pageNumber = isSingle ? ((templateType == SUComponentType.Footer) ? 2 : 1) : series;
            HandleOffsets(ref offset, isSingle, series, prevSeries);
            int? questionNbr;
            // Based on a) Question comes first, b) 1 Question, 1 Comment per page
            switch (templateType) {
                case SUComponentType.QuestionPage:
                    questionNbr = (series * 2) - 1;
                    break;
                case SUComponentType.CommentPage:
                    questionNbr = series * 2;
                    break;
                default:
                    questionNbr = null;
                    break;
            }
            SurveyDetail page = PXSelect<SurveyDetail,
                Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>,
                And<SurveyDetail.pageNbr, Equal<Required<SurveyDetail.pageNbr>>,
                And<SurveyDetail.componentType, Equal<Required<SurveyDetail.componentType>>,
                And<Where<SurveyDetail.questionNbr, IsNull, Or<SurveyDetail.questionNbr, Equal<Required<SurveyDetail.questionNbr>>>>>>>>>.Select(this, survey.SurveyID, pageNumber, templateType, questionNbr);
            if (page == null) {
                page = new SurveyDetail() {
                    SurveyID = survey.SurveyID,
                    PageNbr = pageNumber,
                    SortOrder = (series * 10) + offset,
                    ComponentType = templateType,
                    ComponentID = templateID,
                    QuestionNbr = questionNbr,
                    AttributeID = attrID
                };
                page = Details.Insert(page);
            }
        }

        private static void HandleOffsets(ref int offset, bool isSingle, int series, int prevSeries) {
            if (isSingle || series == prevSeries) {
                offset++;
            } else {
                offset = 0;
            }
        }

        public PXFilter<ComponentFilter> templatefilter;

        [PXFilterable]
        [PXCopyPasteHiddenView]
        public ComponentLookup<ComponentSelected, ComponentFilter> templates;

        public PXAction<Survey> addTemplates;
        [PXUIField(DisplayName = "Add Pages", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable AddTemplates(PXAdapter adapter) {
            if (templatefilter.Current == null) {
                templatefilter.Current = templatefilter.Insert(new ComponentFilter());
            }
            if (templates.AskExt() == WebDialogResult.OK) {
                return AddSelectedTemplate(adapter);
            }
            recipients.Cache.Clear();
            return adapter.Get();
        }

        public PXAction<Survey> addSelectedTemplate;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXButton]
        public virtual IEnumerable AddSelectedTemplate(PXAdapter adapter) {
            Details.Cache.ForceExceptionHandling = true;
            foreach (ComponentSelected template in templates.Cache.Cached) {
                if (template.Selected == true) {
                    var surveyDetail = new SurveyDetail();
                    surveyDetail.SurveyID = Survey.Current.SurveyID;
                    surveyDetail.ComponentType = template.ComponentType;
                    surveyDetail.Description = template.Description;
                    surveyDetail.ComponentID = template.ComponentID;
                    Details.Update(surveyDetail);
                }
            }
            templates.Cache.Clear();
            return adapter.Get();
        }

        public PXAction<Survey> resetPageNumbers;
        [PXUIField(DisplayName = "Reset Page Numbers", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
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
            SurveyDetail page = GetPage(surveyID, SUComponentType.Header);
            if (page != null) {
                UpdatePageNbr(page, 1, 0); // PageNbr = 1, SortOrder = 10;
            }
            page = GetPage(surveyID, SUComponentType.Footer);
            if (page != null) {
                UpdatePageNbr(page, 9999, 0); // PageNbr = 9999, SortOrder = 99990;
            }
            var currentPageNumbers = GetPageNumbers(survey, SurveyUtils.EXCEPT_HF_PAGES);
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
        [PXUIField(DisplayName = "Generate HTML", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable GenerateSample(PXAdapter adapter) {
            if (Survey.Current != null) {
                Save.Press();
                PXLongOperation.StartOperation(this, delegate () {
                    var graph = CreateInstance<SurveyMaint>();
                    graph.DoGenerateSample(Survey.Current);
                });
            }
            return adapter.Get();
        }

        private void DoGenerateSample(Survey survey) {
            Survey.Current = survey;
            var user = DoInsertSampleUser(survey); // TODO Just put in cache
            var collector = DoUpsertCollector(survey, user, null, true, true); // TODO Just put in cache
            var pages = GetPageNumbers(survey, SurveyUtils.ACTIVE_PAGES_ONLY);
            var generator = new SurveyGenerator();
            foreach (var pageNbr in pages) {
                var (pageContent, _) = generator.GenerateSurveyPage(collector.Token, pageNbr);
                SaveContentToAttachment($"Survey-{survey.SurveyID}-Page-{pageNbr}.html", pageContent);
            }
            Actions.PressSave();
        }

        public PXAction<Survey> viewComponent;
        [PXUIField(DisplayName = "View Component", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewComponent(PXAdapter adapter) {
            if (Details.Current != null) {
                var graph = CreateInstance<SurveyComponentMaint>();
                graph.SUComponent.Current = graph.SUComponent.Search<SurveyComponent.componentID>(Details.Current.ComponentID);
                throw new PXRedirectRequiredException(graph, true, string.Empty);
            }
            return adapter.Get();
        }

        public PXAction<Survey> viewAttribute;
        [PXUIField(DisplayName = "View Attribute", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewAttribute(PXAdapter adapter) {
            if (Details.Current != null) {
                var graph = CreateInstance<CSAttributeMaint>();
                graph.Attributes.Current = graph.Attributes.Search<CSAttribute.attributeID>(Details.Current.AttributeID);
                throw new PXRedirectRequiredException(graph, true, string.Empty);
            }
            return adapter.Get();
        }

        public SurveyCollector DoUpsertCollector(Survey survey, SurveyUser user, Guid? refNoteID, bool saveNow, bool isTest) {
            var collector = new SurveyCollector {
                SurveyID = survey.SurveyID,
                UserLineNbr = user.LineNbr,
                RefNoteID = refNoteID,
                Anonymous = user.Anonymous,
                IsTest = isTest
            };
            var inserted = Collectors.Insert(collector);
            if (saveNow) {
                Actions.PressSave();
            }
            return inserted;
        }

        public int GetLastQuestionPageNumber(Survey survey) {
            var allActiveQuestions = GetDetailInfo(survey, SurveyUtils.ACTIVE_QUESTIONS_ONLY, SurveyUtils.GET_QUES_AND_PAGE_NBR);
            var lastQuestionNbr = allActiveQuestions.Max(tu => tu.Item1);
            var lastPageNbr = allActiveQuestions.First(tu => tu.Item1 == lastQuestionNbr).Item2;
            return lastPageNbr;
        }

        public int[] GetPageNumbers(Survey survey, Func<SurveyDetail, bool> predicate) {
            return GetDetailInfo(survey, predicate, SurveyUtils.GET_PAGE_NBR);
        }

        public int[] GetQuestionNumbers(Survey survey, Func<SurveyDetail, bool> predicate) {
            return GetDetailInfo(survey, predicate, SurveyUtils.GET_QUES_NBR);
        }

        public DetailType[] GetDetailInfo<DetailType>(Survey survey, Func<SurveyDetail, bool> predicate, Func<SurveyDetail, DetailType> retriever) {
            Survey.Current = survey;
            var details = Details.Select().
                FirstTableItems.
                Where(pd => predicate(pd) && retriever(pd) != null);
            var infos = details.
                Select(pd => retriever(pd)).
                Distinct().OrderBy(val => val).ToArray();
            return infos;
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
            return InsertOrFindUser(survey, setup.ContactID, false);
        }

        public SurveyUser InsertOrFindUser(Survey survey, int? contactID, bool anonymous) {
            SurveyUser user = SelectFrom<SurveyUser>.
                Where<SurveyUser.surveyID.IsEqual<@P.AsString>.
                And<SurveyUser.contactID.IsEqual<@P.AsInt>>>.
                View.SelectWindowed(this, 0, 1, survey.SurveyID, contactID);
            if (user == null) {
                user = InsertUser(survey, contactID, anonymous);
            }
            return user;
        }

        public SurveyUser InsertUser(Survey survey, int? contactID, bool anonymous) {
            var user = new SurveyUser() {
                Active = true,
                ContactID = contactID,
                SurveyID = survey.SurveyID,
                Anonymous = anonymous,
            };
            user = Users.Insert(user);
            Actions.PressSave();
            return user;
        }

        public PXAction<Survey> startSurvey;
        [PXUIField(DisplayName = "Start Survey", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable StartSurvey(PXAdapter adapter) {
            Save.Press();
            var list = adapter.Get<Survey>().ToList();
            PXLongOperation.StartOperation(this, delegate () {
                var graph = CreateInstance<SurveyMaint>();
                foreach (var survey in list) {
                    if (survey.Status != SurveyStatus.Preparing) {
                        continue;
                    }
                    try {
                        if (adapter.MassProcess) {
                            PXProcessing<Survey>.SetCurrentItem(survey);
                        }
                        graph.DoLoadCollectors(survey, adapter.MassProcess);
                        survey.Status = SurveyStatus.Started;
                        graph.Survey.Update(survey);
                        graph.Actions.PressSave();
                    } catch (Exception ex) {
                        if (!adapter.MassProcess) {
                            throw;
                        }
                        PXProcessing<Survey>.SetError(ex);
                    }
                }
            });
            return list;
        }

        private void DoLoadCollectors(Survey survey, bool massProcess) {
            Survey.Current = survey;
            foreach (var user in Users.Select()) {
                var collector = DoUpsertCollector(Survey.Current, user, null, false, false);
            }
            Actions.PressSave();
            Collectors.View.RequestRefresh();
        }

        public PXAction<Survey> insertSampleCollector;
        [PXUIField(DisplayName = "Insert Sample Collector", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable InsertSampleCollector(PXAdapter adapter) {
            if (Survey.Current != null) {
                Save.Press();
                var user = DoInsertSampleUser(Survey.Current);
                var collector = DoUpsertCollector(Survey.Current, user, null, true, true);
                Actions.PressSave();
                Collectors.View.RequestRefresh();
            }
            return adapter.Get();
        }

        public PXAction<Survey> redirectToSurvey;
        [PXUIField(DisplayName = "Run Survey", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
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
            var token = (collector != null) ? collector.Token : survey.SurveyID;
            var url = generator.GetUrl(token, firstPage.Value);
            throw new PXRedirectToUrlException(url, "Survey") {
                Mode = PXBaseRedirectException.WindowMode.NewWindow
            };
        }

        public PXAction<Survey> redirectToAnonymousSurvey;
        [PXUIField(DisplayName = "Preview", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable RedirectToAnonymousSurvey(PXAdapter adapter) {
            if (Survey.Current != null) {
                Save.Press();
                var graph = CreateInstance<SurveyMaint>();
                var survey = PXCache<Survey>.CreateCopy(Survey.Current);
                graph.DoRedirectToSurvey(survey, null);
            }
            return adapter.Get();
        }

        public PXAction<Survey> ViewEntity;
        [PXButton(Tooltip = "View Reference Entity", OnClosingPopup = PXSpecialButtonType.Refresh)]
        [PXUIField(DisplayName = "View Entity", Visible = false)]
        protected virtual void viewEntity() {
            SurveyCollector current = this.Collectors.Current;
            if (current == null) {
                return;
            }
            (new EntityHelper(this)).NavigateToRow(current.RefNoteID, PXRedirectHelper.WindowMode.New);
        }

        public PXAction<Survey> processAnswers;
        [PXUIField(DisplayName = Messages.SurveyAction.ProcessAnswers, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ProcessAnswers(PXAdapter adapter) {
            var list = adapter.Get<Survey>().ToList();
            Save.Press();
            var graph = CreateInstance<SurveyMaint>();
            foreach (var survey in list) {
                var row = PXCache<Survey>.CreateCopy(survey);
                graph.DoProcessAnswers(row);
            }
            return adapter.Get();
        }

        public PXAction<Survey> reProcessAnswers;
        [PXUIField(DisplayName = Messages.SurveyAction.ClearAnswers, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ReProcessAnswers(PXAdapter adapter) {
            var list = adapter.Get<Survey>().ToList();
            Save.Press();

            // Return from empty list
            if (list.Count < 1) return adapter.Get();

            WebDialogResult result = adapter.View.Ask(list[0], Messages.SurveyAction.ClearAnswersHeader, Messages.SurveyAction.ClearAnswersPrompt, MessageButtons.YesNo, MessageIcon.Question);
            if (result == WebDialogResult.Yes)
            {
                var graph = CreateInstance<SurveyMaint>();
                foreach (var survey in list) {

                        var row = PXCache<Survey>.CreateCopy(survey);
                        graph.DoReProcessAnswers(row);

                }

            }

            return adapter.Get();
        }

        public PXAction<Survey> sendNewNotification;
        [PXUIField(DisplayName = "Send Notification", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable SendNewNotification(PXAdapter adapter) {
            if (Collectors.Current != null) {
                Save.Press();
                var graph = CreateInstance<SurveyCollectorMaint>();
                var coll = PXCache<SurveyCollector>.CreateCopy(Collectors.Current);
                graph.DoSendNewNotification(coll);
            }
            return adapter.Get();
        }

        public PXAction<Survey> sendReminder;
        [PXUIField(DisplayName = "Send Reminder", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable SendReminder(PXAdapter adapter) {
            if (Collectors.Current != null) {
                Save.Press();
                var graph = CreateInstance<SurveyCollectorMaint>();
                var coll = PXCache<SurveyCollector>.CreateCopy(Collectors.Current);
                graph.DoSendReminder(coll, null);
            }
            return adapter.Get();
        }

        public void DoReProcessAnswers(Survey survey) {
            Survey.Current = survey;
            // Delete all answers
            foreach (var answer in Answers.Select()) {
                Answers.Delete(answer);
            }
            // Reset all completed collectors
            foreach (SurveyCollector coll in Collectors.Select()) {
                if (coll.Status == CollectorStatus.Processed) {
                    coll.Status = CollectorStatus.Completed;
                    Collectors.Update(coll);
                }
                Collectors.Current = coll;
                // Reset all collector data rows
                foreach (SurveyCollectorData data in CollectorDataRecords.Select()) {
                    //if (data.Status == CollectorDataStatus.Processed) {
                    data.Status = CollectorDataStatus.Updated;
                    CollectorDataRecords.Update(data);
                    //}
                }
            }
            Actions.PressSave();
            Answers.View.RequestRefresh();
            Collectors.View.RequestRefresh();
            CollectorDataRecords.View.RequestRefresh();
        }

        public bool DoProcessAnswers(Survey survey) {
            Survey.Current = survey;
            var allCollectors = Collectors.Select();
            bool errorOccurred = false;
            var unanswered = GetQuestionNumbers(survey, SurveyUtils.ACTIVE_QUESTIONS_ONLY).ToList();
            foreach (var res in allCollectors) {
                var collector = PXResult.Unwrap<SurveyCollector>(res);
                if (collector.Status != CollectorStatus.Completed) {
                    continue;
                }
                Collectors.Current = collector;
                var token = collector.Token;
                var collectorDatas = CollectorDataRecords.Select().FirstTableItems;
                foreach (var collData in collectorDatas) {
                    if (collData.Status == CollectorDataStatus.Processed ||
                        collData.Status == CollectorDataStatus.Ignored) {
                        continue;
                    }
                    try {
                        var (status, answered) = DoProcessAnswers(survey, collData, collector);
                        unanswered.RemoveAll(unans => answered.Contains(unans));
                        collData.Status = status;
                        collData.Message = null;
                    } catch (Exception ex) {
                        collData.Status = CollectorDataStatus.Error;
                        collData.Message = ex.Message;
                        errorOccurred = true;
                    }
                    var updated = CollectorDataRecords.Update(collData);
                }
                if (unanswered.Any()) {
                    collector.Status = CollectorStatus.Incomplete;
                } else {
                    collector.Status = CollectorStatus.Completed;
                }
                Collectors.Update(collector);
            }
            Answers.View.RequestRefresh();
            Collectors.View.RequestRefresh();
            CollectorDataRecords.View.RequestRefresh();
            Actions.PressSave();
            return errorOccurred;
        }

        private (string, List<int>) DoProcessAnswers(Survey survey, SurveyCollectorData collDataRec, SurveyCollector collector) {
            var answerData = collDataRec.Payload;
            if (string.IsNullOrEmpty(answerData)) {
                throw new PXException(Messages.AnswersNotfound);
            }
            var nvc = HttpUtility.ParseQueryString(answerData);
            var dict = nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
            //{{PageNbr}}.{{QuestionNbr}}.{{LineNbr}}=Value&...
            //var answers = new List<CSAnswers>();
            var status = CollectorDataStatus.Ignored;
            var answered = new List<int>();
            foreach (var kvp in dict) {
                var answerKey = kvp.Key;
                var matches = SurveyUtils.ANSWER_CODE.Matches(answerKey);
                if (matches.Count > 0) {
                    for (int m = 0; m < matches.Count; m++) {
                        var match = matches[m];
                        var groups = match.Groups;
                        var pageNbr = int.Parse(groups[SurveyUtils.PAGE_NBR].Value);
                        var quesNbr = int.Parse(groups[SurveyUtils.QUES_NBR].Value);
                        var lineNbr = int.Parse(groups[SurveyUtils.LINE_NBR].Value);
                        SurveyDetail detail = SurveyDetail.PK.Find(this, survey.SurveyID, lineNbr);
                        if (detail == null) {
                            throw new PXException(Messages.SurveyDetailNotFound, answerKey);
                        }
                        if (pageNbr != detail.PageNbr) {
                            throw new PXException(Messages.SurveyDetailPageNbrDoesNotMatch, detail.PageNbr, pageNbr);
                        }
                        if (quesNbr != detail.QuestionNbr) {
                            throw new PXException(Messages.SurveyDetailQuesNbrDoesNotMatch, detail.QuestionNbr, quesNbr);
                        }
                        string attrID = detail.AttributeID;
                        if (attrID == null) {
                            // Not a question
                            continue;
                        }
                        var value = kvp.Value;
                        // TODO Skip empty comments
                        if (value == null) {
                            // No answer
                            continue;
                        }
                        var controlType = detail.ControlType;
                        if (controlType == SUControlType.Multi) {
                            var allValues = value.Split(',');
                            foreach (var singleValue in allValues) {
                                UpsertAnswer(survey, collector, detail, singleValue, true);
                            }
                        } else {
                            UpsertAnswer(survey, collector, detail, value, false);
                        }
                        answered.Add(quesNbr);
                        status = CollectorDataStatus.Processed;
                    }
                } else {
                    continue;
                }
            }
            return (status, answered);
        }

        private void UpsertAnswer(Survey survey, SurveyCollector collector, SurveyDetail detail, string value, bool searchByValue) {
            SurveyAnswer answer;
            if (searchByValue) {
                answer = SelectFrom<SurveyAnswer>.
                Where<SurveyAnswer.surveyID.IsEqual<@P.AsString>.
                And<SurveyAnswer.collectorID.IsEqual<@P.AsInt>.
                And<SurveyAnswer.detailLineNbr.IsEqual<@P.AsInt>.
                And<SurveyAnswer.value.IsEqual<@P.AsString>>>>>.
                View.SelectWindowed(this, 0, 1, survey.SurveyID, collector.CollectorID, detail.LineNbr, value);
            } else {
                answer = SelectFrom<SurveyAnswer>.
                Where<SurveyAnswer.surveyID.IsEqual<@P.AsString>.
                And<SurveyAnswer.collectorID.IsEqual<@P.AsInt>.
                And<SurveyAnswer.detailLineNbr.IsEqual<@P.AsInt>>>>.
                View.SelectWindowed(this, 0, 1, survey.SurveyID, collector.CollectorID, detail.LineNbr);
            }
            if (answer == null) {
                answer = new SurveyAnswer() {
                    SurveyID = survey.SurveyID,
                    CollectorID = collector.CollectorID,
                    DetailLineNbr = detail.LineNbr,
                    Value = value,
                };
                Answers.Insert(answer);
            } else {
                answer.Value = value;
                Answers.Update(answer);
            }
        }

        protected virtual void _(Events.RowSelected<Survey> e) {
            var row = e.Row;
            if (row == null) { return; }
            bool lockedSurvey = SurveyStatus.IsLocked(row.Status);
            e.Cache.AllowDelete = !lockedSurvey;
            var hasPages = HasDetailRecords();
            redirectToAnonymousSurvey.SetEnabled(hasPages);
            generateSample.SetEnabled(hasPages);
            var readyToStart = row.Status == SurveyStatus.Preparing && hasPages;
            startSurvey.SetEnabled(readyToStart);
            PXUIFieldAttribute.SetEnabled<Survey.target>(e.Cache, row, !lockedSurvey);
            PXUIFieldAttribute.SetEnabled<Survey.layout>(e.Cache, row, !lockedSurvey);
            var isAnon = row.Target == SurveyTarget.Anonymous;
            var isEntity = row.Target == SurveyTarget.Entity;
            PXUIFieldAttribute.SetEnabled<Survey.allowAnonymous>(e.Cache, row, !isAnon);
            PXUIFieldAttribute.SetEnabled<Survey.keepAnswersAnonymous>(e.Cache, row, !isAnon);
            Users.AllowInsert = Users.AllowUpdate = Users.AllowSelect = Users.AllowDelete = true;
            if (isAnon || isEntity) {
                Users.AllowInsert = Users.AllowUpdate = Users.AllowSelect = Users.AllowDelete = false;
                //CampaignMembers.AllowInsert = Users.AllowUpdate = Users.AllowSelect = Users.AllowDelete = false;
                insertSampleCollector.SetEnabled(false);
                insertSampleCollector.SetVisible(false);
                //redirectToSurvey.SetEnabled(false);
                //redirectToSurvey.SetVisible(false);
            }
        }

        public virtual bool HasDetailRecords() {
            if (ActivePages.Current != null) {
                return true;
            }
            if (Survey.Cache.GetStatus(Survey.Current) == PXEntryStatus.Inserted) {
                return ActivePages.Cache.IsDirty;
            }
            return ActivePages.Select(Array.Empty<object>()).Count > 0;
        }

        public virtual bool HasAnswerRecords() {
            if (Answers.Current != null) {
                return true;
            }
            if (Survey.Cache.GetStatus(Survey.Current) == PXEntryStatus.Inserted) {
                return Answers.Cache.IsDirty;
            }
            return Answers.Select(Array.Empty<object>()).Count > 0;
        }

        public virtual bool HasUnprocessedCollectors() {
            if (UnprocessedCollectorDataRecords.Current != null) {
                return true;
            }
            if (Survey.Cache.GetStatus(Survey.Current) == PXEntryStatus.Inserted) {
                return UnprocessedCollectorDataRecords.Cache.IsDirty;
            }
            return UnprocessedCollectorDataRecords.Select(Array.Empty<object>()).Count > 0;
        }

        protected virtual void _(Events.FieldUpdated<Survey, Survey.layout> e) {
            var row = e.Row;
            if (row == null) { return; }
        }

        public void _(Events.FieldUpdated<Survey, Survey.target> e) {
            e.Cache.SetDefaultExt<Survey.allowAnonymous>(e.Row);
            e.Cache.SetDefaultExt<Survey.keepAnswersAnonymous>(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<Survey, Survey.allowAnonymous> e) {
            var row = e.Row;
            if (row == null || string.IsNullOrEmpty(row.Target)) {
                return;
            }
            e.NewValue = row.Target == SurveyTarget.Anonymous;
            e.Cancel = true;
        }

        protected virtual void _(Events.FieldDefaulting<Survey, Survey.keepAnswersAnonymous> e) {
            var row = e.Row;
            if (row == null || string.IsNullOrEmpty(row.Target)) {
                return;
            }
            e.NewValue = row.Target == SurveyTarget.Anonymous;
            e.Cancel = true;
        }

        protected virtual void _(Events.FieldDefaulting<Survey, Survey.baseURL> e) {
            var row = e.Row;
            if (row == null) {
                return;
            }
            var generator = new SurveyGenerator();
            var url = generator.GetUrl();
            e.NewValue = url;
        }

        protected virtual void _(Events.FieldSelecting<Survey, Survey.anonURL> e) {
            var row = e.Row;
            if (row == null) {
                return;
            }
            var generator = new SurveyGenerator();
            var url = generator.GetUrl(row, row.SurveyID, null);
            e.ReturnValue = url;
        }

        protected virtual void _(Events.RowPersisting<SurveyCollector> e) {
            var row = e.Row;
            var survey = Survey.Current;
            if (row == null || survey == null || e.Operation == PXDBOperation.Delete)
                return;
            //if (survey.KeepAnswersAnonymous == true && row.AnonCollectorID == null && row.Anonymous != true) {
            //    var (user, anon) = SurveyUtils.InsertAnonymous(this, survey, null, false);
            //    row.AnonCollectorID = anon?.CollectorID;
            //}
        }

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
                    And<SurveyDetail.componentType, Equal<Required<SurveyDetail.componentType>>>>>.Select(this, surveyID, templateType);
        }

        //private SurveyDetail GetPage(string surveyID, int? pageNbr, string templateType) {
        //    return PXSelect<SurveyDetail,
        //            Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>,
        //            And<SurveyDetail.pageNbr, Equal<Required<SurveyDetail.pageNbr>>,
        //            And<SurveyDetail.templateType, Equal<Required<SurveyDetail.templateType>>>>>>.Select(this, surveyID, pageNbr, templateType);
        //}

        private PXResultset<SurveyDetail> GetRegularPages(string surveyID) {
            return PXSelect<SurveyDetail,
                    Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>,
                    And<SurveyDetail.componentType, In3<SUComponentType.pageHeader, SUComponentType.questionPage, SUComponentType.contentPage, SUComponentType.pageFooter>>>,
                    OrderBy<Asc<SurveyDetail.pageNbr, Asc<SurveyDetail.sortOrder>>>>.Select(this, surveyID);
        }

        protected virtual void _(Events.RowSelected<SurveyDetail> e) {
            var row = e.Row;
            if (row == null || Survey.Current == null || Survey.Current.Layout == null) {
                return;
            }
            var isMulti = Survey.Current.Layout == SurveyLayout.MultiPage;
            PXUIFieldAttribute.SetEnabled<SurveyDetail.pageNbr>(e.Cache, row, isMulti);
            var survey = Survey.Current;
            bool lockedSurvey = SurveyStatus.IsLocked(survey?.Status);
            e.Cache.AllowDelete = !lockedSurvey;
            e.Cache.AllowUpdate = !lockedSurvey;
            e.Cache.AllowInsert = !lockedSurvey;
        }

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.description> e) {
            var row = e.Row;
            if (row == null || row.ComponentID == null || !string.IsNullOrEmpty(row.Description)) {
                return;
            }
            SurveyComponent comp = SurveyComponent.PK.Find(this, row.ComponentID);
            if (comp == null) {
                return;
            }
            switch (comp.ComponentType) {
                case SUComponentType.PageHeader:
                case SUComponentType.PageFooter:
                    e.NewValue = null;
                    break;
                case SUComponentType.Header:
                    e.NewValue = "HELLO YOU";
                    break;
                case SUComponentType.QuestionPage:
                    e.NewValue = (row.QuestionNbr > 0) ? "LET ME ASK YOU THIS " + row.QuestionNbr : null;
                    break;
                case SUComponentType.CommentPage:
                    e.NewValue = (row.QuestionNbr > 0) ? "TELL ME MORE " + row.QuestionNbr : null;
                    break;
                case SUComponentType.ContentPage:
                    e.NewValue = "SHOW YOU";
                    break;
                case SUComponentType.Footer:
                    e.NewValue = "THANK YOU";
                    break;
            }
            e.Cancel = true;
        }

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.componentID> e) {
            var row = e.Row;
            if (row == null || row.ComponentType == null) {
                return;
            }
            var setup = SurveySetup.Current;
            switch (row.ComponentType) {
                case SUComponentType.Header:
                    e.NewValue = setup.DefHeaderID;
                    break;
                case SUComponentType.PageHeader:
                    e.NewValue = setup.DefPageHeaderID;
                    break;
                case SUComponentType.PageFooter:
                    e.NewValue = setup.DefPageFooterID;
                    break;
                case SUComponentType.QuestionPage:
                    e.NewValue = setup.DefQuestionID;
                    break;
                case SUComponentType.CommentPage:
                    e.NewValue = setup.DefCommentID;
                    break;
                case SUComponentType.Footer:
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

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.required> e) {
            var row = e.Row;
            if (row == null) {
                return;
            }
            e.NewValue = (row.ComponentType == SUComponentType.QuestionPage);
        }

        protected virtual void _(Events.FieldUpdated<SurveyDetail, SurveyDetail.componentType> e) {
            e.Cache.SetDefaultExt<SurveyDetail.nbrOfRows>(e.Row);
            e.Cache.SetDefaultExt<SurveyDetail.maxLength>(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.nbrOfRows> e) {
            var row = e.Row;
            if (row == null || row.ComponentType == null) {
                return;
            }
            var setup = SurveySetup.Current;
            e.NewValue = (row.ComponentType == SUComponentType.CommentPage) ? setup?.DefNbrOfRows : null;
            e.Cancel = true;
        }

        protected virtual void _(Events.FieldDefaulting<SurveyDetail, SurveyDetail.maxLength> e) {
            var row = e.Row;
            if (row == null || row.ComponentType == null) {
                return;
            }
            var setup = SurveySetup.Current;
            e.NewValue = (row.ComponentType == SUComponentType.CommentPage) ? setup?.DefMaxLength : null;
            e.Cancel = true;
        }

        private int GetMaxPage(string surveyID) {
            var maxPage = PXSelect<SurveyDetail, Where<SurveyDetail.surveyID, Equal<Required<SurveyDetail.surveyID>>>>.Select(this, surveyID).FirstTableItems.Select(sd => sd.PageNbr).Max();
            return maxPage ?? 0;
        }

        protected virtual void _(Events.RowSelected<SurveyUser> e) {
            var row = e.Row;
            if (row == null) { return; }
            // TODO Lock Recipient for delete if collector is found
        }

        public void _(Events.FieldUpdated<SurveyCollector, SurveyCollector.collectorID> e) {
            e.Cache.SetDefaultExt<SurveyCollector.token>(e.Row);
        }

        public void _(Events.RowSelected<SurveyCollector> e) {
            var survey = Survey.Current;
            var showRefNote = !string.IsNullOrEmpty(survey?.EntityType);
            var row = e.Row;
            if (row == null) { return; }
            var isClosed = survey.Status == SurveyStatus.Closed;
            var notifSent = row.SentOn.HasValue;
            sendNewNotification.SetEnabled(!notifSent && !isClosed);
            sendReminder.SetEnabled(notifSent && !isClosed);
            PXUIFieldAttribute.SetVisible<SurveyCollector.refNoteID>(e.Cache, e.Row, showRefNote);
            PXUIFieldAttribute.SetVisible<SurveyCollector.source>(e.Cache, e.Row, showRefNote);
            var hasPages = HasDetailRecords();
            redirectToSurvey.SetEnabled(e.Row != null && hasPages);
        }

        //protected virtual void _(Events.FieldDefaulting<SurveyCollector, SurveyCollector.token> e) {
        //    var row = e.Row;
        //    if (row == null || row.CollectorID == null) {
        //        return;
        //    }
        //    e.NewValue = Net_Utils.ComputeMd5(row.CollectorID.ToString(), true);
        //    e.Cancel = true;
        //}

        protected virtual void _(Events.RowDeleted<SurveyCollector> e) {
            var row = e.Row;
            if (row == null) {
                return;
            }
            // Deleting a row which is the anonymous collector of another resets the AnonCollectorID
            if (row.CollectorID != null) {
                var realCollector = SurveyCollector.UK.ByAnonCollectorID.Find(this, row.CollectorID);
                if (realCollector != null) {
                    realCollector.AnonCollectorID = null;
                    this.Collectors.Update(realCollector);
                }
            }
            // Deleting a row which has an AnonCollectorID also deletes the anonymous collector
            if (row.AnonCollectorID != null) {
                var anonCollector = SurveyCollector.PK.Find(this, row.AnonCollectorID);
                if (anonCollector != null) {
                    this.Collectors.Delete(anonCollector);
                }
            }
        }

        protected virtual void _(Events.RowDeleted<SurveyCollectorData> e) {
            var row = e.Row;
            if (row == null) {
                return;
            }
            var answers = GetAnswers(row);
            foreach (var answer in answers) {
                Answers.Delete(answer);
            }
            var coll = GetCollector(row);
            if (coll != null) {
                coll.Status = CollectorStatus.Deleted;
                Collectors.Update(coll);
            }
        }

        private PXResultset<SurveyAnswer> GetAnswers(SurveyCollectorData row) {
            var answers = PXSelect<SurveyAnswer,
                Where<SurveyAnswer.collectorID, Equal<Required<SurveyCollector.collectorID>>,
                And<SurveyAnswer.surveyID, Equal<Required<SurveyAnswer.surveyID>>>>>.Select(this, row.CollectorID, row.SurveyID);
            return answers;
        }

        private SurveyCollector GetCollector(SurveyCollectorData row) {
            SurveyCollector coll = PXSelect<SurveyCollector,
                Where<SurveyCollector.token, Equal<Required<SurveyCollector.token>>,
                And<SurveyCollector.surveyID, Equal<Required<SurveyCollectorData.surveyID>>>>>.Select(this, row.Token, row.SurveyID);
            return coll;
        }

        public void _(Events.RowSelected<SurveyAnswer> e) {
            var hasAnswers = HasAnswerRecords();
            reProcessAnswers.SetEnabled(hasAnswers);
            var hasUnProcAnswers = HasUnprocessedCollectors();
            processAnswers.SetEnabled(hasUnProcAnswers);
        }

        protected virtual void _(Events.FieldUpdating<SurveyAnswer, SurveyAnswer.value> e) {
            var row = e.Row;
            if (row == null || row.AttributeID == null) {
                return;
            }
            int? controlType;
            //bool flag;
            //int num;
            DateTime dateTime;
            CRAttribute.Attribute item = CRAttribute.Attributes[row.AttributeID];
            if (item == null) {
                return;
            }
            object newValue = e.NewValue;
            if (newValue is DateTime) {
                DateTime dateTime1 = (DateTime)newValue;
                controlType = item.ControlType;
                if (controlType.GetValueOrDefault() == 5 & controlType.HasValue) {
                    e.NewValue = dateTime1.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    return;
                }
            }
            newValue = e.NewValue;
            string str = newValue as string;
            if (str == null) {
                return;
            }
            controlType = item.ControlType;
            if (controlType.HasValue) {
                switch (controlType.GetValueOrDefault()) {
                    case 2:
                        CRAttribute.AttributeValue attributeValue = item.Values.Find((CRAttribute.AttributeValue a) => string.Equals(a.ValueID, str, StringComparison.OrdinalIgnoreCase)) ?? item.Values.Find((CRAttribute.AttributeValue a) => string.Equals(a.Description, str, StringComparison.OrdinalIgnoreCase));
                        if (attributeValue != null) {
                            e.NewValue = attributeValue.ValueID;
                            return;
                        }
                        e.Cache.RaiseExceptionHandling<SurveyAnswer.value>(row, str, new PXSetPropertyException("The specified value is not valid for the {0} attribute that has the Combo type.", new object[] { row.AttributeID }));
                        break;
                    case 3:
                        break;
                    case 4:
                        if (bool.TryParse(str, out var flag)) {
                            var num = Convert.ToInt32(flag);
                            e.NewValue = num.ToString(CultureInfo.InvariantCulture);
                            return;
                        }
                        if (!(str != "0") || !(str != "1")) {
                            break;
                        }
                        e.Cache.RaiseExceptionHandling<SurveyAnswer.value>(row, str, new PXSetPropertyException("The specified value is not valid for the {0} attribute that has the Checkbox type because it cannot be converted to a boolean value.", new object[] { row.AttributeID }));
                        return;
                    case 5:
                        if (e.Cache.Graph.IsMobile) {
                            str = str.Replace("Z", "");
                        }
                        if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) {
                            e.NewValue = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            return;
                        }
                        e.Cache.RaiseExceptionHandling<SurveyAnswer.value>(row, str, new PXSetPropertyException("The specified value is not valid for the {0} attribute that has the Datetime type because it cannot be converted to a DateTime value.", new object[] { row.AttributeID }));
                        return;
                    case 6:
                        if (string.IsNullOrEmpty(str)) {
                            break;
                        }
                        string[] strArrays = str.Split(new char[] { ',' });
                        for (var num = 0; num < (int)strArrays.Length; num++) {
                            string str1 = strArrays[num];
                            if (item.Values.Find((CRAttribute.AttributeValue a) => string.Equals(a.ValueID, str1, StringComparison.OrdinalIgnoreCase)) == null) {
                                e.Cache.RaiseExceptionHandling<SurveyAnswer.value>(row, str, new PXSetPropertyException("One of the specified values is not valid for the {0} attribute that has the Multi Select Combo type. Note that the Multi Select Combo type supports identifiers and does not support descriptions.", new object[] { row.AttributeID }));
                                return;
                            }
                        }
                        return;
                    default:
                        return;
                }
            }
        }

        [PXCacheName("CreateSurveyFilter")]
        [Serializable]
        public class CreateSurveyFilter : IBqlTable {

            #region NbQuestions
            public abstract class nbQuestions : BqlInt.Field<nbQuestions> { }
            [PXInt]
            [PXUnboundDefault(1)]
            [PXUIField(DisplayName = "Nbr of Question Place Holders")]
            public virtual int? NbQuestions { get; set; }
            #endregion
        }
    }
}