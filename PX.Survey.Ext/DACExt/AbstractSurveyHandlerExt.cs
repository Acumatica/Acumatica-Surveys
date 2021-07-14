﻿using PX.Data;
using PX.Data.DependencyInjection;
using System;
using System.Collections;

namespace PX.Survey.Ext {

    public abstract class AbstractSurveyHandlerExt<EGraph, EDoc> : PXGraphExtension<EGraph>, IGraphWithInitialization
        where EGraph : PXGraph
        where EDoc : class, IBqlTable, new() {

        public PXSetup<SurveySetup> SurveySetup;
        public PXSelect<SurveySetupEntity, Where<SurveySetupEntity.entityType, Equal<Required<SurveySetupEntity.entityType>>>> SurveySetupEntity;

        public override void Initialize() {
            base.Initialize();
            var setup = SurveySetup.Current;
            var primaryType = Base.PrimaryItemType;
            var entitySetup = GetEntitySetup(primaryType);
            var supportsSurvey = entitySetup != null && entitySetup.SurveyID != null;
            //var showAction = b2Setup?.ShowTriggerActions == true;
            requestSurvey.SetEnabled(supportsSurvey); // TODO Keep enabled
            //requestSurvey.SetVisible(supportsSurvey/* && showAction*/);
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
                    var collector = surveyGraph.DoUpsertCollector(survey, user, noteID);
                }
            }
            return adapter.Get();
        }

        private SurveySetupEntity GetEntitySetup(Type entityType) {
            var setup = SurveySetupEntity.SelectSingle(entityType.FullName);
            return setup;
        }
    }
}