using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.Webhooks;
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
            public static Survey Find(PXGraph graph, int? surveyID) => FindBy(graph, surveyID);
            public static Survey FindDirty(PXGraph graph, int? surveyID)
                => (Survey)PXSelect<Survey, Where<surveyID, Equal<Required<surveyID>>>>.SelectWindowed(graph, 0, 1, surveyID);
        }
        public class UK : PrimaryKeyOf<Survey>.By<surveyCD> {
            public static Survey Find(PXGraph graph, string surveyCD) => FindBy(graph, surveyCD);
        }
        public static class FK {
            public class SUSurveyTemplate : SurveyTemplate.PK.ForeignKeyOf<Survey>.By<templateID> { }
            //public class WebHook : SurveyTemplate.PK.ForeignKeyOf<Survey>.By<webHookID> { }
        }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlInt.Field<surveyID> { }
        [PXDBIdentity]
        public virtual int? SurveyID { get; set; }
        #endregion

        #region SurveyCD
        public abstract class surveyCD : BqlString.Field<surveyCD> { }
        [PXDefault]
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Survey.surveyCD))]
        [AutoNumber(typeof(SurveySetup.surveyNumberingID), typeof(AccessInfo.businessDate))]
        public virtual string SurveyCD { get; set; }
        #endregion

        #region Title
        public abstract class title : BqlString.Field<title> { }
        [PXDefault]
        [DBMatrixLocalizableDescription(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Title")]
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
        [PXUIField(DisplayName = "Target")]
        [SurveyTarget.List]
        public virtual string Target { get; set; }
        #endregion

        #region Layout
        public abstract class layout : BqlString.Field<layout> { }
        [PXDBString(1, IsUnicode = false, IsFixed = true)]
        [PXDefault(SurveyLayout.SinglePage)]
        [PXUIField(DisplayName = "Layout")]
        [SurveyLayout.List]
        public virtual string Layout { get; set; }
        #endregion

        #region TemplateID
        public abstract class templateID : BqlInt.Field<templateID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Template")]
        [PXDefault]
        [PXForeignReference(typeof(FK.SUSurveyTemplate)), ]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<SUTemplateType.survey>>>), 
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? TemplateID { get; set; }
        #endregion

        #region Active
        public abstract class active : BqlBool.Field<Survey.active> { }

        [PXDBBool]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        #endregion

        //#region Template
        //public abstract class template : BqlString.Field<template> { }
        //[PXDBLocalizableString(IsUnicode = true)]
        //[PXUIField(DisplayName = "Template")]
        //public virtual string Template { get; set; }
        //#endregion

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
        //[PXFormula(typeof(Selector<bAccountID, BAccount.ownerID>))]
        public virtual int? OwnerID { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : BqlGuid.Field<Survey.noteID> { }
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion

        #region CreatedByID
        public abstract class createdByID : BqlGuid.Field<Survey.createdByID> { }
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        #endregion
        #region CreatedByScreenID
        public abstract class createdByScreenID : BqlString.Field<Survey.createdByScreenID> { }
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        #endregion
        #region CreatedDateTime
        public abstract class createdDateTime : BqlDateTime.Field<Survey.createdDateTime> { }
        [PXDBCreatedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Created Date Time", Enabled = false)]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion
        #region LastModifiedByID
        public abstract class lastModifiedByID : BqlGuid.Field<Survey.lastModifiedByID> { }
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : BqlString.Field<Survey.lastModifiedByScreenID> { }
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        #endregion
        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : BqlDateTime.Field<Survey.lastModifiedDateTime> { }
        [PXDBLastModifiedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Last Modified Date Time", Enabled = false)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion
        #region tstamp
        public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        #endregion

        #region SurveyIDStringID
        public abstract class surveyIDStringID : BqlString.Field<surveyIDStringID> { }
        [PXString]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual string SurveyIDStringID => this.SurveyID.ToString();
        #endregion

        #region IsSurveyInUse
        public abstract class isSurveyInUse : BqlBool.Field<Survey.isSurveyInUse> { }
        /// <summary>
        /// Field to identify if Survey is in use
        /// </summary>
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXBool]
        public virtual bool? IsSurveyInUse { get; set; }
        #endregion
    }
}