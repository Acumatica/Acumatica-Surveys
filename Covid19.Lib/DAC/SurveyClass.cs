using System;
using PX.Data;
using PX.Objects.CS;

namespace Covid19.Lib
{
    [Serializable]
    public class SurveyClass : IBqlTable
    {
        #region SurveyClassID
        public abstract class surveyClassID : PX.Data.BQL.BqlInt.Field<surveyClassID> { }

        [PXDBIdentity]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual int? SurveyClassID { get; set; }
        #endregion

        #region SurveyCD
        [PXDefault]
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(SurveyClass.surveyCD),
                    typeof(SurveyClass.surveyCD),
                    typeof(SurveyClass.surveyName))]
        [AutoNumber(typeof(CRSetupSurveyExt.usrSurveyNumberingID), typeof(AccessInfo.businessDate))]
        public virtual string SurveyCD { get; set; }
        public abstract class surveyCD : PX.Data.BQL.BqlString.Field<surveyCD> { }
        #endregion

        #region SurveyName
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Survey Name")]
        public virtual string SurveyName { get; set; }
        public abstract class surveyName : PX.Data.BQL.BqlString.Field<SurveyClass.surveyName> { }
        #endregion
        #region SurveyDesc
        /// <summary>
        /// Provides a description for the Survey
        /// </summary>
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Survey Description")]
        public virtual string SurveyDesc { get; set; }
        public abstract class surveyDesc : PX.Data.BQL.BqlString.Field<SurveyClass.surveyDesc> { }
        #endregion
        #region Active
        /// <summary>
        /// The Active property will allow to deactivate an entire survey
        /// </summary>
        [PXDBBool()]
        [PXUIField(DisplayName = "Active")]

        public virtual bool? Active { get; set; }
        public abstract class active : PX.Data.BQL.BqlBool.Field<SurveyClass.active> { }
        #endregion
        #region recuring

        /*
         A recuring property has been discussed in Team meetings to handle 
         setting daily, weekly, monthly type setups. On further discussion, 
         we decided to first look at using the Processing Graph to set any 
         recurring logistics there being that it already has the foundation 
         to be able to configure it
         */

        #endregion
        #region NoteID

        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<SurveyClass.noteID> { }
        #endregion
        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<SurveyClass.createdByID> { }
        #endregion
        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<SurveyClass.createdByScreenID> { }
        #endregion
        #region CreatedDateTime
        //[PXDBDate()]
        [PXDBCreatedDateTime]
        [PXUIField(DisplayName = "Created Date Time")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<SurveyClass.createdDateTime> { }
        #endregion
        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<SurveyClass.lastModifiedByID> { }
        #endregion
        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<SurveyClass.lastModifiedByScreenID> { }
        #endregion
        #region LastModifiedDateTime

        [PXDBLastModifiedDateTime]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<SurveyClass.lastModifiedDateTime> { }
        #endregion
        #region tstamp
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
        protected Byte[] _tstamp;
        [PXDBTimestamp]
        public virtual Byte[] tstamp
        {
            get
            {
                return this._tstamp;
            }
            set
            {
                this._tstamp = value;
            }
        }
        #endregion

    }
}