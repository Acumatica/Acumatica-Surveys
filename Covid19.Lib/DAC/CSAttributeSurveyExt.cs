using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace Covid19.Lib.DAC
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

    public sealed class CSAttributeGroup : PXCacheExtension<PX.Objects.CS.CSAttributeGroup>
    {
        #region Description  
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(PXStringAttribute))]
        [PXString(255, IsUnicode = true)]
        public string Description { get; set; }
        #endregion
    }
}