using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN.Matrix.Attributes;
using PX.TM;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXPrimaryGraph(typeof(SurveyMaint))]
    [PXCacheName(Messages.CacheNames.Survey)]
    public class Survey : IBqlTable, INotable {

        #region Keys
        public class PK : PrimaryKeyOf<Survey>.By<surveyID> {
            public static Survey Find(PXGraph graph, string surveyID) => FindBy(graph, surveyID);
            public static Survey FindDirty(PXGraph graph, string surveyID)
                => PXSelect<Survey, Where<surveyID, Equal<Required<surveyID>>>>.SelectWindowed(graph, 0, 1, surveyID);
        }
        public static class FK {
            public class SUSurveyTemplate : SurveyComponent.PK.ForeignKeyOf<Survey>.By<templateID> { }
        }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlString.Field<surveyID> { }
        [SurveyID(IsKey = true, Required = true, Visibility = PXUIVisibility.SelectorVisible)]
        [PXReferentialIntegrityCheck]
        [PXDefault]
        [PXSelector(typeof(Search<surveyID>), DescriptionField = typeof(title))]
        [AutoNumber(typeof(SurveySetup.surveyNumberingID), typeof(AccessInfo.businessDate))]
        public virtual string SurveyID { get; set; }
        #endregion

        #region Title
        public abstract class title : BqlString.Field<title> { }
        [PXDefault]
        [DBMatrixLocalizableDescription(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Title", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]
        public virtual string Title { get; set; }
        #endregion

        #region FormName
        public abstract class formName : BqlString.Field<formName> { }
        [PXDefault("SurveyForm")]
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Form Name")]
        public virtual string FormName { get; set; }
        #endregion

        #region WebHookID
        public abstract class webHookID : BqlGuid.Field<webHookID> { }
        [PXDBGuid(false)]
        [PXDefault(typeof(SurveySetup.webHookID))]
        [PXUIField(DisplayName = "Web Hook")]
        [PXForeignReference(typeof(Field<webHookID>.IsRelatedTo<Api.Webhooks.DAC.WebHook.webHookID>)),]
        [PXSelector(typeof(Api.Webhooks.DAC.WebHook.webHookID),
            new Type[] { typeof(Api.Webhooks.DAC.WebHook.name), typeof(Api.Webhooks.DAC.WebHook.isActive),
                typeof(Api.Webhooks.DAC.WebHook.isSystem) },
            DescriptionField = typeof(Api.Webhooks.DAC.WebHook.name),
            SubstituteKey = typeof(Api.Webhooks.DAC.WebHook.name))]
        public Guid? WebHookID { get; set; }
        #endregion

        #region Target
        public abstract class target : BqlString.Field<target> { }
        [PXDBString(1, IsUnicode = false, IsFixed = true)]
        [PXDefault(SurveyTarget.User)]
        [PXUIField(DisplayName = "Target", Visibility = PXUIVisibility.SelectorVisible)]
        [SurveyTarget.List]
        public virtual string Target { get; set; }
        #endregion

        #region Layout
        public abstract class layout : BqlString.Field<layout> { }
        [PXDBString(1, IsUnicode = false, IsFixed = true)]
        [PXDefault(SurveyLayout.SinglePage)]
        [PXUIField(DisplayName = "Layout", Visibility = PXUIVisibility.SelectorVisible)]
        [SurveyLayout.List]
        public virtual string Layout { get; set; }
        #endregion

        #region TemplateID
        public abstract class templateID : BqlInt.Field<templateID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Template")]
        [PXDefault(typeof(SurveySetup.templateID))]
        [PXForeignReference(typeof(FK.SUSurveyTemplate)), ]
        [PXSelector(typeof(Search<SurveyComponent.componentID, Where<SurveyComponent.componentType, Equal<SUComponentType.survey>>>), 
            DescriptionField = typeof(SurveyComponent.description),
            SubstituteKey = typeof(SurveyComponent.description))]
        public virtual int? TemplateID { get; set; }
        #endregion

        #region Active
        public abstract class active : BqlBool.Field<active> { }

        [PXDBBool]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        #endregion

        #region WorkgroupID
        public abstract class workgroupID : BqlInt.Field<workgroupID> { }
        [PXDBInt]
        //[PXDefault(typeof(BAccount.workgroupID), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXCompanyTreeSelector]
        [PXUIField(DisplayName = "Workgroup")]
        public virtual int? WorkgroupID { get; set; }
        #endregion

        #region OwnerID
        public abstract class ownerID : BqlInt.Field<ownerID> { }
        [PXDefault(typeof(Search<CREmployee.defContactID, Where<CREmployee.userID, Equal<Current<AccessInfo.userID>>, And<CREmployee.status, NotEqual<BAccount.status.inactive>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [Owner(typeof(workgroupID), null, false, false, null, null)] // Do not validate : Did not allow some value when data was coming from Shipments
        public virtual int? OwnerID { get; set; }
        #endregion

        #region NotificationID
        public abstract class notificationID : BqlInt.Field<notificationID> { }
        [PXDBInt]
        [PXDefault(typeof(SurveySetup.notificationID))]
        [PXSelector(typeof(Search<SM.Notification.notificationID, Where<SM.Notification.screenID, Equal<SurveyUtils.surveyScreen>>>), SubstituteKey = typeof(SM.Notification.name))]
        [PXUIField(DisplayName = "New Notification")]
        public virtual int? NotificationID { get; set; }
        #endregion

        #region RemindNotificationID
        public abstract class remindNotificationID : BqlInt.Field<remindNotificationID> { }
        [PXDBInt]
        [PXDefault(typeof(SurveySetup.remindNotificationID))]
        [PXSelector(typeof(Search<SM.Notification.notificationID, Where<SM.Notification.screenID, Equal<SurveyUtils.surveyScreen>>>), SubstituteKey = typeof(SM.Notification.name))]
        [PXUIField(DisplayName = "Reminder Notification")]
        public virtual int? RemindNotificationID { get; set; }
        #endregion

        #region EntityType
        public abstract class entityType : BqlString.Field<entityType> { }
        [PXDBString(256, IsUnicode = true)]
        [PXEntityTypeList]
        [PXDefault("", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Entity Type", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string EntityType { get; set; }
        #endregion

        #region IsSurveyInUse
        public abstract class isSurveyInUse : BqlBool.Field<isSurveyInUse> { }
        /// <summary>
        /// Field to identify if Survey is in use
        /// </summary>
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXBool]
        public virtual bool? IsSurveyInUse { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : BqlGuid.Field<noteID> { }
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion

        #region CreatedByID
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        [PXDBCreatedByID]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedByID, Enabled = false)]
        public virtual Guid? CreatedByID { get; set; }
        #endregion

        #region CreatedByScreenID
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        #endregion

        #region CreatedDateTime
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        [PXDBCreatedDateTime]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false)]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion

        #region LastModifiedByID
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        [PXDBLastModifiedByID]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedByID, Enabled = false)]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion

        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        #endregion

        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        [PXDBLastModifiedDateTime]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime, Enabled = false)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region tstamp
        public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        #endregion
    }
}