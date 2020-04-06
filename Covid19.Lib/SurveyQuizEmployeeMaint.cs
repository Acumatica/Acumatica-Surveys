using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CR;

namespace Covid19.Lib
{
    public class SurveyQuizEmployeeMaint : PXGraph<SurveyQuizEmployeeMaint, SurveyQuiz>
    {
        public PXSelect<SurveyQuiz> Quizes;
        public CRAttributeList<SurveyQuiz> Answers;

        protected void _(Events.FieldDefaulting<SurveyQuiz, SurveyQuiz.quizCD> e)
        {
            var row = e.Row;
            e.NewValue = PXAccess.GetUserName() + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        protected void _(Events.FieldDefaulting<SurveyQuiz, SurveyQuiz.quizedUser> e)
        {
            var row = e.Row;
            e.NewValue = PXAccess.GetUserID();
        }
    }
}
