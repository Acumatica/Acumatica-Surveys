using System.Collections;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;

namespace Covid19.Lib
{
    public class SurveyQuizEmployeeMaint : PXGraph<SurveyQuizEmployeeMaint>
    {
        public SelectFrom<SurveyCollector>.View Quizes;

        public CRAttributeList<SurveyCollector> Answers;

        public PXCancel<SurveyCollector> Cancel;
        public PXSave<SurveyCollector> Save;

        protected void _(Events.RowSelected<SurveyCollector> e)
        {
            this.Submit.SetEnabled(Quizes.Current.CollectorStatus == SurveyResponseStatus.CollectorSent || 
                                   Quizes.Current.CollectorStatus == SurveyResponseStatus.CollectorNew);
            Answers.Cache.AllowUpdate = (Quizes.Current.CollectorStatus == SurveyResponseStatus.CollectorSent ||
                                         Quizes.Current.CollectorStatus == SurveyResponseStatus.CollectorNew);

            PXUIFieldAttribute.SetDisplayName<CSAnswers.attributeID>(Answers.Cache, "Question");
            PXUIFieldAttribute.SetDisplayName<CSAnswers.value>(Answers.Cache, "Answer");
        }

        public PXAction<SurveyCollector> Submit;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Submit", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable submit(PXAdapter adapter)
        {
            Persist();
            var currentQuiz = Quizes.Current;

            PXLongOperation.StartOperation(this, delegate ()
            {
                SurveyQuizEmployeeMaint graph = PXGraph.CreateInstance<SurveyQuizEmployeeMaint>();
                graph.Quizes.Current = graph.Quizes.Search<SurveyCollector.collectorID>(currentQuiz.CollectorID);
                graph.Quizes.Current.CollectorStatus = SurveyResponseStatus.CollectorResponded;
                graph.Quizes.Current.CollectedDate = PXTimeZoneInfo.Now;
                graph.Quizes.Update(graph.Quizes.Current);
                graph.Persist();
            });

            return adapter.Get();
        }        
    }
}