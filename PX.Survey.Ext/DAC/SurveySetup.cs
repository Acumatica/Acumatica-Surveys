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
        public abstract class badRequestID : BqlInt.Field<badRequestID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Bad Request Template")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.badRequest>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? BadRequestID { get; set; }
        #endregion

        #region TemplateID
        public abstract class templateID : BqlInt.Field<templateID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Default Main Template")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.survey>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? TemplateID { get; set; }
        #endregion

        #region DefHeaderID
        public abstract class defHeaderID : BqlInt.Field<defHeaderID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Default Header")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.header>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? DefHeaderID { get; set; }
        #endregion

        #region DefPageHeaderID
        public abstract class defPageHeaderID : BqlInt.Field<defPageHeaderID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Default Page Header")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.pageHeader>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? DefPageHeaderID { get; set; }
        #endregion

        #region DefQuestionID
        public abstract class defQuestionID : BqlInt.Field<defQuestionID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Default Question")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.questionPage>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? DefQuestionID { get; set; }
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
        public abstract class defCommentID : BqlInt.Field<defCommentID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Default Comment")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.commentPage>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? DefCommentID { get; set; }
        #endregion

        #region DefCommAttrID
        public abstract class defCommAttrID : BqlString.Field<defCommAttrID> { }
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXDefault]
        [PXSelector(typeof(CS.CSAttribute.attributeID), DescriptionField = typeof(CS.CSAttribute.description))]
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
        public abstract class defPageFooterID : BqlInt.Field<defPageFooterID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Default Page Footer")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.pageFooter>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? DefPageFooterID { get; set; }
        #endregion

        #region DefFooterID
        public abstract class defFooterID : BqlInt.Field<defFooterID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Default Footer")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.footer>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? DefFooterID { get; set; }
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
        [PXUIField(DisplayName = "Default Web Hook")]
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