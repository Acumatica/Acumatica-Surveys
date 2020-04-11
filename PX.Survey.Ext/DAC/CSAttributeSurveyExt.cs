using PX.Data;

namespace PX.Survey.Ext
{
    public sealed class CSAttributeSurveyExt : PXCacheExtension<PX.Objects.CS.CSAttribute>
    {
        #region Description  
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(PXDBLocalizableStringAttribute))]
        [PXDBLocalizableString(255, IsUnicode = true)]
        public string Description { get; set; }
        #endregion
    }

    public sealed class CSAttributeGroupSurveyExt : PXCacheExtension<PX.Objects.CS.CSAttributeGroup>
    {
        #region Description  
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(PXStringAttribute))]
        [PXString(255, IsUnicode = true)]
        public string Description { get; set; }
        #endregion
    }
}