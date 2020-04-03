using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CR;

namespace Covid19.Lib
{
    public class CovidQuizSetting : PXGraph<CovidQuizSetting, CovidClass>
    {
        public PXSelect<CovidClass> CovidClassCurrent;
        public CSAttributeGroupList<CovidClass.covidClassID, CovidQuiz> Mapping;


    }
}
