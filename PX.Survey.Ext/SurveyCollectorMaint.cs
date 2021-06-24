using CommonServiceLocator;
using PX.Api.Mobile.PushNotifications;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Objects.EP;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PX.Survey.Ext {

    public class SurveyCollectorMaint : PXGraph<SurveyCollectorMaint> {

        private static readonly IPushNotificationSender pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();

        public PXSelect<SurveyCollector> Collector;
        public PXSelect<Survey, Where<Survey.surveyID, Equal<Current<SurveyCollector.surveyID>>>> CurrentSurvey;
        public PXSelect<SurveyUser, Where<SurveyUser.lineNbr, Equal<Current<SurveyCollector.userLineNbr>>>> CurrentUser;

        public SelectFrom<SurveyCollectorData>.Where<SurveyCollectorData.collectorID.IsEqual<SurveyCollector.collectorID.FromCurrent>>.View CollectedAnswers;
        public SelectFrom<SurveyCollectorData>.Where<SurveyCollectorData.status.IsNotEqual<CollectorDataStatus.processed>>.View UnprocessedCollectedAnswers;

        public PXCancel<SurveyCollector> Cancel;
        public PXSave<SurveyCollector> Save;

        protected void _(Events.RowSelected<SurveyCollector> e) {
            var row = e.Row;
            if (row == null) {
                return;
            }
            bool bEnabled = (row.Status == CollectorStatus.Sent || row.Status == CollectorStatus.New);
            //Submit.SetEnabled(bEnabled);
            //Answers.Cache.AllowUpdate = (bEnabled);
            //ReOpen.SetEnabled(row.Status == CollectorStatus.Responded);
        }

        public PXAction<SurveyCollector> sendNewNotification;
        [PXUIField(DisplayName = "Send New Notification", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable SendNewNotification(PXAdapter adapter) {
            if (Collector.Current != null) {
                Save.Press();
                var graph = CreateInstance<SurveyCollectorMaint>();
                var row = PXCache<SurveyCollector>.CreateCopy(Collector.Current);
                var errorOccurred = graph.DoSendNewNotification(row);
            }
            return adapter.Get();
        }

        public bool DoSendNewNotification(SurveyCollector collector) {
            var survey = CurrentSurvey.Current;
            Collector.Current = collector;
            var errorOccurred = false;
            try {
                DoSendNotification(collector, survey.NotificationID);
                collector.Status = CollectorStatus.Sent;
                collector.Message = null;
            } catch (Exception ex) {
                collector.Status = CollectorStatus.Error;
                collector.Message = ex.Message;
                errorOccurred = true;
            }
            Collector.Update(collector);
            Actions.PressSave();
            return errorOccurred;
        }

        public PXAction<SurveyCollector> sendReminder;
        [PXUIField(DisplayName = "Send Reminder", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable SendReminder(PXAdapter adapter) {
            if (Collector.Current != null) {
                Save.Press();
                var graph = CreateInstance<SurveyCollectorMaint>();
                var row = PXCache<SurveyCollector>.CreateCopy(Collector.Current);
                var errorOccurred = graph.DoSendReminder(row, 90); // TODO Ask for Delay
            }
            return adapter.Get();
        }

        public bool DoSendReminder(SurveyCollector collector, int? delay) {
            var survey = CurrentSurvey.Current;
            Collector.Current = collector;
            var errorOccurred = false;
            try {
                DoSendNotification(collector, survey.RemindNotificationID);
                collector.Status = CollectorStatus.Reminded;
                collector.Message = null;
                if (delay > 0) {
                    collector.ExpirationDate = DateTime.UtcNow.AddMinutes(delay.Value);
                }
            } catch (Exception ex) {
                collector.Status = CollectorStatus.Error;
                collector.Message = ex.Message;
                errorOccurred = true;
            }
            Collector.Update(collector);
            Actions.PressSave();
            return errorOccurred;
        }

        public void DoSendNotification(SurveyCollector collector, int? notificationID) {
            SurveyUser surveyUser = CurrentUser.Current;
            if (surveyUser.UsingMobileApp == true) {
                SendPushNotification(surveyUser, collector);
            } else {
                SendMailNotification(surveyUser, collector, notificationID);
            }
        }

        private void SendPushNotification(SurveyUser surveyUser, SurveyCollector surveyCollector) {
            string sScreenID = PXSiteMap.Provider
                .FindSiteMapNodeByGraphType(typeof(SurveyCollectorMaint).FullName).ScreenID;
            Guid noteID = surveyCollector.NoteID.GetValueOrDefault();
            if (surveyUser.UserID != null) {
                //PXTrace.WriteInformation("UserID " + surveyUser.UserID.Value);
                //PXTrace.WriteInformation("NoteID " + noteID.ToString());
                //PXTrace.WriteInformation("ScreenID " + sScreenID);
                List<Guid> userIds = new List<Guid> { surveyUser.UserID.GetValueOrDefault() };
                pushNotificationSender.SendNotificationAsync(
                    userIds: userIds,
                    title: Messages.PushNotificationTitleSurvey,
                    text: $"{Messages.PushNotificationMessageBodySurvey} # {surveyCollector.Name}.",
                    link: (sScreenID, noteID),
                    cancellation: CancellationToken.None);
            }
        }

        private void SendMailNotification(SurveyUser surveyUser, SurveyCollector collector, int? notificationID) {
            Notification notification = PXSelect<Notification, Where<Notification.notificationID, Equal<Required<Notification.notificationID>>>>.Select(this, notificationID);
            //var sent = false;
            var sender = TemplateNotificationGenerator.Create(collector, notification);
            sender.LinkToEntity = collector.NoteID != null;
            sender.MailAccountId = notification.NFrom ?? MailAccountManager.DefaultMailAccountID;
            sender.RefNoteID = collector.NoteID;
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
            var sent = sender.Send().Any();
        }

        //public PXAction<SurveyCollector> Submit;
        //[PXButton(CommitChanges = true)]
        //[PXUIField(DisplayName = Messages.Submit, MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        //public virtual IEnumerable submit(PXAdapter adapter) {
        //    Persist();
        //    var currentQuestion = Collector.Current;
        //    PXLongOperation.StartOperation(this, delegate () {
        //        SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();
        //        graph.Collector.Current = graph.Collector.Search<SurveyCollector.collectorID>(currentQuestion.CollectorID);
        //        if (graph.Answers.Select().ToList().Any(x => (x.GetItem<CSAnswers>().IsRequired.GetValueOrDefault(false) &&
        //                                                     (String.IsNullOrEmpty(x.GetItem<CSAnswers>().Value))))) {
        //            throw new PXException(Messages.AnswerReqiredQuestions);
        //        }
        //        graph.Collector.Current.Status = CollectorStatus.Responded;
        //        graph.Collector.Current.CollectedDate = PXTimeZoneInfo.Now;
        //        graph.Collector.Update(graph.Collector.Current);
        //        graph.Persist();
        //    });
        //    return adapter.Get();
        //}

        //public PXAction<SurveyCollector> ReOpen;

        //[PXButton(CommitChanges = true)]
        //[PXUIField(DisplayName = Messages.ReOpen, MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        //public virtual IEnumerable reOpen(PXAdapter adapter) {
        //    Persist();
        //    var currentQuestion = Collector.Current;
        //    PXLongOperation.StartOperation(this, delegate () {
        //        SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();
        //        graph.Collector.Current = graph.Collector.Search<SurveyCollector.collectorID>(currentQuestion.CollectorID);
        //        graph.Collector.Current.Status = CollectorStatus.Sent;
        //        //graph.Collector.Current.CollectedDate = null;
        //        graph.Collector.Update(graph.Collector.Current);
        //        graph.Persist();
        //    });
        //    return adapter.Get();
        //}
    }
}