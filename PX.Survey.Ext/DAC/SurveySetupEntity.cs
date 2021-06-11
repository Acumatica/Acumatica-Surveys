using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXCacheName(Messages.CacheNames.SurveySetupEntity)]
    public partial class SurveySetupEntity : IBqlTable, INotable {

        #region EntityType
        public abstract class entityType : BqlString.Field<entityType> { }
        [PXDBString(256, IsKey = true, IsUnicode = true)]
        [PXDefault]
        [PXEntityTypeList]
        [PXUIField(DisplayName = "Entity Type", Required = true)]
        public virtual string EntityType { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlInt.Field<surveyID> { }
        [PXDBInt]
        [PXDefault]
        [PXSelector(typeof(Search<Survey.surveyID>),
            typeof(Survey.active),
            typeof(Survey.title),
            SubstituteKey = typeof(Survey.title),
            DescriptionField = typeof(Survey.title))]
        [PXUIField(DisplayName = "Survey", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
        public virtual int? SurveyID { get; set; }
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

