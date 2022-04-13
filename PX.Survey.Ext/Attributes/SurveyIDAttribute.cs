using PX.Data;
using PX.Objects.GL;

namespace PX.Survey.Ext {

    [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
    [PXUIField(DisplayName = "Survey ID", Visibility = PXUIVisibility.SelectorVisible)]
    public class SurveyIDAttribute : AcctSubAttribute {
        public SurveyIDAttribute() {
        }
    }
}
