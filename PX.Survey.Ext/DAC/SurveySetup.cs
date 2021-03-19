using PX.Data;
using PX.Objects.CS;
using System;

namespace PX.Survey.Ext {
    [Serializable]
    [PXPrimaryGraph(typeof(SurveySetupMaint))]
    [PXCacheName(Messages.SurveySetupCacheName)]
    public class SurveySetup : IBqlTable {
        #region SurveyNumberingID
        public abstract class surveyNumberingID : PX.Data.BQL.BqlString.Field<surveyNumberingID> { }

        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXDefault]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        [PXUIField(DisplayName = "Survey Numbering ID")]
        public virtual string SurveyNumberingID { get; set; }
        #endregion

        #region DemoSurvey
        public abstract class demoSurvey : PX.Data.BQL.BqlBool.Field<demoSurvey> { }

        [PXDBBool()]
        [PXUIField(DisplayName = "Created Demo Survey", Enabled = false)]
        public virtual bool? DemoSurvey { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        #endregion

        #region CreatedByID
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        #endregion

        #region CreatedByScreenID
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        #endregion

        #region CreatedDateTime
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion

        #region LastModifiedByID
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion

        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        #endregion

        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region tstamp
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
        [PXDBTimestamp()]
        public virtual byte[] tstamp { get; set; }
        #endregion
    }
}