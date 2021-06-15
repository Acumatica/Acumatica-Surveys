using PX.Data;
using System.Text;

namespace PX.Survey.Ext {
    public class SUDBTypeNameAttribute : PXDBStringAttribute {

        public SUDBTypeNameAttribute() : this(255) { }

        public SUDBTypeNameAttribute(int length) : base(length) {
            IsUnicode = true;
        }

        public override void CacheAttached(PXCache sender) {
            var wasNull = _InputMask == null;
            base.CacheAttached(sender);
            if (wasNull) {
                // Replace it
                var stringBuilder = new StringBuilder("");
                for (int i = 0; i < this._Length; i++) {
                    stringBuilder.Append("C");
                }
                this.InputMask = stringBuilder.ToString();
                this._AutoMask = PXDBStringAttribute.MaskMode.Auto;
                return;
            }
            if (!PXDBStringAttribute._masks.TryGetValue(string.Concat(this._BqlTable.Name, "$", this._FieldName), out this._InputMask)) {
                this._AutoMask = PXDBStringAttribute.MaskMode.Foreign;
            }
        }
    }
}
