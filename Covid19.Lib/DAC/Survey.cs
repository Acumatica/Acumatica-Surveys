using System;
using PX.Data;

namespace Covid19.Lib
{
    /// <summary>
    /// The Survey entity is used to define specific surveys
    /// This is the master record for the Survey 
    /// </summary>
    [Serializable]
    public class Surveys : IBqlTable
    {
        #region SurveyID
        /// <summary>
        /// This sets a surrogate key for the Survey
        /// </summary>
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Survey ID")]        
        public virtual int? SurveyID { get; set; }
        public abstract class surveyID : PX.Data.BQL.BqlInt.Field<surveyID> { }
        #endregion
        //todo: determine if we really need a CD natural Key. I doubt its needed.
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
        [PXDBBool()]
        [PXUIField(DisplayName = "Active")]
        //[PXDBDefault(true)]
        public virtual bool? Active { get; set; }
        public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
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
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
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
    }
}

/*
--drop table SRVMaster

--The Survey table is used to define specific surveys 

Create Table Survey
(
CompanyID Int NOT NULL,
SurveyID int NOT NULL,
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
