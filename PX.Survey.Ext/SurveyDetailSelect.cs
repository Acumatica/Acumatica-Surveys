using PX.Data;
using System;

namespace PX.Survey.Ext {

    [PXDynamicButton(
        new string[] { PasteLineCommand, ResetOrderCommand },
        new string[] { Messages.SurveyDetailPasteLineCommand, Messages.SurveyDetailResetOrderCommand },
        TranslationKeyType = typeof(Messages))]
    public class SurveyDetailSelect :
        PXOrderedSelect<Survey, SurveyDetail,
        Where<SurveyDetail.surveyID, Equal<Current<Survey.surveyID>>>,
        OrderBy<Asc<SurveyDetail.sortOrder, Asc<SurveyDetail.lineNbr>>>> {

        public new const string PasteLineCommand = "SurveyDetailPasteLine";
        public new const string ResetOrderCommand = "SurveyDetailResetOrder";

        public SurveyDetailSelect(PXGraph graph) : base(graph) { }

        public SurveyDetailSelect(PXGraph graph, Delegate handler) : base(graph, handler) { }

        protected override void AddActions(PXGraph graph) {
            AddAction(graph, PasteLineCommand, Messages.SurveyDetailPasteLineCommand, new PXButtonDelegate(PasteLine));
            AddAction(graph, ResetOrderCommand, Messages.SurveyDetailResetOrderCommand, new PXButtonDelegate(ResetOrder));
        }
    }
}
