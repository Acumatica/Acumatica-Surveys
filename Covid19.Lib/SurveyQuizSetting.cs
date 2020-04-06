using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;

namespace Covid19.Lib
{
    public class SurveyQuizSetting : PXGraph<SurveyQuizSetting, SurveyClass>
    {
        public SelectFrom<SurveyClass>.View SurveyClassCurrent;
        public CSAttributeGroupList<SurveyClass.surveyClassID, SurveyQuiz> Mapping;
    }
}
