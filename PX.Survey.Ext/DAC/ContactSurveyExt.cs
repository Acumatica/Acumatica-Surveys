using PX.Data;
using PX.Objects.CR;

namespace PX.Survey.Ext
{
    public sealed class ContactSurveyExt : PXCacheExtension<Contact>
    {
        #region UsrUsingMobileApp
        public abstract class usrUsingMobileApp : PX.Data.BQL.BqlBool.Field<usrUsingMobileApp> { }

        [PXBool()]
        [PXDependsOnFields(typeof(SurveyUser.contactID), typeof(SurveyUser.userID))]
        [PXUIField(DisplayName = "Mobile App Notifications", Enabled = false)]
        public bool? UsrUsingMobileApp { get; set; }
        #endregion
    }
}