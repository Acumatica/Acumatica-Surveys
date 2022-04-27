using PX.Data;
using System;

namespace PX.Survey.Ext {

    [PXDynamicButton(
        new string[] { PasteLineCommand, ResetOrderCommand },
        new string[] { Messages.Command.SetupPasteLine, Messages.Command.SetupResetOrder },
        TranslationKeyType = typeof(Messages))]
    public class SurveySetupEntitySelect :
        PXOrderedSelect<SurveySetup, SurveySetupEntity,
        Where<True, Equal<True>>,
        OrderBy<Asc<SurveySetupEntity.sortOrder, Asc<SurveySetupEntity.lineNbr>>>> {

        public new const string PasteLineCommand = "SetupPasteLine";
        public new const string ResetOrderCommand = "SetupResetOrder";

        public SurveySetupEntitySelect(PXGraph graph)
            : base(graph) {
        }

        public SurveySetupEntitySelect(PXGraph graph, Delegate handler)
            : base(graph, handler) {
        }

        protected override void AddActions(PXGraph graph) {
            AddAction(graph, PasteLineCommand, Messages.Command.SetupPasteLine, new PXButtonDelegate(PasteLine));
            AddAction(graph, ResetOrderCommand, Messages.Command.SetupResetOrder, new PXButtonDelegate(ResetOrder));
        }
    }
}
