using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace Covid19.Lib
{
    public sealed class CSAttributeDetailSurveyExt : PXCacheExtension<PX.Objects.CS.CSAttributeDetail>
    {
        #region Description
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(PXDBLocalizableStringAttribute))]
        [PXDBLocalizableString(255, IsUnicode = true)]
        public string Description { get; set; }
        #endregion
    }
}
