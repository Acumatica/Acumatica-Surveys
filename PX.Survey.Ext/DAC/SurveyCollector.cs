using System;
using PX.Data;
using PX.Objects.CR;

namespace PX.Survey.Ext
{
    /// <summary>
    /// This entity is used to coordinate gathering and attaching Survey answers to a specific time. 
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.SurveyCollectorCacheName)]
    [PXPrimaryGraph(typeof(SurveyCollectorMaint))]
    public class SurveyCollector : IBqlTable
    {
        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }

        /// <summary>
        /// Selected
        /// </summary>
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region CollectorID
        public abstract class collectorID : PX.Data.BQL.BqlInt.Field<collectorID> { }

        /// <summary>
        /// Uniquely Identifies this Collector record.
        /// </summary>
        [PXUIField(DisplayName = "Collector ID", Visible = false)]
        [PXDBIdentity(IsKey = true, BqlField = typeof(collectorID))]
        [PXSelector(typeof(Search<SurveyCollector.collectorID>))]
        public virtual int? CollectorID { get; set; }
        #endregion

        #region CollectorName
        public abstract class collectorName : PX.Data.BQL.BqlInt.Field<collectorName> { }

        /// <summary>
        /// Name of this Collector record.
        /// </summary>
        [PXUIField(DisplayName = "Collector", Enabled = false)]
        [PXDBString(60, IsUnicode = true)]
        public virtual String CollectorName { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : PX.Data.BQL.BqlInt.Field<surveyID> { }

        /// <summary>
        /// Identifies the specific Survey this collector record belongs too.
        /// </summary>
        [PXUIField(DisplayName = "Survey ID", Enabled = false)]
        [PXSelector(typeof(Search<Survey.surveyID, Where<Survey.active, Equal<True>>>),
                    typeof(Survey.surveyCD),
                    typeof(Survey.surveyName),
                    DescriptionField = typeof(Survey.surveyName))]
        [PXDBInt]
        public virtual int? SurveyID { get; set; }
        #endregion

        #region UserID
        public abstract class userID : PX.Data.BQL.BqlGuid.Field<userID> { }

        /// <summary>
        /// Identifies the user that this Collector is assigned too.
        /// </summary>
        [PXDBGuid()]
        [PXUIField(DisplayName = "Recipient", Enabled = false)]
        [PXSelector(typeof(Search<Contact.userID, Where<Contact.userID, IsNotNull,
                                And<Contact.isActive, Equal<True>,
                                And<Contact.contactType, Equal<ContactTypesAttribute.employee>>>>>),
                    SubstituteKey = typeof(Contact.displayName))]
        public virtual Guid? UserID { get; set; }
        #endregion

        #region CollectedDate
        public abstract class collectedDate : PX.Data.BQL.BqlDateTime.Field<collectedDate> { }
        /// <summary>
        /// Specifies the date that the Survey was collected
        /// </summary>
        [PXDBDate(InputMask = "g", DisplayMask = "g", PreserveTime = true)]
        [PXUIField(DisplayName = "Collected Date", Enabled = false)]
        public virtual DateTime? CollectedDate { get; set; }
        #endregion

        #region ExpirationDate
        public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
        /// <summary>
        /// Specifies the date that this Collector expires. The user has up until this date to finish the survey.
        /// </summary>        
        [PXDBDate()]
        [PXUIField(DisplayName = "Expiration Date", Enabled = false)]
        public virtual DateTime? ExpirationDate { get; set; }
        #endregion

        #region CollectorStatus
        public abstract class collectorStatus : PX.Data.BQL.BqlString.Field<collectorStatus> { }
        /// <summary>
        /// Reference to the state the collector record is in   
        /// </summary>
        [PXDBString(1, IsUnicode = false, IsFixed = true)]
        [PXDefault(SurveyResponseStatus.CollectorNew)]
        [PXUIField(DisplayName = "Collector Status", Enabled = false)]
        [SurveyResponseStatus.List]
        public virtual string CollectorStatus { get; set; }

        #endregion
        #region NoteID
        public abstract class noteID : PX.Data.IBqlField { }

        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion
        #region Attributes
        public abstract class attributes : BqlAttributes.Field<attributes> { }

        /// <summary>
        /// Attributes
        /// </summary>
        [CRAttributesField(typeof(surveyID), typeof(noteID))]
        public virtual string[] Attributes { get; set; }

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
        [PXDBCreatedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Created Date Time")]
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
        [PXDBLastModifiedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region tstamp        
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

        [PXDBTimestamp]
        public virtual Byte[] tstamp { get; set; }
        #endregion
    }
}