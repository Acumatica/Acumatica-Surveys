using PX.Api.Mobile.PushNotifications.DAC;
using PX.Data;
using System;
using System.Collections.Generic;

namespace PX.Survey.Ext {
    public class MobileAppEnabled<UserID> : BqlFormulaEvaluator<UserID>
        where UserID : IBqlOperand {

        public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars) {
            Guid? userID = (Guid?)pars[typeof(UserID)];
            bool isMobileAppEnabled = false;
            if (userID.HasValue) {
                MobileDevice device = PXSelectReadonly<MobileDevice, Where<MobileDevice.userID, Equal<Required<MobileDevice.userID>>,
                                                                        And<MobileDevice.enabled, Equal<True>,
                                                                        And<MobileDevice.expiredToken, NotEqual<True>>>>>
                                                                        .SelectWindowed(cache.Graph, 0, 1, userID);
                isMobileAppEnabled = (device != null);
            }
            return isMobileAppEnabled;
        }
    }
}
