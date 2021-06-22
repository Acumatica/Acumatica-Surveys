using PX.Data;
using PX.Data.Maintenance.GI;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;

namespace PX.Survey.Ext {
    public class SurveySetupMaint : PXGraph<SurveySetupMaint> {

        public PXSave<SurveySetup> Save;
        public PXCancel<SurveySetup> Cancel;
        public PXSelect<SurveySetup> surveySetup;
        public PXSelect<SurveySetupEntity> DefaultSurveys;

        public PXSelect<CacheEntityItem, Where<CacheEntityItem.path, Equal<CacheEntityItem.path>>, OrderBy<Asc<CacheEntityItem.number>>> EntityItems;

        public IEnumerable entityItems(string parent) {
            var notifGraph = PXGraph.CreateInstance<SMNotificationMaint>();
            var screenID = DefaultSurveys.Current?.ScreenID;
            if (string.IsNullOrEmpty(screenID)) {
                return null;
            }
            var notif = new Notification {
                ScreenID = screenID
            };
            notifGraph.Notifications.Cache.Insert(notif);
            var mi = notifGraph.GetType().GetMethod("GetEntityItemsImpl", BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null) {
                //return this.GetEntityItemsImpl(parent, false, (string primaryView, CacheEntityItem entry) => entry);
                Func<string, CacheEntityItem, CacheEntityItem> transFormAndFilterEntries = (pv, entry) => entry;
                var args = new object[] { parent, false, transFormAndFilterEntries };
                return (IEnumerable) mi.Invoke(notifGraph, args);
            }
            return null;
        }

        //protected virtual void _(Events.RowSelected<SurveySetupEntity> e) {
        //    string screenID;
        //    var row = e.Row;
        //    string str = screenID;
        //    if (row == null || str == null) {
        //        return;
        //    }
        //    string[] strArrays = null;
        //    string[] strArrays1 = null;
        //    foreach (PXEventSubscriberAttribute attribute in sender.GetAttributes(row, "value")) {
        //        PrimaryViewValueListAttribute primaryViewValueListAttribute = attribute as PrimaryViewValueListAttribute;
        //        if (primaryViewValueListAttribute == null) {
        //            continue;
        //        }
        //        bool? fromSchema = row.FromSchema;
        //        if (!(fromSchema.GetValueOrDefault() & fromSchema.HasValue)) {
        //            primaryViewValueListAttribute.IsActive = false;
        //            if (strArrays != null || SMNotificationMaint.GetScreenFields(str, out strArrays, out strArrays1)) {
        //                primaryViewValueListAttribute.SetList(sender, strArrays, strArrays1);
        //            } else {
        //                return;
        //            }
        //        } else {
        //            primaryViewValueListAttribute.IsActive = true;
        //        }
        //    }
        //}

        protected virtual void _(Events.FieldUpdated<SurveySetupEntity, SurveySetupEntity.screenID> e) {
            e.Cache.SetDefaultExt<SurveySetupEntity.entityType>(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<SurveySetupEntity, SurveySetupEntity.entityType> e) {
            var row = e.Row;
            if (row == null || row.ScreenID == null) {
                return;
            }
            var smn = PXSiteMap.Provider.FindSiteMapNodeByScreenID(row.ScreenID);
            if (smn != null) {
                var graphType = smn.GraphType;
                var primaryCacheInfo = GraphHelper.GetPrimaryCache(graphType);
                e.NewValue = primaryCacheInfo?.CacheType.FullName;
                e.Cancel = e.NewValue != null;
            }
        }
    }
}