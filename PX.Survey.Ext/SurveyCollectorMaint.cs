using CommonServiceLocator;
using PX.Api.Mobile.PushNotifications;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.EP;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace PX.Survey.Ext {

    public class SurveyCollectorMaint : PXGraph<SurveyCollectorMaint> {

        private static readonly IPushNotificationSender pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();

        public PXSelect<SurveyCollector> Collector;

        public PXSelect<Survey, Where<Survey.surveyID, Equal<Required<SurveyCollector.surveyID>>>> FindSurvey;
        public PXSelect<SurveyUser,
            Where<SurveyUser.surveyID, Equal<Required<SurveyUser.surveyID>>,
            And<SurveyUser.lineNbr, Equal<Required<SurveyUser.lineNbr>>>>> FindUser;

        public SelectFrom<SurveyCollectorData>.Where<SurveyCollectorData.collectorID.IsEqual<SurveyCollector.collectorID.FromCurrent>>.View CollectedAnswers;
        public SelectFrom<SurveyCollectorData>.Where<SurveyCollectorData.status.IsNotEqual<CollectorDataStatus.processed>>.View UnprocessedCollectedAnswers;

        public PXCancel<SurveyCollector> Cancel;
        public PXSave<SurveyCollector> Save;

        //protected virtual void _(Events.FieldSelecting<SurveyCollector, SurveyCollector.baseURL> e) {
        //    var row = e.Row;
        //    if (row == null) {
        //        return;
        //    }
        //    var generator = new SurveyGenerator();
        //    e.ReturnValue = generator.GetUrl();
        //}

        public PXAction<SurveyCollector> sendNewNotification;
        [PXUIField(DisplayName = "Send New Notification", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable SendNewNotification(PXAdapter adapter) {
            if (Collector.Current != null) {
                Save.Press();
                var graph = CreateInstance<SurveyCollectorMaint>();
                var row = PXCache<SurveyCollector>.CreateCopy(Collector.Current);
                graph.DoSendNewNotification(row);
            }
            return adapter.Get();
        }

        public void DoSendNewNotification(SurveyCollector collector) {
            Collector.Current = collector;
            Survey survey = FindSurvey.Select(collector.SurveyID);
            DoSendNotification(collector, survey, survey.NotificationID);
            collector.SentOn = PXTimeZoneInfo.Now;
            collector.Status = CollectorStatus.Sent;
            collector.Message = null;
            Collector.Update(collector);
            Actions.PressSave();
        }

        public PXAction<SurveyCollector> sendReminder;
        [PXUIField(DisplayName = "Send Reminder", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable SendReminder(PXAdapter adapter) {
            if (Collector.Current != null) {
                Save.Press();
                var graph = CreateInstance<SurveyCollectorMaint>();
                var row = PXCache<SurveyCollector>.CreateCopy(Collector.Current);
                graph.DoSendReminder(row, 90); // TODO Ask for Delay
            }
            return adapter.Get();
        }

        public void DoSendReminder(SurveyCollector collector, int? delay) {
            Collector.Current = collector;
            Survey survey = FindSurvey.Select(collector.SurveyID);
            DoSendNotification(collector, survey, survey.RemindNotificationID);
            collector.SentOn = PXTimeZoneInfo.Now;
            collector.Status = CollectorStatus.Reminded;
            collector.Message = null;
            if (delay > 0) {
                collector.ExpirationDate = DateTime.UtcNow.AddMinutes(delay.Value);
            }
            Collector.Update(collector);
            Actions.PressSave();
        }

        public void DoSendNotification(SurveyCollector collector, Survey survey, int? notificationID) {
            SurveyUser surveyUser = FindUser.Select(collector.SurveyID, collector.UserLineNbr);
            if (surveyUser.UsingMobileApp == true) {
                SendPushNotification(survey, surveyUser, collector);
            } else {
                SendMailNotification(survey, surveyUser, collector, notificationID);
            }
        }

        private void SendPushNotification(Survey survey, SurveyUser surveyUser, SurveyCollector surveyCollector) {
            string sScreenID = PXSiteMap.Provider
                .FindSiteMapNodeByGraphType(typeof(SurveyCollectorMaint).FullName).ScreenID;
            Guid noteID = surveyCollector.NoteID.GetValueOrDefault();
            if (surveyUser.UserID != null) {
                List<Guid> userIds = new List<Guid> { surveyUser.UserID.GetValueOrDefault() };
                pushNotificationSender.SendNotificationAsync(
                    userIds: userIds,
                    title: Messages.PushNotificationTitleSurvey,
                    text: $"{Messages.PushNotificationMessageBodySurvey} # {survey.Title}.",
                    link: (sScreenID, noteID),
                    cancellation: CancellationToken.None);
            }
        }

        private void SendMailNotification(Survey survey, SurveyUser surveyUser, SurveyCollector collector, int? notificationID) {
            Notification notification = PXSelect<Notification, Where<Notification.notificationID, Equal<Required<Notification.notificationID>>>>.Select(this, notificationID);
            /*
            notification.RefNoteID = collector.NoteID.ToString();
            */
            //var sent = false;
            var emailGenerator = TemplateNotificationGenerator.Create(collector, notification);
            emailGenerator.LinkToEntity = true;
            emailGenerator.To = surveyUser.Email;
            emailGenerator.ContactID = surveyUser.ContactID;
            var generator = new SurveyGenerator();
            var url = generator.GetUrl(survey, collector.Token, null);
            emailGenerator.Body = emailGenerator.Body.Replace("((Collector.URL))", url);
            //sender.MailAccountId = notification.NFrom ?? MailAccountManager.DefaultMailAccountID;
            emailGenerator.RefNoteID = collector.NoteID;
            //sender.Subject =
            //bool asAttachment = false;
            //if (asAttachment) {
            //if (!string.IsNullOrEmpty(message)) {
            //    sender.AddAttachment("HeaderContent.json", Encoding.UTF8.GetBytes(message));
            //}
            //} else {
            //sender.Body = message;
            //sender.BodyFormat = PX.Objects.CS.NotificationFormat.Html;
            //}
            //foreach (Guid? attachment in (IEnumerable<Guid?>)attachments) {
            //    if (attachment.HasValue)
            //        notificationGenerator.AddAttachmentLink(attachment.Value);
            //}
            var emails = emailGenerator.Send();
        }
        //}
    }
}