using PX.Data;
using PX.Objects.CS;

namespace Covid19.Lib
{
    public sealed class CRSetupSurveyExt : PXCacheExtension<PX.Objects.CR.CRSetup>
    {
        #region UsrSurveyNumberingID
        public abstract class usrSurveyNumberingID : PX.Data.BQL.BqlString.Field<usrSurveyNumberingID> { }

        [PXDBString(15, IsUnicode = true)]        
        [PXUIField(DisplayName = "Survey Numbering ID")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public string UsrSurveyNumberingID { get; set; }
        #endregion
    }
}