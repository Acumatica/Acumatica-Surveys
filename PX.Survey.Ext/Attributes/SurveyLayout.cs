using PX.Data;

namespace PX.Survey.Ext {
    public static class SurveyLayout {
        public class ListAttribute : PXStringListAttribute {
            public ListAttribute() : base(
                new string[] { SinglePage, MultiPage},
                new string[] { 
                    Messages.SurveyLayout.SinglePage, Messages.SurveyLayout.MultiPage,
                }) { }
        }

        public const string SinglePage = "S";
        public const string MultiPage = "M";
    }
}
