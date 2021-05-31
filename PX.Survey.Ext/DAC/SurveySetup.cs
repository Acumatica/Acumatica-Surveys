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

        #region TemplateID
        public abstract class templateID : BqlInt.Field<templateID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Bad Request Template")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.badRequest>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? TemplateID { get; set; }
        #endregion

        #region PHHeaderID
        public abstract class pHHeaderID : BqlInt.Field<pHHeaderID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Place Holder Header")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.header>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? PHHeaderID { get; set; }
        #endregion

        #region PHPageHeaderID
        public abstract class pHPageHeaderID : BqlInt.Field<pHPageHeaderID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Place Holder Page Header")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.pageHeader>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? PHPageHeaderID { get; set; }
        #endregion

        #region PHQuestionID
        public abstract class pHQuestionID : BqlInt.Field<pHQuestionID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Place Holder Question")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.questionPage>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? PHQuestionID { get; set; }
        #endregion

        #region PHPageFooterID
        public abstract class pHPageFooterID : BqlInt.Field<pHPageFooterID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Place Holder Page Footer")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.pageFooter>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? PHPageFooterID { get; set; }
        #endregion

        #region PHFooterID
        public abstract class pHFooterID : BqlInt.Field<pHFooterID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Place Holder Footer")]
        [PXDefault]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.footer>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? PHFooterID { get; set; }
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