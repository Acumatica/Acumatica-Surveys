﻿using PX.Data;

namespace PX.Survey.Ext {
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public sealed class CSAttributeDetailSurveyExt : PXCacheExtension<PX.Objects.CS.CSAttributeDetail> {
        #region Description
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(PXDBLocalizableStringAttribute))]
        [PXDBLocalizableString(255, IsUnicode = true)]
        public string Description { get; set; }
        #endregion
    }
}
