using System;
using PX.Data;
using PX.Objects.CS;

namespace PX.Survey.Ext
{
    [Serializable]
    [PXPrimaryGraph(typeof(SurveyMaint))]
    [PXCacheName(Messages.SurveyCacheName)]
    public class Survey : IBqlTable
    {
        #region SurveyID
        public abstract class surveyID : PX.Data.BQL.BqlInt.Field<surveyID> { }

        [PXDBIdentity]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual int? SurveyID { get; set; }
        #endregion

        #region SurveyCD
        public abstract class surveyCD : PX.Data.BQL.BqlString.Field<surveyCD> { }

        [PXDefault]
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Survey.surveyCD),
                    typeof(Survey.surveyCD),
                    typeof(Survey.surveyName))]
        [AutoNumber(typeof(SurveySetup.surveyNumberingID), typeof(AccessInfo.businessDate))]
        public virtual string SurveyCD { get; set; }
        #endregion

        #region SurveyName
        public abstract class surveyName : PX.Data.BQL.BqlString.Field<Survey.surveyName> { }

        [PXDefault]
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Survey Name")]
        public virtual string SurveyName { get; set; }
        #endregion

        #region SurveyDesc
        public abstract class surveyDesc : PX.Data.BQL.BqlString.Field<Survey.surveyDesc> { }
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Survey Description")]
        public virtual string SurveyDesc { get; set; }
        #endregion

        #region Active
        public abstract class active : PX.Data.BQL.BqlBool.Field<Survey.active> { }
        
        [PXDBBool()]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<Survey.noteID> { }
        [PXNote()]
        public virtual Guid? NoteID { get; set; }        
        #endregion

        #region LineCntr
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
        [PXDBInt()]
        [PXDefault(0)]
        public virtual Int32? LineCntr { get; set; }
        #endregion
        #region CreatedByID
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<Survey.createdByID> { }
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        #endregion
        #region CreatedByScreenID
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<Survey.createdByScreenID> { }
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        #endregion
        #region CreatedDateTime
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<Survey.createdDateTime> { }
        [PXDBCreatedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Created Date Time", Enabled = false)]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion
        #region LastModifiedByID
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<Survey.lastModifiedByID> { }
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<Survey.lastModifiedByScreenID> { }
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        #endregion
        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<Survey.lastModifiedDateTime> { }
        [PXDBLastModifiedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Last Modified Date Time", Enabled = false)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion
        #region tstamp
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
        [PXDBTimestamp]
        public virtual Byte[] tstamp { get; set; }
        #endregion

        #region SurveyIDStringID
        public abstract class surveyIDStringID : PX.Data.BQL.BqlString.Field<surveyIDStringID> { }
        [PXString]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual string SurveyIDStringID => this.SurveyID.ToString();
        #endregion
    }
}