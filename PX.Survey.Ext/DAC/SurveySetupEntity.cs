using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using System;
using System.Diagnostics;

namespace PX.Survey.Ext {

    [DebuggerDisplay("SurveySetupEntity: GraphType = {GraphType}, EntityType = {EntityType}, ContactField = {ContactField}")]
    [Serializable]
    [PXCacheName(Messages.CacheNames.SurveySetupEntity)]
    public partial class SurveySetupEntity : IBqlTable, ISortOrder, INotable {

        #region GraphType
        public abstract class graphType : BqlString.Field<graphType> { }
        [PXDBString(256, IsUnicode = true)]
        [PXDefault]
        [SUGraphSelector]
        [PXUIField(DisplayName = "Graph Type")]
        public virtual string GraphType { get; set; }
        #endregion

        #region EntityType
        public abstract class entityType : BqlString.Field<entityType> { }
        [PXDBString(256, IsUnicode = true)]
        [PXDefault]
        [PXEntityTypeList]
        [PXUIField(DisplayName = "Entity Type", IsReadOnly = true, Required = true)]
        public virtual string EntityType { get; set; }
        #endregion

        #region LineNbr
        public abstract class lineNbr : BqlInt.Field<lineNbr> { }
        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(SurveySetup))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual int? LineNbr { get; set; }
        #endregion

        #region SortOrder
        public abstract class sortOrder : BqlInt.Field<sortOrder> { }
        [PXUIField(DisplayName = "Line Order", Visible = false, Enabled = false)]
        [PXDBInt]
        public virtual int? SortOrder { get; set; }
        #endregion

        #region ContactField
        public abstract class contactField : BqlString.Field<contactField> { }
        ////[SUFieldList(typeof(entityType))]
        //[PXUIField(DisplayName = "Contact Field", Required = true)]
        //[PrimaryViewFieldsList(typeof(screenID), ShowDisplayNameAsLabel = true)]
        [PXDBString(64, InputMask = "", IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Contact Field")]
        public virtual string ContactField { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlString.Field<surveyID> { }
        [SurveyID]
        [PXSelector(typeof(Search<Survey.surveyID, Where<Survey.entityType, Equal<Current<entityType>>>>), DescriptionField = typeof(Survey.title))]
        public virtual string SurveyID { get; set; }
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

