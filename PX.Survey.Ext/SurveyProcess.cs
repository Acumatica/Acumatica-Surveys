using CommonServiceLocator;
using PX.Api.Mobile.PushNotifications;
using PX.Common;
using PX.Data;
using PX.Data.EP;
using PX.Objects.EP;
using PX.SM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PX.Survey.Ext {
    public class SurveyProcess : PXGraph<SurveyProcess> {

        public PXCancel<SurveyFilter> Cancel;
        public PXFilter<SurveyFilter> Filter;

        public PXFilteredProcessing<Survey, SurveyFilter,
            Where<Survey.active, Equal<True>,
            And<SurveyUser.surveyID, Equal<Current<SurveyFilter.surveyID>>>>> Surveys;

        private static IPushNotificationSender pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();

        public SurveyProcess() {
            Surveys.SetProcessCaption(Messages.Send);
            Surveys.SetProcessAllCaption(Messages.SendAll);
        }

        protected virtual void _(Events.RowSelected<SurveyFilter> e) {
            var row = e.Row;
            if (row == null) {
                return;
            }
            var action = row.SurveyAction;
            var doProcessAnswers = action == SurveyAction.ProcessAnswers;
            PXUIFieldAttribute.SetRequired<SurveyFilter.surveyID>(e.Cache, !doProcessAnswers);
            PXDefaultAttribute.SetPersistingCheck<SurveyFilter.surveyID>(e.Cache, null, !doProcessAnswers ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
            Surveys.SetProcessDelegate(list => ProcessSurvey(list, row));
        }

        public static void ProcessSurvey(List<Survey> surveys, SurveyFilter filter) {
            bool errorOccurred = false;
            SurveyMaint graph = CreateInstance<SurveyMaint>();
            var action = filter.SurveyAction;
            var doProcessAnswers = action == SurveyAction.ProcessAnswers;
            var generator = new SurveyGenerator(graph);
            if (doProcessAnswers) {
                errorOccurred = ProcessAnswers(graph);
            } else {
                foreach (var survey in surveys) {
                    graph.CurrentSurvey.Current = survey;
                    switch (filter.SurveyAction) {
                        //note: the or clauses below are intended to preserve a previous error indicator and not let 
                        //      successive iterations override a previous error detection.
                        case SurveyAction.RenderOnly:
                            errorOccurred = Render(graph, generator, survey, filter) || errorOccurred;
                            break;
                        case SurveyAction.NewOnly:
                            errorOccurred = SendNew(graph, survey, filter) || errorOccurred;
                            break;
                        case SurveyAction.RemindOnly:
                            errorOccurred = SendReminders(graph, survey, filter) || errorOccurred;
                            break;
                        case SurveyAction.ExpireOnly:
                            errorOccurred = SetExpiredSurveys(graph, survey, filter) || errorOccurred;
                            break;
                        case SurveyAction.DefaultAction:
                            errorOccurred = DefaultRoutine(graph, generator, survey, filter) || errorOccurred;
                            break;
                        default:
                            throw new PXException(Messages.SurveyActionNotRecognised);
                    }
                }
            }
            if (errorOccurred) {
                throw new PXException(Messages.SurveyError);
            }
        }

        private static bool Render(SurveyMaint graph, SurveyGenerator generator, Survey survey, SurveyFilter filter) {
            var collectors = graph.Collectors.Select().FirstTableItems;
            var users = graph.Users.Select().FirstTableItems;
            foreach (var user in users) {
                var collector = collectors.FirstOrDefault(coll => coll.UserID == user.UserID);
                if (collector != null && collector.Status != CollectorStatus.New && collector.Status != CollectorStatus.Error) {
                    continue;
                }
                if (collector == null) {
                    collector = new SurveyCollector {
                        CollectorName =
                            $"{survey.SurveyName} {PXTimeZoneInfo.Now:yyyy-MM-dd hh:mm:ss}",
                        SurveyID = survey.SurveyID,
                        UserID = user.UserID,
                        CollectedDate = null,
                        ExpirationDate = CalculateExpirationDate(filter.DurationTimeSpan),
                    };
                    var inserted = graph.Collectors.Insert(collector);
                }
                try {
                    var surveySays = generator.GenerateSurvey(survey, user);
                    collector.Rendered = surveySays;
                    collector.Status = CollectorStatus.Rendered;
                    collector.Message = null;
                } catch (Exception ex) {
                    collector.Status = CollectorStatus.Error;
                    collector.Message = ex.Message;
                }
                var updated = graph.Collectors.Update(collector);
            }
            return false;
        }


        /// <summary>
        /// This is intended to be the primary action that this process is run that will flow through a logical sequence that will hit any of the three
        /// process flows.
        /// 1) the set expiration logic will be run every time
        /// 2) a determination is made whether the user has any active open surveys after the expiration routine has run.
        /// 3) if any active surveys are found then a Reminder is sent. A new reminder does not get sent
        /// 4) if no active surveys are found a new one is sent. This is refereed to as a Re-Send
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="surveyUserList"></param>
        /// <param name="filter"></param>
        /// <returns>Whether of not any of the processes within have an error</returns>
        private static bool DefaultRoutine(SurveyMaint graph, SurveyGenerator generator, Survey survey, SurveyFilter filter) {
            var errorOccurred = Render(graph, generator, survey, filter);
            errorOccurred = SetExpiredSurveys(graph, survey, filter) || errorOccurred;
            errorOccurred = SendReminders(graph, survey, filter) || errorOccurred;
            errorOccurred = SendNew(graph, survey, filter) || errorOccurred;
            return errorOccurred;
        }

        private static bool ProcessAnswers(SurveyMaint graph) {
            bool errorOccurred = false;
            try {
                graph.Clear();
                // TODO
                //var allUnanswered = graph.UnprocessedCollectedAnswers.Select();
                //foreach (SurveyCollectorData unanswered in allUnanswered) {
                //    PXProcessing<SurveyCollectorData>.SetCurrentItem(unanswered);
                //    DoProcessAnswers(graph, unanswered);
                //    graph.UnprocessedCollectedAnswers.Update(unanswered);
                //}
                graph.Persist();
            } catch (AggregateException ex) {
                var message = string.Join(";", ex.InnerExceptions.Select(e => e.Message));
                PXProcessing<SurveyCollectorData>.SetError(message);
                errorOccurred = true;
            } catch (Exception e) {
                errorOccurred = true;
                PXProcessing<SurveyCollectorData>.SetError(e);
            }
            return errorOccurred;
        }

        private static void DoProcessAnswers(SurveyCollectorMaint graph, SurveyCollectorData unanswered) {
            var collectorID = unanswered.CollectorID;
            var xx = graph.Collector.Search<SurveyCollector.collectorID>(collectorID);
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method will create a new collector record and invoke a notification on it.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="surveyUserList"></param>
        /// <param name="filter"></param>
        /// <remarks>
        /// </remarks>
        /// <returns>
        ///     Whether or not an error has occured within the process which is used by the main calling process to throw a final exception at the end of the process
        /// </returns>
        private static bool SendNew(SurveyMaint graph, Survey survey, SurveyFilter filter) {
            bool errorOccurred = false;
            var collectors = graph.Collectors.Select();
            foreach (var res in collectors) {
                var collector = PXResult.Unwrap<SurveyCollector>(res);
                if (collector.Status != CollectorStatus.Rendered) {
                    continue;
                }
                var surveyUser = PXResult.Unwrap<SurveyUser>(res);
                try {
                    SendNotification(surveyUser, collector);
                } catch (Exception ex) {
                    collector.Status = CollectorStatus.Error;
                    collector.Message = ex.Message;
                    errorOccurred = true;
                }
                var updated = graph.Collectors.Update(collector);
            }
            return errorOccurred;
        }

        /// <summary>
        /// This method allows the duration value to be expressed using a PXTimeSpanLong.
        /// </summary>
        /// <returns>
        /// The Calculated Time Span
        /// </returns>
        /// <remarks>
        ///     If the durationTimeSpan is either null or zero we explicitly need to set a
        ///     null value as the expiration date.
        /// </remarks>
        private static DateTime? CalculateExpirationDate(int? durationTimeSpan) {
            if (durationTimeSpan.GetValueOrDefault() == 0) return null;
            return DateTime.UtcNow.AddMinutes(durationTimeSpan.GetValueOrDefault());
        }


        /// <summary>
        /// This method sends a reminder notification for any active Collector for a given user.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="surveyUserList"></param>
        /// <param name="filter"></param>
        /// <remarks>
        /// Note:   Regarding the term to "Re-Send" and "Reminder"
        ///         By this term resend we are creating a new Collector record sometime after the first has been
        ///         sent. the term "Re-Send" is not the same as a reminder where a reminder is a second notification for the same collector
        ///         record.
        /// note:   If a collector has a null expiration date and the duration was set for this round, the expiration will be set. if,
        ///         however, the expiration was already previously set, the expiration will never be overridden. 
        /// </remarks>
        private static bool SendReminders(SurveyMaint graph, Survey survey, SurveyFilter filter) {
            bool errorOccurred = false; //assume a successful result until we detect the first specific failure in the loop below; 
            var activeCollectors = GetActiveCollectors(surveyUser, graph, surveyCurrent);
            var collServCache = graph.Caches[typeof(SurveyCollector)];
            foreach (var surveyCollector in activeCollectors) {
                try {
                    SendNotification(surveyUser, surveyCollector);
                    if (surveyCollector.ExpirationDate == null && filter.DurationTimeSpan > 0) {
                        surveyCollector.ExpirationDate = DateTime.UtcNow.AddMinutes(filter.DurationTimeSpan.GetValueOrDefault());
                        collServCache.Update(surveyCollector);
                        graph.Persist();
                    }
                } catch (Exception e) {
                    errorOccurred = true;
                    PXTrace.WriteError(e);
                    PXTrace.WriteInformation(Messages.AnErrorOccuredTryingToResendANotificationForUserID_0, surveyUser.UserID);
                }
            }
            if (!errorOccurred) {
                PXProcessing<SurveyUser>.SetInfo(surveyUserList.IndexOf(surveyUser), Messages.SurveyReminderSent);
            } else {
                PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), Messages.SurveyReminderFailed);
            }
            return errorOccurred;
        }

        private static void SendNotification(SurveyUser surveyUser, SurveyCollector collector) {
            if (surveyUser.UsingMobileApp == true) {
                SendPushNotification(surveyUser, collector);
                collector.Message = "Don't know how to send to mobileApp";
            } else {
                SendMailNotification(surveyUser, collector);
                collector.Status = CollectorStatus.Sent;
                collector.Message = null;
            }
        }

        private static void SendPushNotification(SurveyUser surveyUser, SurveyCollector surveyCollector) {
            string sScreenID = PXSiteMap.Provider
                .FindSiteMapNodeByGraphType(typeof(SurveyCollectorMaint).FullName).ScreenID;
            Guid noteID = surveyCollector.NoteID.GetValueOrDefault();
            if (surveyUser.UserID != null) {
                PXTrace.WriteInformation("UserID " + surveyUser.UserID.Value);
                PXTrace.WriteInformation("noteID " + noteID.ToString());
                PXTrace.WriteInformation("ScreenID " + sScreenID);
                List<Guid> userIds = new List<Guid> { surveyUser.UserID.GetValueOrDefault() };
                pushNotificationSender.SendNotificationAsync(
                    userIds: userIds,
                    title: Messages.PushNotificationTitleSurvey,
                    text: $"{Messages.PushNotificationMessageBodySurvey} # {surveyCollector.CollectorName}.",
                    link: (sScreenID, noteID),
                    cancellation: CancellationToken.None);
            }
        }

        private static void SendMailNotification(SurveyUser surveyUser, SurveyCollector collector) {
            //Notification notification = PXSelect<Notification, Where<Notification.notificationID, Equal<Required<Notification.notificationID>>>>.Select(context.MessageGraph, notificationID);
            //var sent = false;
            //var sError = "Failed to send E-mail.";
            //try {
            //    var message = collector.Rendered;
            //    var sender = TemplateNotificationGenerator.Create(message, notification);
            //    sender.LinkToEntity = true;
            //    sender.MailAccountId = notification.NFrom ?? MailAccountManager.DefaultMailAccountID;
            //    sender.RefNoteID = collector.NoteID;
            //    bool asAttachment = false;
            //    if (asAttachment) {
            //        if (!string.IsNullOrEmpty(message)) {
            //            sender.AddAttachment("HeaderContent.json", Encoding.UTF8.GetBytes(message));
            //        }
            //    } else {
            //        sender.Body = message;
            //        //sender.BodyFormat = PX.Objects.CS.NotificationFormat.Html;
            //    }
            //    //foreach (Guid? attachment in (IEnumerable<Guid?>)attachments) {
            //    //    if (attachment.HasValue)
            //    //        notificationGenerator.AddAttachmentLink(attachment.Value);
            //    //}
            //    sent |= sender.Send().Any();
            //} catch (Exception ex) {
            //    sent = false;
            //    sError = ex.Message;
            //}
        }


        /// <summary>
        /// This method will search for active collectors then set the status to expired for any record that has
        /// passed the expiration date.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="surveyUserList"></param>
        private static bool SetExpiredSurveys(SurveyMaint graph, Survey survey, SurveyFilter filter) {
            bool errorOccurred = false;
            bool isPastExpiration(SurveyCollector collector) {
                //We consider collectors with a null ExpirationDate as a record that never expires
                //This can be explicitly controlled by setting the duration to 0 which will in turn 
                //set a null value into the table.
                if (!collector.ExpirationDate.HasValue) return false;
                return collector.ExpirationDate < DateTime.UtcNow;
            }
            try {
                List<SurveyCollector> usersActiveCollectors = GetActiveCollectors(surveyUser, graph, surveyCurrent);
                foreach (var surveyCollector in usersActiveCollectors.Where(isPastExpiration)) {
                    surveyCollector.Status = CollectorStatus.Expired;
                    graph.Caches["SurveyCollector"].Update(surveyCollector);
                }
                graph.Persist();
            } catch (Exception e) {
                errorOccurred = true;
                PXTrace.WriteError(e);
                PXTrace.WriteInformation(Messages.SettingTheExpirationForUserID_0_Failed, surveyUser.UserID);
            }
            if (!errorOccurred) {
                PXProcessing<SurveyUser>.SetInfo(surveyUserList.IndexOf(surveyUser), Messages.SetExpirationSuccess);
            } else {
                PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), Messages.SetExpirationFailed);
            }
            return errorOccurred;
        }

        /// <summary>
        /// This retrieves a list of active Survey Collectors for a given user and survey 
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <returns>
        ///     This is intended to be used by both the expiration mechanism as well as the
        ///     the mechanism to resend the notification.
        /// </returns>
        private static List<SurveyCollector> GetActiveCollectors(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent) {
            PXResultset<SurveyCollector> activeCollectorsResultSet =
                PXSelect<SurveyCollector,
                        Where<SurveyCollector.userID, Equal<Required<SurveyCollector.userID>>,
                                And<SurveyCollector.surveyID, Equal<Required<SurveyCollector.surveyID>>
                                , And<SurveyCollector.collectorStatus, NotEqual<Required<SurveyCollector.collectorStatus>>
                                , And<SurveyCollector.collectorStatus, NotEqual<Required<SurveyCollector.collectorStatus>>>>>>>
                    .Select(
                        graph,
                        surveyUser.UserID,
                        surveyCurrent.SurveyID,
                        CollectorStatus.Responded,
                        CollectorStatus.Expired);
            List<SurveyCollector> activeCollectors = new List<SurveyCollector>();
            foreach (var rCollector in activeCollectorsResultSet) {
                var collector = (SurveyCollector)rCollector;
                if (collector.Status == CollectorStatus.New ||
                    collector.Status == CollectorStatus.Sent) {
                    activeCollectors.Add(collector);
                }
            }
            return activeCollectors;
        }
    }

    #region SurveyFilter

    [Serializable]
    [PXCacheName(Messages.CacheNames.SurveyFilter)]
    public class SurveyFilter : IBqlTable {

        #region SurveyAction
        public abstract class surveyAction : IBqlField { }
        [PXString(1, IsUnicode = false, IsFixed = true)]
        [PXDefault(Ext.SurveyAction.DefaultAction, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Action")]
        [SurveyAction.List]
        public virtual string SurveyAction { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : IBqlField { }
        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Search<Survey.surveyID, Where<Survey.active, Equal<True>>>),
                    typeof(Survey.surveyCD),
                    typeof(Survey.surveyName),
                    SubstituteKey = typeof(Survey.surveyCD),
                    DescriptionField = typeof(Survey.surveyName))]
        public virtual int? SurveyID { get; set; }
        #endregion

        #region DurationTimeSpan
        public abstract class durationTimeSpan : Data.BQL.BqlInt.Field<durationTimeSpan> { }
        [PXDBTimeSpanLongExt(Format = TimeSpanFormatType.DaysHoursMinites)]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Expire After")]
        public virtual int? DurationTimeSpan { get; set; }
        #endregion

    }
    #endregion
}
