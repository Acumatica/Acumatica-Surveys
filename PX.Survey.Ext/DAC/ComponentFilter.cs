using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {

    [PXCacheName("ComponentFilter")]
    public class ComponentFilter : IBqlTable {

        #region ComponentType
        public abstract class componentType : BqlString.Field<componentType> { }
        [PXString(2, IsFixed = true)]
        [SUComponentType.DetailList]
        [PXUIField(DisplayName = "Component Type")]
        public virtual string ComponentType { get; set; }
        #endregion
    }
}
