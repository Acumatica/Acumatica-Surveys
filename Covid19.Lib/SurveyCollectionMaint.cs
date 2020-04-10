using System;
using PX.Data;
using PX.Api.Mobile.PushNotifications;
using PX.Api.Mobile.PushNotifications.DAC;
using Microsoft.Practices.ServiceLocation;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Covid19.Lib
{
    //DHIREN'S NOTE - REQUIRES CLEAN UP
    public class SurveyCollectionMaint : PXGraph<SurveyCollectionMaint, SurveyCollector>
    {
        public PXSelect<SurveyCollector, Where<SurveyCollector.collectorStatus, Equal<SurveyResponseStatus.CollectorNewStatus>>> Collections;

        public void AssignSetStatus(SurveyCollector c, bool isMassProcess = false)
        {
            Collections.Current = c;
            c.CollectorStatus = "S";
            Collections.Update(c);
            Persist();

            var pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();

            List<Guid> userIds = new List<Guid>();
            userIds.Add(c.Userid.Value);

            MobileDevice device = PXSelect<MobileDevice, 
                                    Where<MobileDevice.userID, Equal<Required<MobileDevice.userID>>>>.Select(this, c.Userid.Value);
            if (device == null)
            {
                throw new PXException(Messages.NoDeviceError);
            }

            string sScreenID = "CV301010"; //tied to SurveyQuizEmployeeMaint
            Guid noteID = c.NoteID.Value;

            PXLongOperation.StartOperation(this, () =>
            {
                try
                {
                    pushNotificationSender.SendNotificationAsync(
                                        userIds: userIds,
                                        title: "",
                                        text: $"{ Messages.PushNotificationMessageBodySurvey } { c.CollectorName }.",
                                        link: (sScreenID, noteID),
                                        cancellation: CancellationToken.None);
                }
                catch (AggregateException ex)
                {
                    var message = string.Join(";", ex.InnerExceptions.Select(e => e.Message));
                    throw new InvalidOperationException(message);
                }
            });

            if (isMassProcess)
            {
                PXProcessing.SetInfo(String.Format(
                "Survey  {0} has been successfully updated.", c.CollectorID));
            }
        }
    }
}