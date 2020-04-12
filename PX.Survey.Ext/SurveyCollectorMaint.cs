using System.Collections;
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
                graph.SurveyQuestions.Current.CollectorStatus = SurveyResponseStatus.CollectorResponded;
                graph.SurveyQuestions.Current.CollectedDate = PXTimeZoneInfo.Now;
                graph.SurveyQuestions.Update(graph.SurveyQuestions.Current);
                graph.Persist();
            });

            return adapter.Get();
        }        
    }
}