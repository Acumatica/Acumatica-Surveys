using PX.Data;
using System;
using System.Collections.Generic;

namespace PX.Survey.Ext {
    public class DateAsStringFormat {
        public class roundTrip : Data.BQL.BqlString.Constant<roundTrip> { public roundTrip() : base("O") { } }
        public class shortDate : Data.BQL.BqlString.Constant<shortDate> { public shortDate() : base("d") { } }
    }

    public class DateAsString<TDate, TFormat> : BqlFormulaEvaluator<TDate>
                where TDate : IBqlOperand
                where TFormat : Data.BQL.BqlType<Data.BQL.IBqlString, string>.Constant<TFormat>, new() {



        private Data.BQL.BqlType<Data.BQL.IBqlString, string>.Constant<TFormat> _dateFormat;
        private Type DateFormat => typeof(TFormat);

        public virtual string Format {
            get {
                if (_dateFormat == null) {
                    _dateFormat = (Data.BQL.BqlType<Data.BQL.IBqlString, string>.Constant<TFormat>)Activator.CreateInstance(DateFormat);
                }
                return _dateFormat.Value.ToString();
            }
        }

        public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars) {
            DateTime? collectedDate = (DateTime?)pars[typeof(TDate)];
            //if (!collectedDate.HasValue) {
            //    var parCache = cache.Graph.Caches[typeof(TDate).DeclaringType];
            //    collectedDate = (DateTime?) parCache?.GetValue(parCache.Current, typeof(TDate).Name);
            //}
            if (!collectedDate.HasValue) {
                return null;
            }
            return collectedDate.Value.ToString(Format);
        }
    }
}
