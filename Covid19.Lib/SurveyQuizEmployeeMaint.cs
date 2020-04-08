using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;

namespace Covid19.Lib
{
    public class SurveyQuizEmployeeMaint : PXGraph<SurveyQuizEmployeeMaint, SurveyCollector>
    {
        public SelectFrom<SurveyCollector>.View Quizes;
        public CRAttributeList<SurveyCollector> Answers;

        protected void _(Events.RowSelected<SurveyCollector> e)
        {
            this.Actions["Insert"].SetVisible(false);
            this.Actions["Delete"].SetVisible(false);
            this.Actions["CopyPaste"].SetVisible(false);
            this.Actions["First"].SetVisible(false);
            this.Actions["Previous"].SetVisible(false);
            this.Actions["Next"].SetVisible(false);
            this.Actions["Last"].SetVisible(false);
            this.Submit.SetEnabled(Quizes.Current.CollectorStatus == "S");

        }

        public PXAction<SurveyCollector> Submit;

        [PXButton]
        [PXUIField(DisplayName = "Submit", MapViewRights = PXCacheRights.Select,
            MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable submit(PXAdapter adapter)
        {
            Persist();
            var currentQuiz = Quizes.Current;
            currentQuiz.CollectorStatus = "R";
            currentQuiz.CollectedDate = DateTime.Now;
            Quizes.Cache.Update(currentQuiz);
            Persist();
            return adapter.Get();
        }
        
    }
}
