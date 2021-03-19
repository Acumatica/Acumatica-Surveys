using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Survey.Ext.DAC;

namespace PX.Survey.Ext {
    public class SurveyCollectorDataMaint : PXGraph<SurveyCollectorDataMaint> {
        //assumption: all we should need is a super simple graph as to be able to save Data records

        public SelectFrom<SurveyCollectorData>.View CollectorData;


    }
}
