using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;

namespace PX.Survey.Ext {

    [PXCacheName("RecipientFilter")]
    public class RecipientFilter : IBqlTable {

        #region ContactType
        public abstract class contactType : BqlString.Field<contactType> { }
        [PXString(2, IsFixed = true)]
        [PXUnboundDefault(ContactTypesAttribute.Employee)]
        [ContactTypes]
        [PXUIField(DisplayName = "Recipient Type")]
        public virtual string ContactType { get; set; }
        #endregion
    }
}
