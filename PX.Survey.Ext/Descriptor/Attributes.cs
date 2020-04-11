using System;
using PX.Data;
using PX.Api.Mobile.PushNotifications.DAC;

namespace PX.Survey.Ext
{
    public class MobileAppEnabledAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
    {
        protected string _UserID = null;

        public MobileAppEnabledAttribute(Type userID)
        {
            _UserID = userID.Name;
        }

        public virtual void FieldSelecting(PXCache cache, PXFieldSelectingEventArgs e)
        {
            Guid? userID = (Guid?)cache.GetValue(e.Row, _UserID);
            bool isMobileAppEnabled = false;

            if (userID.HasValue)
            {
                MobileDevice device = PXSelect<MobileDevice, Where<MobileDevice.userID, Equal<Required<MobileDevice.userID>>,
                                                                And<MobileDevice.enabled, Equal<True>,
                                                                And<MobileDevice.expiredToken, NotEqual<True>>>>>
                                                                .SelectWindowed(cache.Graph, 0, 1, userID);
                isMobileAppEnabled = (device != null);
            }
            e.ReturnValue = isMobileAppEnabled;
        }
    }
}