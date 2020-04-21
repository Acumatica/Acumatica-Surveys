using System;
using System.Collections;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Survey.Ext
{
    public class SurveyCollectorMaint : PXGraph<SurveyCollectorMaint>
    {
        public SelectFrom<SurveyCollector>.View SurveyQuestions;

        public CRAttributeList<SurveyCollector> Answers;

        public PXCancel<SurveyCollector> Cancel;
        public PXSave<SurveyCollector> Save;

        protected void _(Events.RowSelected<SurveyCollector> e)
        {
            bool bEnabled = (SurveyQuestions.Current.CollectorStatus == SurveyResponseStatus.CollectorSent ||
                             SurveyQuestions.Current.CollectorStatus == SurveyResponseStatus.CollectorNew);
            Submit.SetEnabled(bEnabled);
            Answers.Cache.AllowUpdate = (bEnabled);

            ReOpen.SetEnabled(SurveyQuestions.Current.CollectorStatus == SurveyResponseStatus.CollectorResponded);

            PXUIFieldAttribute.SetDisplayName<CSAnswers.attributeID>(Answers.Cache, Messages.Question);
            PXUIFieldAttribute.SetDisplayName<CSAnswers.value>(Answers.Cache, Messages.Answer);
        }

        public PXAction<SurveyCollector> Submit;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = Messages.Submit, MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable submit(PXAdapter adapter)
        {
            Persist();
            var currentQuestion = SurveyQuestions.Current;

            PXLongOperation.StartOperation(this, delegate ()
            {
                SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();
                graph.SurveyQuestions.Current = graph.SurveyQuestions.Search<SurveyCollector.collectorID>(currentQuestion.CollectorID);

                if (graph.Answers.Select().ToList().Any(x => (x.GetItem<CSAnswers>().IsRequired.GetValueOrDefault(false) &&
                                                             (String.IsNullOrEmpty(x.GetItem<CSAnswers>().Value)))))
                {
                    throw new PXException(Messages.AnswerReqiredQuestions);
                }

                graph.SurveyQuestions.Current.CollectorStatus = SurveyResponseStatus.CollectorResponded;
                graph.SurveyQuestions.Current.CollectedDate = PXTimeZoneInfo.Now;
                graph.SurveyQuestions.Update(graph.SurveyQuestions.Current);
                graph.Persist();
            });

            return adapter.Get();
        }

        public PXAction<SurveyCollector> ReOpen;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = Messages.ReOpen, MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable reOpen(PXAdapter adapter)
        {
            Persist();
            var currentQuestion = SurveyQuestions.Current;

            PXLongOperation.StartOperation(this, delegate ()
            {
                SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();
                graph.SurveyQuestions.Current = graph.SurveyQuestions.Search<SurveyCollector.collectorID>(currentQuestion.CollectorID);
                graph.SurveyQuestions.Current.CollectorStatus = SurveyResponseStatus.CollectorSent;
                graph.SurveyQuestions.Current.CollectedDate = null;
                graph.SurveyQuestions.Update(graph.SurveyQuestions.Current);
                graph.Persist();
            });

            return adapter.Get();
        }
    }
}