using PX.Data;
using PX.Objects.CS;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PX.Survey.Ext {
    public class SurveySetupMaint : PXGraph<SurveySetupMaint> {

        public PXSave<SurveySetup> Save;
        public PXCancel<SurveySetup> Cancel;
        public PXSelect<SurveySetup> surveySetup;
        public SurveySetupEntitySelect DefaultSurveys;

        public PXSelect<CacheEntityItem, Where<CacheEntityItem.path, Equal<CacheEntityItem.path>>, OrderBy<Asc<CacheEntityItem.number>>> EntityItems;

        //public IEnumerable entityItems(string parent) {
        //    var notifGraph = PXGraph.CreateInstance<SMNotificationMaint>();
        //    var screenID = DefaultSurveys.Current?.ScreenID;
        //    if (string.IsNullOrEmpty(screenID)) {
        //        return null;
        //    }
        //    var notif = new Notification {
        //        ScreenID = screenID
        //    };
        //    notifGraph.Notifications.Cache.Insert(notif);
        //    var mi = notifGraph.GetType().GetMethod("GetEntityItemsImpl", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (mi != null) {
        //        //return this.GetEntityItemsImpl(parent, false, (string primaryView, CacheEntityItem entry) => entry);
        //        Func<string, CacheEntityItem, CacheEntityItem> transFormAndFilterEntries = (pv, entry) => entry;
        //        var args = new object[] { parent, false, transFormAndFilterEntries };
        //        return (IEnumerable) mi.Invoke(notifGraph, args);
        //    }
        //    return null;
        //}

        protected virtual void _(Events.FieldSelecting<SurveySetupEntity, SurveySetupEntity.entityType> e) {
            if (e.Row != null) {
                e.ReturnState = CreateFieldStateForEntity(e.ReturnValue, e.Row.EntityType, e.Row.GraphType);
            }
        }

        private PXFieldState CreateFieldStateForEntity(object returnState, string entityType, string graphType) {
            List<string> strs = new List<string>();
            List<string> strs1 = new List<string>();
            Type type = GraphHelper.GetType(entityType);
            if (type != null) {
                Type primaryGraphType = EntityHelper.GetPrimaryGraphType(this, type);
                if (!string.IsNullOrEmpty(graphType)) {
                    primaryGraphType = GraphHelper.GetType(graphType);
                }
                if (primaryGraphType != null) {
                    foreach (CacheEntityItem cacheEntityItem in EMailSourceHelper.TemplateEntity(this, null, type.FullName, primaryGraphType.FullName)) {
                        if (cacheEntityItem.SubKey == typeof(CSAnswers).FullName) {
                            continue;
                        }
                        strs.Add(cacheEntityItem.SubKey);
                        strs1.Add(cacheEntityItem.Name);
                    }
                } else {
                    PXCacheNameAttribute[] customAttributes = (PXCacheNameAttribute[])type.GetCustomAttributes(typeof(PXCacheNameAttribute), true);
                    if (type.IsSubclassOf(typeof(CSAnswers))) {
                        strs.Add(type.FullName);
                        strs1.Add((customAttributes.Length != 0 ? customAttributes[0].Name : type.Name));
                    }
                }
            }
            bool? nullable = null;
            return PXStringState.CreateInstance(returnState, new int?(60), nullable, "Entity", new bool?(false), new int?(1), null, strs.ToArray(), strs1.ToArray(), new bool?(true), null, null);
        }

        protected virtual void _(Events.FieldUpdated<SurveySetupEntity, SurveySetupEntity.graphType> e) {
            var row = e.Row;
            if (row != null && row.GraphType != null) {
                PXGraph pXGraph = CreateInstance(GraphHelper.GetType(row.GraphType));
                Type itemType = pXGraph.Views[pXGraph.PrimaryView].Cache.GetItemType();
                row.EntityType = itemType.FullName;
            }
        }

        protected IEnumerable entityItems(string parent) {
            if (DefaultSurveys.Current != null) {
                Type graphType;
                Type entityType = GraphHelper.GetType(DefaultSurveys.Current.EntityType);
                if (DefaultSurveys.Current.GraphType != null) {
                    graphType = GraphHelper.GetType(DefaultSurveys.Current.GraphType);
                } else if (!(entityType == null) || parent == null) {
                    graphType = EntityHelper.GetPrimaryGraphType(this, entityType);
                } else {
                    yield break;
                }
                string entityTypeName = entityType.FullName;
                string graphTypeName = graphType?.FullName;
                foreach (CacheEntityItem cacheEntityItem in EMailSourceHelper.TemplateEntity(this, parent, entityTypeName, graphTypeName)) {
                    yield return cacheEntityItem;
                }
            }
            yield break;
        }

        //protected virtual void _(Events.FieldSelecting<CacheEntityItem.e EPRuleCondition_Entity_(PXCache sender, PXFieldSelectingEventArgs e) {
        //    if (this.AssigmentMap.Current != null) {
        //        e.ReturnState = this.CreateFieldStateForEntity(e.ReturnValue, this.AssigmentMap.Current.EntityType, this.AssigmentMap.Current.GraphType);
        //    }
        //}

        //protected virtual void EPRuleCondition_Entity_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e) {
        //    EPRuleCondition row = e.Row as EPRuleCondition;
        //    this.Rules.Cache.SetValue<EPRuleCondition.fieldName>(row, null);
        //}

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

        //protected virtual void _(Events.FieldUpdated<SurveySetupEntity, SurveySetupEntity.screenID> e) {
        //    e.Cache.SetDefaultExt<SurveySetupEntity.entityType>(e.Row);
        //}

        protected virtual void _(Events.FieldDefaulting<SurveySetupEntity, SurveySetupEntity.entityType> e) {
            var row = e.Row;
            //if (row == null || row.ScreenID == null) {
            //    return;
            //}
            //var smn = PXSiteMap.Provider.FindSiteMapNodeByScreenID(row.ScreenID);
            //if (smn != null) {
            //    var graphType = smn.GraphType;
            //    var primaryCacheInfo = GraphHelper.GetPrimaryCache(graphType);
            //    e.NewValue = primaryCacheInfo?.CacheType.FullName;
            //    e.Cancel = e.NewValue != null;
            //}
        }
    }
}