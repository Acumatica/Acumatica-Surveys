using PX.Data;

namespace PX.Survey.Ext
{
    public sealed class CSAttributeDetailSurveyExt : PXCacheExtension<PX.Objects.CS.CSAttributeDetail>
    {
        public static bool IsActive()
        {
            return true;
        }

        #region Description
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(PXDBLocalizableStringAttribute))]
        [PXDBLocalizableString(255, IsUnicode = true)]
        public string Description { get; set; }
        #endregion
    }
}
