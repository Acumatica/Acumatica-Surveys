using PX.Data;
using PX.Data.DependencyInjection;
using System;
using System.Collections;

namespace PX.Survey.Ext {

    public abstract class AbstractSurveyHandlerExt<EGraph, EDoc> : PXGraphExtension<EGraph>, IGraphWithInitialization
        where EGraph : PXGraph
        where EDoc : class, IBqlTable, new() {

        public PXSetup<SurveySetup> SurveySetup;
        public PXSelect<SurveySetupEntity, Where<SurveySetupEntity.entityType, Equal<Required<SurveySetupEntity.entityType>>>> SurveySetupEntity;
        public PXSelect<SurveySetupEntity,
            Where<SurveySetupEntity.entityType, Equal<Required<SurveySetupEntity.entityType>>,
            And<SurveySetupEntity.surveyID, IsNotNull>>> SurveyIntegrations;

        public override void Initialize() {
            base.Initialize();
            var primaryType = Base.PrimaryItemType;
            var entitySetup = GetEntitySetup(primaryType);
            var supportsSurvey = entitySetup != null && entitySetup.SurveyID != null;
            requestSurvey.SetEnabled(supportsSurvey); // TODO Keep enabled
            //var integrations = GetSurveyIntegrations(primaryType);
            //AddIntegrations(integrations);
        }

        public PXAction<EDoc> requestSurvey;
        [PXUIField(DisplayName = "Request Survey")]
        [PXButton(CommitChanges = true, SpecialType = PXSpecialButtonType.Insert, Tooltip = "Insert a Survey Collector to send a survey regarding this Document", ImageKey = "AddNew")]
        public virtual IEnumerable RequestSurvey(PXAdapter adapter) {
            // TODO - Popup message if not setup!
            var caches = Base.Caches;
            var cache = caches[typeof(EDoc)];
            var entityType = cache.GetItemType();
            var entitySetup = GetEntitySetup(entityType);
            var doc = cache.Current;
            var noteID = PXNoteAttribute.GetNoteIDIfExists(cache, doc);
            if (entitySetup != null && entitySetup.SurveyID != null && noteID.HasValue) {
                var contactID = (int?)cache.GetValue(doc, entitySetup.ContactField); // REDO to use TableName.FieldName
                var surveyGraph = PXGraph.CreateInstance<SurveyMaint>();
                var survey = surveyGraph.Survey.Search<Survey.surveyID>(entitySetup.SurveyID);
                if (survey != null && contactID.HasValue) {
                    surveyGraph.Survey.Current = survey;
                    var user = surveyGraph.InsertOrFindUser(survey, contactID, false);
                    var collector = surveyGraph.DoUpsertCollector(survey, user, noteID, true);
                }
            }
            return adapter.Get();
        }

        private SurveySetupEntity GetEntitySetup(Type entityType) {
            var setup = SurveySetupEntity.SelectSingle(entityType.FullName);
            return setup;
        }

        //private IEnumerable<SurveySetupEntity> GetSurveyIntegrations(Type entityType) {
        //    var setups = SurveyIntegrations.Select(entityType.FullName).FirstTableItems;
        //    return setups;
        //}

        //private void AddIntegrations(IEnumerable<SurveySetupEntity> integrations) {
        //    if (!integrations.Any()) {
        //        return;
        //    }
        //    foreach (SurveySetupEntity integration in integrations) {
        //        var survey = Survey.PK.Find(Base, integration.SurveyID);
        //        //AddAction(Base, survey);
        //    }
        //}

        //private PXAction AddAction(PXGraph graph, Survey survey) {
        //    var name = survey.SurveyID;
        //    var displayName = survey.Title;
        //    PXButtonDelegate handler = (PXAdapter adapter) => {
        //        string str1 = (string.IsNullOrEmpty(adapter.CommandArguments) ? graph.PrimaryView : adapter.CommandArguments);
        //        string primaryView = graph.PrimaryView;
        //        PXCache cache = graph.Views[primaryView].Cache;
        //        var current = (cache != null ? cache.Current : null);
        //        return adapter.Get();
        //    };
        //    return AddAction(name, displayName, handler);
        //}

        //private PXAction AddAction(string name, string displayName, PXButtonDelegate handler) {
        //    PXUIFieldAttribute pXUIFieldAttribute = new PXUIFieldAttribute() {
        //        DisplayName = PXMessages.LocalizeNoPrefix(displayName),
        //        MapEnableRights = PXCacheRights.Select
        //    };
        //    PXNamedAction<EDoc> pXNamedAction = new PXNamedAction<EDoc>(Base, name, handler, (new List<PXEventSubscriberAttribute>()
        //    {
        //        pXUIFieldAttribute
        //    }).ToArray());
        //    Base.Actions[name] = pXNamedAction;
        //    return pXNamedAction;
        //}
    }
}