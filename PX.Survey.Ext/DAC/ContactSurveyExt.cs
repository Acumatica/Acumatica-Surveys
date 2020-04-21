using System;
using PX.Data;
using PX.Objects.CR;

namespace PX.Survey.Ext
{
    public sealed class ContactSurveyExt : PXCacheExtension<Contact>
    {
        #region UsrMobileAppDeviceOS
        public abstract class usrMobileAppDeviceOS : PX.Data.BQL.BqlString.Field<usrMobileAppDeviceOS> { }

        [PXString()]
        [PXUIField(DisplayName = "Mobile Device OS", Enabled = false)]
        public String UsrMobileAppDeviceOS { get; set; }
        #endregion

        #region UsrUsingMobileApp
        public abstract class usrUsingMobileApp : PX.Data.BQL.BqlBool.Field<usrUsingMobileApp> { }

        [PXBool()]        
        [PXUIField(DisplayName = "Mobile App Notifications", Enabled = false)]
        public bool? UsrUsingMobileApp { get; set; }
        #endregion
    }
}