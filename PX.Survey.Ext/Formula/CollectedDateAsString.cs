using PX.Data;
using System;
using System.Collections.Generic;

namespace PX.Survey.Ext {
    public class CollectedDateAsString<CollectedDate> : BqlFormulaEvaluator<CollectedDate>
                where CollectedDate : IBqlOperand {
        public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars) {
            DateTime? collectedDate = (DateTime?)pars[typeof(CollectedDate)];
            if (!collectedDate.HasValue) return null;
            return collectedDate.Value.Date.ToString("d");
        }
    }
}
