using PX.Data;
using PX.Data.EP;

namespace PX.Survey.Ext {

    public class SUEntityTypeListAttribute : PXEntityTypeListAttribute {
        
        public override void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e) {
            //PXStringState stateExt = sender.GetStateExt(e.Row, this._FieldName) as PXStringState;
            //if (stateExt == null || stateExt.AllowedValues == null || stateExt.AllowedValues.Length == 0) {
            //    base.FieldDefaulting(sender, e);
            //    return;
            //}
            //e.NewValue = stateExt.AllowedValues[0];
        }
    }
}
