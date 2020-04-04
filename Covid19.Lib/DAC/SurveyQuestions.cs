using System;
using PX.Data;

namespace Covid19.Lib
{
    /// <summary>
    /// this entity holds the specific questions that will be asked
    /// </summary>
    [Serializable]
    public class SurveyQuestions : IBqlTable
    {
        #region SurveyID
        /// <summary>
        /// This identifies the Survey this Question belongs to
        /// </summary>
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Survey ID")]
        public virtual int? SurveyID { get; set; }
        public abstract class surveyID : PX.Data.BQL.BqlInt.Field<surveyID> { }
        #endregion
        #region QuestionID
        /// <summary>
        /// Identifier for this specific Question.
        /// </summary>
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Question ID")]
        public virtual int? QuestionID { get; set; }
        public abstract class questionID : PX.Data.BQL.BqlInt.Field<questionID> { }
        #endregion
        #region Question
        /// <summary>
        /// the Question as it will be displayed to the user.
        /// </summary>
        [PXDBString(500, InputMask = "")]
        [PXUIField(DisplayName = "Question")]
        public virtual string Question { get; set; }
        public abstract class question : PX.Data.BQL.BqlString.Field<question> { }
        #endregion
        #region SortOrder
        /// <summary>
        /// This allows for sorting using a Alpha Numeric value 
        /// </summary>
        [PXDBString(20, InputMask = "")]
        [PXUIField(DisplayName = "Sort Order")]
        public virtual string SortOrder { get; set; }
        public abstract class sortOrder : PX.Data.BQL.BqlString.Field<sortOrder> { }
        #endregion
        #region QuestionGroup
        /// <summary>
        /// Allows fore grouping questions together.
        /// </summary>
        /// <remarks>
        /// Todo: We will still need to determine how this is going to work
        /// At this point my idea is to use a field like this where if a set
        /// of questions have the same group value such as Symptoms those questions
        /// will be grouped together.
        /// Now that i have the understanding that these will be in a Grid instead we may
        /// not end up needing this field. We could control the same desired result with the
        /// sort order above.
        /// </remarks>
        [PXDBString(40, InputMask = "")]
        [PXUIField(DisplayName = "Question Group")]
        public virtual string QuestionGroup { get; set; }
        public abstract class questionGroup : PX.Data.BQL.BqlString.Field<questionGroup> { }
        #endregion
        #region AttributeID
        /// <summary>
        /// Associates the Question to an Attribute
        /// </summary>
        /// <remarks>
        /// An attribute will handel the data type and any enumeration values that will be associated with an
        /// answer.
        /// </remarks>
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        //todo: need to determine the correct attributes needed to be able to link to an Acumatica Attribute
        [PXUIField(DisplayName = "Attribute ID")]
        public virtual string AttributeID { get; set; }
        public abstract class attributeID : PX.Data.BQL.BqlString.Field<attributeID> { }
        #endregion
        #region NoteID
        [PXDBGuid()]//todo: confirm this is the correct attribute
        [PXUIField(DisplayName = "NoteID")]
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

        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
    }
}

/*
Create Table SurveyQuestions
(
	CompanyID  Int,
	SurveyID   int,
	QuestionID int,
	--This is the how the question should be worded to the user.
	Question varchar(500),
	--alpha numeric method to order the questions
	SortOrder varchar(20), 
	--Question Group will be used to group such things as 
	--Symptoms together in one block
	--we also have the option of defining this through another table.
	--Keeping it simple for now
	QuestionGroup varchar(40), 
	--I am working off of assumtions how the Attributes work.
	--Its assumed that we need to tie the quetion to a particular
	--Attribute as it will define the type and enumberations if 
	--applicable
	AttributeID nvarchar(10),
	NoteID uniqueidentifier NOT NULL,
	CreatedByID uniqueidentifier NOT NULL,
	CreatedByScreenID char(8) NOT NULL,
	CreatedDateTime datetime NOT NULL,
	LastModifiedByID uniqueidentifier NOT NULL,
	LastModifiedByScreenID char(8) NOT NULL,
	LastModifiedDateTime datetime NOT NULL,
CONSTRAINT SurveyQuestions_PK PRIMARY KEY CLUSTERED 
(
	CompanyID ASC,
	SurveyID  ASC,
	QuestionID ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
 */

