using System;
using PX.Data;

namespace Covid19.Lib
{
    /// <summary>
    /// This Entity establishes a many to many relationship between a user and
    /// participation in a survey.
    /// </summary>
    /// <remarks>
    /// Future iterations of this entity will allow for a relationship between
    /// a Customer, Vendor, or other enties in the system.
    /// todo: One thing that will need to be discussed with the above is that a user will
    /// need to be established in order for any given entity the ability to log in and
    /// fill out a survey. We could also consider moving the survey to the Self Service
    /// portal at a future point and tap into the existing feature set to establish a
    /// new user via a contact record in the system. 
    /// </remarks>
    [Serializable]
    public class SurveyRecipients : IBqlTable
    {
        //#region SurveyID
        ///// <summary>
        ///// References the Survey record this Cross reference is for
        ///// </summary>
        ///// <remarks>
        ///// Todo:   refactor this field into an Auto Number field similar to how OrderNbr work on a SalesOrder DAC
        /////         I am not sure what need to be done to make this happen. I assume we leave this as is and add a new
        /////         SurveyNbr field that would then be auto-numbered as it does for OrderNbr. 
        ///// </remarks>
        //[PXDBInt(IsKey = true)]
        //[PXUIField(DisplayName = "Survey ID")]
        //public virtual int? SurveyID { get; set; }
        //public abstract class surveyID : PX.Data.BQL.BqlInt.Field<surveyID> { }
        //#endregion

        #region SurveyID_AsSrring
        [PXDBString(40, IsKey = true)]
        [PXUIField(DisplayName = "Survey ID")]
        public virtual string SurveyID { get; set; }
        public abstract class surveyID : PX.Data.BQL.BqlString.Field<surveyID> { }
        #endregion

        #region LineNbr
        //copied from SOLine
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        protected Int32? _LineNbr;
        [PXDBInt(IsKey = true)]
        //todo: determin how the below attribute needs to be set.
        //[PXLineNbr(typeof(SOOrder.lineCntr))]
        [PXLineNbr(typeof(Surveys.lineCntr))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual Int32? LineNbr
        {
            get
            {
                return this._LineNbr;
            }
            set
            {
                this._LineNbr = value;
            }
        }
        #endregion


        #region ContactID
        ///// <summary>
        ///// Identifies the Contact that this Collector is assigned too.
        ///// </summary>
        /// <remarks>
        /// it has been decided on 04/05/2020 that the userID field will be replaced with
        /// Contact as it is the contact that we are most concerned regarding any Survey
        /// </remarks>
        [PXDBInt()]
        [PXUIField(DisplayName = "Contact")]
        public virtual int? ContactID { get; set; }
        public abstract class contactID : PX.Data.BQL.BqlInt.Field<contactID> { }
        #endregion

        #region Active
        /// <summary>
        /// This allows for deactivation of a recipient that was previously enabled for a survey.
        /// </summary>
        /// <remarks>
        /// If a cross reference record is never established that user will never get a Survey.
        /// The primary reason for this property is to allow a recipient to be turned off after
        /// they where once active. this will be needed to preserve any referential fact data for
        /// BI analysis
        /// </remarks>
        [PXDBBool()]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
        #endregion
        #region SuppressUntilDate
        //todo: It has been discussed having some mechanism to allow for temporary suppression
        //of Survey delivery if the user is away for any given reason such as Vacation, Paid Time Off,
        //performing diferent duties, ect... 
        //Any easy way to do this is to have a nullable date value and if if is set then only 
        //send the Survey if the date entered has already passed. if the date is some time in the 
        //future don't send the Survey.
        //Other options where discussed to maybe use an employee calendar and determine logic off that if it 
        //indicated being away.
        //
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

        #region Dead Code
        //no longer using USER ID as it has been decided the contact is what this whole project pivots around
        //#region UserID
        ///// <summary>
        ///// This holds the User ID that is associated with this cross reference record
        ///// </summary>
        ///// <remarks>
        ///// See remarks on the class header on notes about future iterations to get
        ///// this record to work with customer, vendor, or other entities. We likely
        ///// will always need a user as they will need to log in to fill out the survey
        ///// </remarks>
        //[PXDBGuid(IsKey = true)]
        //[PXUIField(DisplayName = "User ID")]
        //public virtual int? UserID { get; set; }
        //public abstract class userID : PX.Data.BQL.BqlInt.Field<userID> { }
        //#endregion


        #endregion
    }
}

/*
--DROP TABLE SurveyRecipients
CREATE TABLE SurveyRecipients
(
	CompanyID Int NOT NULL,
	SurveyID nvarchar(40) NOT NULL,
	--EmployeeID Int NOT NULL,
	--UserID uniqueidentifier NOT NULL,
	LineNbr Int NOT NULL,
	ContactID Int NOT NULL,
	Active bit, --default to true, employee only participates if this is true. to opt an employee out uncheck this box.
    NoteID uniqueidentifier NOT NULL,
	CreatedByID uniqueidentifier NOT NULL,
	CreatedByScreenID char(8) NOT NULL,
	CreatedDateTime datetime NOT NULL,
	LastModifiedByID uniqueidentifier NOT NULL,
	LastModifiedByScreenID char(8) NOT NULL,
	LastModifiedDateTime datetime NOT NULL,
CONSTRAINT [SurveyRecipients_PK] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[SurveyID]  ASC,
	--[EmployeeID] ASC
	--[UserID]    ASC
	[LineNbr] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
 */
