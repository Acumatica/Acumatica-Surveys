using PX.Data;
using PX.Data.BQL;

namespace PX.Survey.Ext {

    [PXCacheName("TemplateFilter")]
    public class TemplateFilter : IBqlTable {

        #region TemplateType
        public abstract class templateType : BqlString.Field<templateType> { }
        [PXString(2, IsFixed = true)]
        [SUTemplateType.DetailList]
        [PXUIField(DisplayName = "Template Type")]
        public virtual string TemplateType { get; set; }
        #endregion
    }
}
