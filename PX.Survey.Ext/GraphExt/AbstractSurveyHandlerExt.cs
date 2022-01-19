using PX.Data;
using PX.Data.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Survey.Ext {

    public abstract class AbstractSurveyHandlerExt<EGraph, EDoc> : PXGraphExtension<EGraph>, IGraphWithInitialization
        where EGraph : PXGraph
        where EDoc : class, IBqlTable, new() {

        private static string ACTION_PREFIX = "Survey-";

        public PXSetup<SurveySetup> SurveySetup;
        //public PXSelect<SurveySetupEntity, Where<SurveySetupEntity.entityType, Equal<Required<SurveySetupEntity.entityType>>>> SurveySetupEntity;
        public PXSelect<SurveySetupEntity,
            Where<SurveySetupEntity.entityType, Equal<Required<SurveySetupEntity.entityType>>,
            And<SurveySetupEntity.surveyID, IsNotNull>>> SurveyIntegrations;
        public PXAction<EDoc> SurveyFolder;

        public override void Initialize() {
            base.Initialize();
            var primaryType = Base.PrimaryItemType;
            // Acuminator disable once PX1085 DatabaseQueriesInPXGraphInitialization [Justification]
            var integrations = GetSurveyIntegrations(primaryType);
            AddIntegrations(integrations);
        }

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
                fieldName = Clean(fieldName);
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

        private string Clean(string fieldName) {
            if (string.IsNullOrEmpty(fieldName)) {
                return fieldName;
            }
            if (fieldName.StartsWith("((")) {
                fieldName = fieldName.Substring(2);
            }
            if (fieldName.EndsWith("))")) {
                fieldName = fieldName.Substring(0, fieldName.Length - 2);
            }
            return fieldName;
        }

        private PXAction AddAction(string actionName, string displayName, PXButtonDelegate handler) {
            var action = PXNamedAction<EDoc>.AddAction(Base, actionName, displayName, handler);
            SurveyFolder.AddMenuAction(action);
            return action;
        }
    }
}