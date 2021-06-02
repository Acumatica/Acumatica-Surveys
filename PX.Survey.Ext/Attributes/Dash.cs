using PX.Data.BQL;

namespace PX.Survey.Ext {

    public class Dash : BqlType<IBqlString, string>.Constant<Dash> {
        public Dash() : base("-") {
        }
    }
}
