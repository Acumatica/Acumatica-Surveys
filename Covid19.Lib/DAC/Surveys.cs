using System;
using PX.Data;
using PX.Objects.CS;

namespace Covid19.Lib
{
    /// <summary>
    /// The Survey entity is used to define specific surveys
    /// This is the master record for the Survey 
    /// </summary>
    [Serializable]
    public class Surveys : IBqlTable
    {
        //#region SurveyID
        ///// <summary>
        ///// This sets a surrogate key for the Survey
        ///// </summary>
        /////  todo:  determine how we want to do this field.
        ////          in 04/05/2020 meeting i believe we decided to use a Auto Incremented number
        ////          so I believe this will entail a property something on the order of an OrderNbr
        ////          in the SalesOrder DAC
        ////          I am not clear what we need to do to get this to an auto number.
        ////          1) do we keep this SurveyID field as a integer primary key and add a SurveyNbr field. or
        ////          2) alter the properties of this to be a Alpha numeric and then wire this into a auto number
        //[PXDBInt(IsKey = true)]
        //[PXUIField(DisplayName = "Survey ID")]
        //public virtual int? SurveyID { get; set; }
        //public abstract class surveyID : PX.Data.BQL.BqlInt.Field<surveyID> { }
        //#endregion

        #region SurveyID_AsSrring
        [PXDBString(40, IsKey = true)]
        [PXUIField(DisplayName = "Survey ID")]
        [AutoNumber(typeof(CRSetupSurveyExt.usrSurveyNumberingID), typeof(AccessInfo.businessDate))]
        public virtual string SurveyID { get; set; }
        public abstract class surveyID : PX.Data.BQL.BqlString.Field<surveyID> { }
        #endregion

        #region SurveyName
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Survey Name")]
        public virtual string SurveyName { get; set; }
        public abstract class surveyName : PX.Data.BQL.BqlString.Field<surveyName> { }
        #endregion
        #region SurveyDesc
        /// <summary>
        /// Provides a description for the Survey
        /// </summary>
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Survey Description")]
        public virtual string SurveyDesc { get; set; }
        public abstract class surveyDesc : PX.Data.BQL.BqlString.Field<surveyDesc> { }
        #endregion
        #region Active
        /// <summary>
        /// The Active property will allow to deactivate an entire survey
        /// </summary>
        /// <remarks>
        /// Its was discussed on the 04/05/2020 meeting that the Active property should not
        /// be active by default when the field is first crated. Once the survey is active
        /// it should then render the survey immutable.
        /// Todo:   make sure the Survey form immutable once the survey it set to inactive.
        ///         Also discuss with team if we want to follow through with the immutable
        ///         requirement as a phase 2 or if its needed for phase one. The simpler
        ///         we can make phase one the sooner we can deliver something functional. 
        /// </remarks>
        [PXDBBool()]
        [PXUIField(DisplayName = "Active")]
        //todo: make sure this defaults to false
        //      need to determin the correct syntax needed to default this to true
        //      neither of the two below work.
        //[PXDBDefault(false)] 
        //[PXDBDefault(typeof(false))]
        public virtual bool? Active { get; set; }
        public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
        #endregion
        #region recuring

        /*
         A recurring property has been discussed in Team meetings to handle 
         setting daily, weekly, monthly type setups. On further discussion, 
         we decided to first look at using the Processing Graph to set any 
         recurring logistics there being that it already has the foundation 
         to be able to configure it
         */
        /*
        In the 04/05/2020 meeting the recurring specification being needed came up 
        once again. after the discussion it was deemed that we need to link this record
        to a schedule record as opposed to reinventing the wheal here. 
        Todo: follow up with determining how we can accomplish the above. 
            all of this properties should be derivable from the schedule record once we determin how to link it to 
            this Survey record.
            Survey Frequency - Daily / Weekly / Monthly - (Not needed as a DB field and use scheduler)
            Survey Timeframe -  (Not needed as a DB field and use scheduler)
            Survey Duration  -  Survey can be open to run this survey on frequency - 30 days   
        */



        #endregion
        #region NoteID
        public abstract class noteID : PX.Data.IBqlField { }

        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion
        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion
        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion
        #region CreatedDateTime
        //[PXDBDate()]
        [PXDBCreatedDateTime]
        [PXUIField(DisplayName = "Created Date Time")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion
        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion
        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion
        #region LastModifiedDateTime
        //[PXDBDate()]
        [PXDBLastModifiedDateTime]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
        #region LineCntr
        //copied from SalesOrder
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
        protected Int32? _LineCntr;
        [PXDBInt()]
        [PXDefault(0)]
        public virtual Int32? LineCntr
        {
            get
            {
                return this._LineCntr;
            }
            set
            {
                this._LineCntr = value;
            }
        }
        #endregion
    }
}

/*
--DROP TABLE Survey

--The Survey table is used to define specific surveys 

CREATE TABLE Survey
(
CompanyID Int NOT NULL,
SurveyID Nvarchar(40) NOT NULL,
SurveyName nvarchar(100) NOT NULL,
SurveyDesc nvarchar(255),
Active bit, --use this to deactivate the survey
NoteID uniqueidentifier NOT NULL,
CreatedByID uniqueidentifier NOT NULL,
CreatedByScreenID char(8) NOT NULL,
CreatedDateTime datetime NOT NULL,
LastModifiedByID uniqueidentifier NOT NULL,
LastModifiedByScreenID char(8) NOT NULL,
LastModifiedDateTime datetime NOT NULL,
CONSTRAINT [Survey_PK]
PRIMARY KEY CLUSTERED 
(
[CompanyID]
ASC,
[SurveyID]
ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

*/

