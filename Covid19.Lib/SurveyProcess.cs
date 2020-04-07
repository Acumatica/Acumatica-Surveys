using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;


namespace Covid19.Lib
{
    public class SurveyProcess : PXGraph<SurveyProcess>
    {
        #region Constants
        public class newSurvey : Constant<string>
        {
            public newSurvey()
            : base("N")
            {
            }
        }
        #endregion

        public PXCancel<SurveyFilter> Cancel;
        public PXFilter<SurveyFilter> Filter;
        //public PXSelect<SurveyCollector> Records;
        public PXFilteredProcessing<SurveyCollector, SurveyFilter,
        Where<SurveyCollector.collectorStatus,Equal<SurveyProcess.newSurvey>,
        //    Where<True,Equal<True>,
                And<SurveyCollector.surveyID, Equal<Current<SurveyFilter.surveyID>>>>> Records;
        

        public SurveyProcess()
        {
            Records.SetProcessCaption("Send");
            Records.SetProcessAllCaption("Send All");
            Records.SetProcessDelegate<SurveyCollectionMaint>(
            delegate (SurveyCollectionMaint graph, SurveyCollector surveyCollector)
            {
                graph.Clear();
                graph.AssignSetStatus(surveyCollector, true);
            });
        }




        #region SurveyFilter
        [Serializable]
        public class SurveyFilter : IBqlTable
        {
            #region SurveyID
            public abstract class surveyID : PX.Data.IBqlField
            {
            }
            [PXDBInt()]
            [PXDefault()]
            [PXUIField(DisplayName = "Survey ID")]
            [PXSelector(typeof(Search<Surveys.surveyID>),
                typeof(Surveys.surveyName),
                typeof(Surveys.surveyDesc),
                DescriptionField = typeof(Surveys.surveyName))]
            public virtual int? SurveyID { get; set; }
            #endregion
        }

        #endregion

    }
}