using PX.Data;

namespace PX.Survey.Ext {
    public class SurveySetupMaint : PXGraph<SurveySetupMaint> {

        public PXSave<SurveySetup> Save;
        public PXCancel<SurveySetup> Cancel;
        public PXSelect<SurveySetup> surveySetup;
        public PXSelect<SurveySetupEntity> DefaultSurveys;

        //protected virtual void _(Events.RowSelected<SurveySetupEntity> e) {
        //    string screenID;
        //    var row = e.Row;
        //    string str = screenID;
        //    if (row == null || str == null) {
        //        return;
        //    }
        //    string[] strArrays = null;
        //    string[] strArrays1 = null;
        //    foreach (PXEventSubscriberAttribute attribute in sender.GetAttributes(row, "value")) {
        //        PrimaryViewValueListAttribute primaryViewValueListAttribute = attribute as PrimaryViewValueListAttribute;
        //        if (primaryViewValueListAttribute == null) {
        //            continue;
        //        }
        //        bool? fromSchema = row.FromSchema;
        //        if (!(fromSchema.GetValueOrDefault() & fromSchema.HasValue)) {
        //            primaryViewValueListAttribute.IsActive = false;
        //            if (strArrays != null || SMNotificationMaint.GetScreenFields(str, out strArrays, out strArrays1)) {
        //                primaryViewValueListAttribute.SetList(sender, strArrays, strArrays1);
        //            } else {
        //                return;
        //            }
        //        } else {
        //            primaryViewValueListAttribute.IsActive = true;
        //        }
        //    }
        //}
    }
}