using System;
using PX.Data;

namespace Covid19.Lib
{
    /// <summary>
    /// This entity is used to coordinate gathering and attaching Survey answers to a specific time. 
    /// </summary>
    /// <remarks>
    ///    Todo: My assumptions have shifted a bit in how this is going to work. Need confirm with team the understanding is now correct.
    ///    My original assumption was that we would have a one to one relationship between a survey question and an answer in the CSAnswer table.
    ///    I now assume that is wrong and that this records will attach all of the answers to a Survey at a particular time to a single Collector record.
    ///    So this collector will link an answer record for each question in a given survey. When that survey is complete a new Collector record will
    ///    spin up at some future time for the user to re-answer at that future time.
    ///    So this is a one to many relationship to the CSAnswer table. 
    /// </remarks>
    [Serializable]
    public class SurveyCollector : IBqlTable
    {

        #region Selected
        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        //#region SurveyID
        ///// <summary>
        ///// Identifies the specific Survey this collector record belongs too.
        ///// </summary>
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

        #region CollectorID
        /// <summary>
        /// Uniquely Identifies this Collector record.
        /// </summary>
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Collector ID")]
        public virtual int? CollectorID { get; set; }
        public abstract class collectorID : PX.Data.BQL.BqlInt.Field<collectorID> { }
        #endregion
        #region CollectorCD

        public abstract class collectorCD : PX.Data.BQL.BqlString.Field<collectorCD> { }
        /// <summary>
        /// The human-readable identifier of the Collector record.
        /// </summary>
        /// <remarks>
        /// Todo:   confirm following assumption with Team:
        ///         We will and to have this field for any URL links sent to any Contact
        ///         the link will hold have this particular value in one of its query parameters.
        ///         as to be able to direct the users directly to this specific collector.
        ///         This is the field that zaljur was using in his Quiz page where he was
        ///         using the Survey Name then a DateTime as string within the QuizCD feild.
        ///         I belive the SurveyColector is the equivalent of his Quiz record.
        /// </remarks>
        [PXDBString(50, IsKey = true, IsUnicode = true)]//todo: confirm with team this is the right attribute
        [PXDefault]
        public virtual string CollectorCD { get; set; }
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
        #region CollectedDate
        /// <summary>
        /// Specifies the date that the Survey was collected
        /// </summary>
        /// <remarks>
        /// this should remain null until the survey is answered once the
        /// Survey is accepted the Graph should automatically set this date
        /// todo: Discuss with team if we should make this updateable. I assume we should just set it once survey is accepted. 
        /// </remarks>
        [PXDBDate()]
        [PXUIField(DisplayName = "Collected Date")]
        public virtual DateTime? CollectedDate { get; set; }
        public abstract class collectedDate : PX.Data.BQL.BqlDateTime.Field<collectedDate> { }
        #endregion
        #region ExpirationDate
        /// <summary>
        /// Specifies the date that this Collector expires. The user has up until this date to finish the survey.
        /// </summary>
        /// <remarks>
        /// Todo: a Graph or Processing page should handel automaticaly setting the CollectorState value to expired if is
        /// remains still open beyond this expiration date.
        /// </remarks>
        [PXDBDate()]
        [PXUIField(DisplayName = "Expiration Date")]
        public virtual DateTime? ExpirationDate { get; set; }
        public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
        #endregion
        #region CollectorStatus



        //Sent/Open/responded/expired
        private const string CollectorNew = "N";
        private const string CollectorSent = "S";
        /*
         Per discussions on 4/05/2020, we are not certain if Open will be needed, so it is commented out until we determine if we do really need it.
         */
        //private const string CollectorOpen = "O";
        private const string CollectorResponded = "R";
        private const string CollectorExpired = "E";

        protected string _CollectorStatus;
        /// <summary>
        /// Reference to the state the collector record is in   
        /// </summary>
        /// <remarks>
        /// Todo: confirm the below assumptions below with the team.
        /// 
        /// The document originally had an enumeration value of Sent. My thoughts are to name this Send as in the state
        /// of the record in in a state where the Survey needs to be sent to the user. Once the survey is delivered the
        /// state should then be set to Open meaning that the collection is open for the user to be answered. If the user
        /// neglects to answer the survey we will then set this to expired once the expiration date has passed. If the user
        /// does answer the survey the state gets set to Responded. 
        /// </remarks>
        public abstract class collectorStatus : PX.Data.BQL.BqlString.Field<collectorStatus> { }

        [PXDBString(1, IsUnicode = true)]
        [PXDefault(CollectorNew)]
        [PXUIField(DisplayName = "Collector Status")]
        [PXStringList(
            new[]
            {
                CollectorNew,
                CollectorSent,
                //CollectorOpen,
                CollectorResponded,
                CollectorExpired
            },
            new[]
            {
                "New",
                "Send",
                //"Open",
                "Responded",
                "Expired"
            })]
        public virtual string CollectorStatus
        {
            get => _CollectorStatus;
            set => _CollectorStatus = value;
        }



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

        #region DeadCode

        //#region QuestionID
        ////todo: purge this when the above assumption is confiremed. this was added on the assumption that we have a one to one relationship beteen this collector, Question, and CSAnswer.
        ////      this also make more sense why Harsha insisted we needed the SurveyID in this entity
        ////[PXDBInt(IsKey = true)]
        ////[PXUIField(DisplayName = "Question ID")]
        ////public virtual int? QuestionID { get; set; }
        ////public abstract class questionID : PX.Data.BQL.BqlInt.Field<questionID> { }
        //#endregion

        //#region Userid_Removed
        /*
         it has been decided on 04/05/2020 that the userID field will be replaced with Contact as it is the contact that we are most concerned regarding any Survey
        
        */
        ///// <summary>
        ///// Identifies the user that this Collector is assigned too.
        ///// </summary>
        //[PXDBGuid()]
        //[PXUIField(DisplayName = "Userid")]
        //public virtual Guid? Userid { get; set; }
        //public abstract class userid : PX.Data.BQL.BqlGuid.Field<userid> { }
        //#endregion

        #endregion
    }
}


/*
--DROP TABLE SurveyCollector
CREATE TABLE SurveyCollector
(
	CompanyID  Int not null,
	SurveyID   nvarchar(40) not null,
	CollectorID   Int not null,
    CollectorCD nvarchar(50) not null,
	ContactID Int not null,  
	CollectedDate DateTime not null,--the date the question was answered
	ExpirationDate datetime,
	CollectorStatus char(1), -- Status of record - New/Sent/responded/expired
	NoteID uniqueidentifier NOT NULL,
	CreatedByID uniqueidentifier NOT NULL,
	CreatedByScreenID char(8) NOT NULL,
	CreatedDateTime datetime NOT NULL,
	LastModifiedByID uniqueidentifier NOT NULL,
	LastModifiedByScreenID char(8) NOT NULL,
	LastModifiedDateTime datetime NOT NULL,
CONSTRAINT SurveyCollector_PK PRIMARY KEY CLUSTERED 
(
	CompanyID   ASC,
	SurveyID    ASC,
	CollectorID ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

 */
