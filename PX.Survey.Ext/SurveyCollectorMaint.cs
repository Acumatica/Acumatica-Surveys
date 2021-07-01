using CommonServiceLocator;
using PX.Api.Mobile.PushNotifications;
using PX.Common;
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

        //protected void _(Events.RowSelected<SurveyCollector> e) {
        //    var row = e.Row;
        //    if (row == null) {
        //        return;
        //    }
        //}

        public PXAction<SurveyCollector> sendNewNotification;
        [PXUIField(DisplayName = "Send New Notification", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
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
            Survey survey = CurrentSurvey.Select();
            DoSendNotification(collector, survey, survey.NotificationID);
            collector.SentOn = PXTimeZoneInfo.Now;
            collector.Status = CollectorStatus.Sent;
            collector.Message = null;
            Collector.Update(collector);
            Actions.PressSave();
        }

        public PXAction<SurveyCollector> sendReminder;
        [PXUIField(DisplayName = "Send Reminder", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
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
            Survey survey = CurrentSurvey.Select();
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
            SurveyUser surveyUser = CurrentUser.Select();
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
            //var sent = false;
            var email = TemplateNotificationGenerator.Create(collector, notification);
            email.LinkToEntity = true;
            email.To = surveyUser.Email;
            email.ContactID = surveyUser.ContactID;
            var generator = new SurveyGenerator();
            var url = generator.GetUrl(survey, collector.Token, null);
            email.Body = email.Body.Replace("((Collector.URL))", url);
            //sender.MailAccountId = notification.NFrom ?? MailAccountManager.DefaultMailAccountID;
            //sender.RefNoteID = collector.NoteID;
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
            var sent = email.Send();
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