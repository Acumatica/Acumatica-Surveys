namespace PX.Survey.Ext {
    public class POOrderEntryExt : AbstractSurveyHandlerExt<Objects.PO.POOrderEntry, Objects.PO.POOrder> {
        public static bool IsActive() => true;
    }
}