using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CR;

namespace Covid19.Lib
{
    public class CovidQuizEmployeeMaint : PXGraph<CovidQuizEmployeeMaint, CovidQuiz>
    {
        public PXSelect<CovidQuiz> Quizes;
        public CRAttributeList<CovidQuiz> Answers;

        protected void _(Events.FieldDefaulting<CovidQuiz, CovidQuiz.quizCD> e)
        {
            var row = e.Row;
            e.NewValue = PXAccess.GetUserName() + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        protected void _(Events.FieldDefaulting<CovidQuiz, CovidQuiz.quizedUser> e)
        {
            var row = e.Row;
            e.NewValue = PXAccess.GetUserID();
        }
    }
}
