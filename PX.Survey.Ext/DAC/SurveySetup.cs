using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using PX.Objects.CS;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXPrimaryGraph(typeof(SurveySetupMaint))]
    [PXCacheName(Messages.CacheNames.SurveySetup)]
    public class SurveySetup : IBqlTable, INotable {

        #region SurveyNumberingID
        public abstract class surveyNumberingID : BqlString.Field<surveyNumberingID> { }
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXDefault("SURVEY")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        [PXUIField(DisplayName = "Survey Numbering ID")]
        public virtual string SurveyNumberingID { get; set; }
        #endregion

        #region BadRequestID
        public abstract class badRequestID : BqlString.Field<badRequestID> { }
        [ComponentID(typeof(Where<SurveyComponent.componentType, Equal<SUComponentType.badRequest>>), DisplayName = "Bad Request Template", Required = true)]
        [PXDefault("SUBADREQUEST")]
        public virtual string BadRequestID { get; set; }
        #endregion

        #region TemplateID
        public abstract class templateID : BqlString.Field<templateID> { }
        [ComponentID(typeof(Where<SurveyComponent.componentType, Equal<SUComponentType.survey>>), DisplayName = "Default Main Template", Required = true)]
        [PXDefault("SUMAIN")]
        public virtual string TemplateID { get; set; }
        #endregion

        #region DefHeaderID
        public abstract class defHeaderID : BqlString.Field<defHeaderID> { }
        [ComponentID(typeof(Where<SurveyComponent.componentType, Equal<SUComponentType.header>>), DisplayName = "Default Header", Required = true)]
        [PXDefault("SUWELCOME")]
        public virtual string DefHeaderID { get; set; }
        #endregion

        #region DefPageHeaderID
        public abstract class defPageHeaderID : BqlString.Field<defPageHeaderID> { }
        [ComponentID(typeof(Where<SurveyComponent.componentType, Equal<SUComponentType.pageHeader>>), DisplayName = "Default Page Header", Required = true)]
        [PXDefault("SUPROGRESS")]
        public virtual string DefPageHeaderID { get; set; }
        #endregion

        #region DefQuestionID
        public abstract class defQuestionID : BqlString.Field<defQuestionID> { }
        [ComponentID(typeof(Where<SurveyComponent.componentType, Equal<SUComponentType.questionPage>>), DisplayName = "Default Question", Required = true)]
        [PXDefault("SURADIOBUTTONS")]
        public virtual string DefQuestionID { get; set; }
        #endregion

        #region DefQuestAttrID
        public abstract class defQuestAttrID : BqlString.Field<defQuestAttrID> { }
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXDefault]
        [PXSelector(typeof(CS.CSAttribute.attributeID), DescriptionField = typeof(CS.CSAttribute.description))]
        [PXUIField(DisplayName = "Default Question Type")]
        public virtual string DefQuestAttrID { get; set; }
        #endregion

        #region DefCommentID
        public abstract class defCommentID : BqlString.Field<defCommentID> { }
        [ComponentID(typeof(Where<SurveyComponent.componentType, Equal<SUComponentType.commentPage>>), DisplayName = "Default Comment", Required = true)]
        [PXDefault("SUTEXTAREA")]
        public virtual string DefCommentID { get; set; }
        #endregion

        #region DefCommAttrID
        public abstract class defCommAttrID : BqlString.Field<defCommAttrID> { }
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXDefault("SUCOMMENT")]
        [PXSelector(typeof(Search<CS.CSAttribute.attributeID, Where<CS.CSAttribute.controlType, Equal<SUControlType.text>>>), DescriptionField = typeof(CS.CSAttribute.description))]
        [PXUIField(DisplayName = "Default Comment Type")]
        public virtual string DefCommAttrID { get; set; }
        #endregion

        #region DefNbrOfRows
        public abstract class defNbrOfRows : BqlInt.Field<defNbrOfRows> { }
        [PXDBInt]
        [PXDefault(3, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Def. Comment Rows")]
        public virtual int? DefNbrOfRows { get; set; }
        #endregion

        #region DefMaxLength
        public abstract class defMaxLength : BqlInt.Field<defMaxLength> { }
        [PXDBInt]
        [PXDefault(20, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Def. Max Length")]
        public virtual int? DefMaxLength { get; set; }
        #endregion

        #region DefPageFooterID
        public abstract class defPageFooterID : BqlString.Field<defPageFooterID> { }
        [ComponentID(typeof(Where<SurveyComponent.componentType, Equal<SUComponentType.pageFooter>>), DisplayName = "Default Page Footer", Required = true)]
        [PXDefault("SUPREVNEXT")]
        public virtual string DefPageFooterID { get; set; }
        #endregion

        #region DefFooterID
        public abstract class defFooterID : BqlString.Field<defFooterID> { }
        [ComponentID(typeof(Where<SurveyComponent.componentType, Equal<SUComponentType.footer>>), DisplayName = "Default Footer", Required = true)]
        [PXDefault("SUTHANKYOU")]
        public virtual string DefFooterID { get; set; }
        #endregion

        #region NotificationID
        public abstract class notificationID : BqlInt.Field<notificationID> { }
        [PXDBInt]
        [PXSelector(typeof(Search<SM.Notification.notificationID, Where<SM.Notification.screenID, Equal<SurveyUtils.surveyScreen>>>), SubstituteKey = typeof(SM.Notification.name))]
        [PXUIField(DisplayName = "New Notification")]
        public virtual int? NotificationID { get; set; }
        #endregion

        #region RemindNotificationID
        public abstract class remindNotificationID : BqlInt.Field<remindNotificationID> { }
        [PXDBInt]
        [PXSelector(typeof(Search<SM.Notification.notificationID, Where<SM.Notification.screenID, Equal<SurveyUtils.surveyScreen>>>), SubstituteKey = typeof(SM.Notification.name))]
        [PXUIField(DisplayName = "Reminder Notification")]
        public virtual int? RemindNotificationID { get; set; }
        #endregion

        #region ContactID
        public abstract class contactID : BqlInt.Field<contactID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Sample Contact")]
        [PXSelector(typeof(Search<Contact.contactID,
                            Where<Contact.contactType, Equal<ContactTypesAttribute.employee>,
                            And<Contact.isActive, Equal<True>, And<Contact.userID, IsNotNull>>>>),
                    DescriptionField = typeof(Contact.displayName))]
        public virtual int? ContactID { get; set; }
        #endregion

        #region AnonContactID
        public abstract class anonContactID : BqlInt.Field<anonContactID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Anonymous Contact")]
        [PXSelector(typeof(Search<Contact.contactID, Where<Contact.isActive, Equal<True>>>),
                    DescriptionField = typeof(Contact.displayName))]
        public virtual int? AnonContactID { get; set; }
        #endregion

        #region WebHookID
        public abstract class webHookID : BqlGuid.Field<webHookID> { }
        [PXDBGuid(false)]
        [PXDefault]
        [PXUIField(DisplayName = "Web Hook")]
        [PXSelector(typeof(Api.Webhooks.DAC.WebHook.webHookID), 
            new Type[] { typeof(Api.Webhooks.DAC.WebHook.name), typeof(Api.Webhooks.DAC.WebHook.isActive), 
                typeof(Api.Webhooks.DAC.WebHook.isSystem) },
            DescriptionField = typeof(Api.Webhooks.DAC.WebHook.name),
            SubstituteKey = typeof(Api.Webhooks.DAC.WebHook.name))]
        public Guid? WebHookID { get; set; }
        #endregion


        #region NoteID
        public abstract class noteID : BqlGuid.Field<noteID> { }
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion

        #region CreatedByID
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        [PXDBCreatedByID]
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
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion

        #region LastModifiedByID
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        [PXDBLastModifiedByID]
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
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region tstamp
        public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        #endregion
    }
}