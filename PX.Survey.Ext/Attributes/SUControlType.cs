using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {
    public static class SUControlType {
        public class ListAttribute : PXIntListAttribute {
            public ListAttribute() : base(
                new int[] { Text, Combo, Multi, Checkbox, Datetime, Selector },
                new[] { "Text", "Combo", "Multi Select Combo", "Checkbox", "Datetime", "Selector" }
                ) { }
        }

        public const int Text = 1;
        public const int Combo = 2;
        public const int Multi = 6;
        public const int Checkbox = 4;
        public const int Datetime = 5;
        public const int Selector = 7;

        public class text : BqlInt.Constant<text> { public text() : base(Text) { } }
        public class combo : BqlInt.Constant<combo> { public combo() : base(Combo) { } }
        public class multi : BqlInt.Constant<multi> { public multi() : base(Multi) { } }
        public class checkbox : BqlInt.Constant<checkbox> { public checkbox() : base(Checkbox) { } }
        public class datetime : BqlInt.Constant<datetime> { public datetime() : base(Datetime) { } }
        public class selector : BqlInt.Constant<selector> { public selector() : base(Selector) { } }
    }
}
