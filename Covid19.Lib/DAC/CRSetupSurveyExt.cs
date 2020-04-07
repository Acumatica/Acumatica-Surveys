//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//
//{
//    class CRSetupSurveyExt
//    {
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace Covid19.Lib
{
    public class CRSetupSurveyExt : PXCacheExtension<PX.Objects.CR.CRSetup>
    {
        #region UsrSurveyNumberingID
        [PXDBString(15)]
        [PXUIField(DisplayName = "Survey Numbering ID")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string UsrSurveyNumberingID { get; set; }
        public abstract class usrSurveyNumberingID : PX.Data.BQL.BqlString.Field<usrSurveyNumberingID> { }
        #endregion
    }
}