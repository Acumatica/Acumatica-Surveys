using PX.Data;
using PX.Data.DependencyInjection;
using PX.Data.Wiki.Parser;
using PX.Objects.CN.Common.Extensions;
using PX.PushNotifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Survey.Ext {

    public abstract class AbstractSurveyHandlerExt<EGraph, EDoc> : PXGraphExtension<EGraph>, IGraphWithInitialization
        where EGraph : PXGraph
        where EDoc : class, IBqlTable, new() {

        private static string ACTION_PREFIX = "Survey-";

        public PXSetup<SurveySetup> SurveySetup;
        public PXSelect<SurveySetupEntity, Where<SurveySetupEntity.entityType, Equal<Required<SurveySetupEntity.entityType>>>> SurveySetupEntity;
        public PXSelect<SurveySetupEntity,
            Where<SurveySetupEntity.entityType, Equal<Required<SurveySetupEntity.entityType>>,
            And<SurveySetupEntity.surveyID, IsNotNull>>> SurveyIntegrations;
        //public SurveyFolder<EDoc> SurveyFolder;
        public PXAction<EDoc> SurveyFolder;

        public override void Initialize() {
            base.Initialize();
            var primaryType = Base.PrimaryItemType;
            // Acuminator disable once PX1085 DatabaseQueriesInPXGraphInitialization [Justification]
            //var entitySetup = GetEntitySetup(primaryType);
            //var supportsSurvey = entitySetup != null && entitySetup.SurveyID != null;
            //requestSurvey.SetEnabled(supportsSurvey); // TODO Keep enabled
            // Acuminator disable once PX1085 DatabaseQueriesInPXGraphInitialization [Justification]
            var integrations = GetSurveyIntegrations(primaryType);
            AddIntegrations(integrations);
        }

        //[PXButton(MenuAutoOpen = true, SpecialType = PXSpecialButtonType.ReportsFolder)]
        //[PXUIField(DisplayName = "Surveys")]
        //public virtual IEnumerable surveyFolder(PXAdapter adapter) {
        //    return adapter.Get();
        //}

        protected void _(Events.RowSelected<EDoc> e) {
            // TODO Enable, get all actions StartsWith ACTION_PREFIX
            // Read survey
            //var name = "Survey-";// + survey.SurveyID;
            //disable all survey actions
            //pXNamedAction.SetEnabled(status != SurveyStatus.Preparing);
        }

        [PXButton(CommitChanges = true, MenuAutoOpen = true)]
        [PXUIField(DisplayName = "Surveys")]
        protected virtual void surveyFolder() {
        }

        // TODO Add action under surveyFolder to Refresh survey action list (remove and add back)




        //public PXAction<EDoc> requestSurvey;
        //[PXUIField(DisplayName = "Request Survey")]
        //[PXButton(CommitChanges = true, SpecialType = PXSpecialButtonType.Insert, Tooltip = "Insert a Survey Collector to send a survey regarding this Document", ImageKey = "AddNew")]
        //public virtual IEnumerable RequestSurvey(PXAdapter adapter) {
        //    // TODO - Popup message if not setup!
        //    var caches = Base.Caches;
        //    var cache = caches[typeof(EDoc)];
        //    var entityType = cache.GetItemType();
        //    var entitySetup = GetEntitySetup(entityType);
        //    var doc = cache.Current;
        //    var noteID = PXNoteAttribute.GetNoteIDIfExists(cache, doc);
        //    if (entitySetup != null && entitySetup.SurveyID != null && noteID.HasValue) {
        //        var contactID = (int?)cache.GetValue(doc, entitySetup.ContactField);
        //        var surveyGraph = PXGraph.CreateInstance<SurveyMaint>();
        //        var survey = surveyGraph.Survey.Search<Survey.surveyID>(entitySetup.SurveyID);
        //        if (survey != null && contactID.HasValue) {
        //            surveyGraph.Survey.Current = survey;
        //            var user = surveyGraph.InsertOrFindUser(survey, contactID, false);
        //            var collector = surveyGraph.DoUpsertCollector(survey, user, noteID, true);
        //        }
        //    }
        //    return adapter.Get();
        //}

        //private SurveySetupEntity GetEntitySetup(Type entityType) {
        //    var setup = SurveySetupEntity.SelectSingle(entityType.FullName);
        //    return setup;
        //}

        private IEnumerable<SurveySetupEntity> GetSurveyIntegrations(Type entityType) {
            var setups = SurveyIntegrations.Select(entityType.FullName).FirstTableItems;
            return setups;
        }

        private void AddIntegrations(IEnumerable<SurveySetupEntity> integrations) {
            if (!integrations.Any()) {
                return;
            }
            foreach (SurveySetupEntity integration in integrations) {
                var survey = Survey.PK.Find(Base, integration.SurveyID);
                if (survey.Status == SurveyStatus.Closed) {
                    continue;
                }
                AddAction(Base, survey, integration.ContactField);
            }
        }

        private PXAction AddAction(PXGraph graph, Survey survey, string fieldName) {
            PXButtonDelegate handler = (PXAdapter adapter) => {
                string str1 = (string.IsNullOrEmpty(adapter.CommandArguments) ? graph.PrimaryView : adapter.CommandArguments);
                string primaryView = graph.PrimaryView;
                var surveyID = survey.SurveyID;
                var caches = Base.Caches;
                var cache = caches[typeof(EDoc)];
                var doc = cache?.Current;
                var entityType = cache?.GetItemType();
                var noteID = PXNoteAttribute.GetNoteIDIfExists(cache, doc);
                //PXCache cache = graph.Views[primaryView].Cache;
                //var current = (cache != null ? cache.Current : null);
                //int? contactID = null;
                //if (fieldName.StartsWith("((") && fieldName.EndsWith("))")) {
                //    if (int.TryParse(PXTemplateContentParser.Instance.Process(fieldName, array, graph, primaryView), out var num)) {
                //        contactID = new int?(num);
                //    }
                //} else {
                //    object[] objArray = "ID".CreateArray<object>();
                //    if (int.TryParse(PXTemplateContentParser.Instance.Process("(("+fieldName+ "))", graph, entityType, objArray), out var num)) {
                //        contactID = new int?(num);
                //    }
                //}
                var contactID = (int?)cache?.GetValue(doc, fieldName);
                var surveyGraph = PXGraph.CreateInstance<SurveyMaint>();
                if (survey != null && contactID.HasValue && noteID.HasValue) {
                    surveyGraph.Survey.Current = survey;
                    var user = surveyGraph.InsertOrFindUser(survey, contactID, false);
                    var collector = surveyGraph.DoUpsertCollector(survey, user, noteID, true, false);
                }
                return adapter.Get();
            };
            var actionName = ACTION_PREFIX + survey.SurveyID;
            var displayName = survey.Title; // TODO Add " -> Ship-To Contact"
            return AddAction(actionName, displayName, handler);
        }

        private PXAction AddAction(string actionName, string displayName, PXButtonDelegate handler) {
            PXUIFieldAttribute pXUIFieldAttribute = new PXUIFieldAttribute() {
                DisplayName = PXMessages.LocalizeNoPrefix(displayName),
                MapEnableRights = PXCacheRights.Select,
            };
            PXNamedAction<EDoc> pXNamedAction = new PXNamedAction<EDoc>(Base, actionName, handler, (new List<PXEventSubscriberAttribute>()
            {
                pXUIFieldAttribute
            }).ToArray());
            //Base.Actions[name] = pXNamedAction;
            // TODO Find Inquiries or Actions and go after
            SurveyFolder.AddMenuAction(pXNamedAction);
            return pXNamedAction;
        }
    }
}