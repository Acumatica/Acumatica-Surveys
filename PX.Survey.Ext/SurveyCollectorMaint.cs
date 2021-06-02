using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Linq;

namespace PX.Survey.Ext {
    public class SurveyCollectorMaint : PXGraph<SurveyCollectorMaint> {

        public SelectFrom<SurveyCollector>.View Collector;
        public SelectFrom<SurveyCollectorData>.Where<SurveyCollectorData.collectorID.IsEqual<SurveyCollector.collectorID.FromCurrent>>.View CollectedAnswers;
        public SelectFrom<SurveyCollectorData>.Where<SurveyCollectorData.surveyID.IsNull>.View UnprocessedCollectedAnswers;
        //public CRAttributeList<SurveyCollector> Answers;

        public PXCancel<SurveyCollector> Cancel;
        public PXSave<SurveyCollector> Save;

        protected void _(Events.RowSelected<SurveyCollector> e) {
            var row = e.Row;
            if (row == null) {
                return;
            }
            bool bEnabled = (row.Status == CollectorStatus.Sent || row.Status == CollectorStatus.New);
            //Submit.SetEnabled(bEnabled);
            //Answers.Cache.AllowUpdate = (bEnabled);
            ReOpen.SetEnabled(row.Status == CollectorStatus.Responded);
            //PXUIFieldAttribute.SetDisplayName<CSAnswers.attributeID>(Answers.Cache, Messages.Question);
            //PXUIFieldAttribute.SetDisplayName<CSAnswers.value>(Answers.Cache, Messages.Answer);
        }

        //public PXAction<SurveyCollector> Submit;
        //[PXButton(CommitChanges = true)]
        //[PXUIField(DisplayName = Messages.Submit, MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        //public virtual IEnumerable submit(PXAdapter adapter) {
        //    Persist();
        //    var currentQuestion = Collector.Current;
        //    PXLongOperation.StartOperation(this, delegate () {
        //        SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();
        //        graph.Collector.Current = graph.Collector.Search<SurveyCollector.collectorID>(currentQuestion.CollectorID);
        //        if (graph.Answers.Select().ToList().Any(x => (x.GetItem<CSAnswers>().IsRequired.GetValueOrDefault(false) &&
        //                                                     (String.IsNullOrEmpty(x.GetItem<CSAnswers>().Value))))) {
        //            throw new PXException(Messages.AnswerReqiredQuestions);
        //        }
        //        graph.Collector.Current.Status = CollectorStatus.Responded;
        //        graph.Collector.Current.CollectedDate = PXTimeZoneInfo.Now;
        //        graph.Collector.Update(graph.Collector.Current);
        //        graph.Persist();
        //    });
        //    return adapter.Get();
        //}

        public PXAction<SurveyCollector> ReOpen;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = Messages.ReOpen, MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable reOpen(PXAdapter adapter) {
            Persist();
            var currentQuestion = Collector.Current;
            PXLongOperation.StartOperation(this, delegate () {
                SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();
                graph.Collector.Current = graph.Collector.Search<SurveyCollector.collectorID>(currentQuestion.CollectorID);
                graph.Collector.Current.Status = CollectorStatus.Sent;
                graph.Collector.Current.CollectedDate = null;
                graph.Collector.Update(graph.Collector.Current);
                graph.Persist();
            });
            return adapter.Get();
        }
    }
}