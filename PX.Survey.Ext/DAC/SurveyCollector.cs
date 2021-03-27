using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using System;

namespace PX.Survey.Ext {

    /// <summary>
    /// This entity is used to coordinate gathering and attaching Survey answers to a specific time. 
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.CacheNames.SurveyCollector)]
    [PXPrimaryGraph(typeof(SurveyCollectorMaint))]
    public class SurveyCollector : IBqlTable, INotable {

        #region Keys
        public class PK : PrimaryKeyOf<SurveyCollector>.By<collectorID> {
            public static SurveyCollector Find(PXGraph graph, int? collectorID) => FindBy(graph, collectorID);
        }
        public static class FK {
            public class SUSurvey : Survey.PK.ForeignKeyOf<SurveyCollector>.By<collectorID> { }
        }
        #endregion

        #region Selected
        public abstract class selected : BqlBool.Field<selected> { }
        /// <summary>
        /// Selected
        /// </summary>
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region CollectorID
        public abstract class collectorID : BqlInt.Field<collectorID> { }
        /// <summary>
        /// Uniquely Identifies this Collector record.
        /// </summary>
        [PXDBIdentity(IsKey = true)]
        [PXSelector(typeof(Search<SurveyCollector.collectorID>))]
        public virtual int? CollectorID { get; set; }
        #endregion

        #region Name
        public abstract class name : BqlInt.Field<name> { }
        /// <summary>
        /// Name of this Collector record.
        /// </summary>
        [PXDBString(60, IsUnicode = true)]
        [PXUIField(DisplayName = "Name", Enabled = false)]
        public virtual string Name { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlInt.Field<surveyID> { }
        /// <summary>
        /// Identifies the specific Survey this collector record belongs too.
        /// </summary>
        [PXDBInt]
        [PXDBDefault(typeof(Survey.surveyID), DefaultForUpdate = false)]
        [PXParent(typeof(FK.SUSurvey))]
        [PXUIField(DisplayName = "Survey ID", Enabled = false)]
        [PXSelector(typeof(Search<Survey.surveyID>),
                    typeof(Survey.surveyCD),
                    typeof(Survey.surveyType),
                    typeof(Survey.name),
            SubstituteKey = typeof(Survey.surveyCD),
            DescriptionField = typeof(Survey.name))]
        public virtual int? SurveyID { get; set; }
        #endregion

        #region UserLineNbr
        public abstract class userLineNbr : BqlInt.Field<userLineNbr> { }
        [PXDBInt]
        [PXDBDefault(typeof(SurveyUser.lineNbr), DefaultForUpdate = false)]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual int? UserLineNbr { get; set; }
        #endregion

        #region Token
        public abstract class token : BqlInt.Field<token> { }
        /// <summary>
        /// Collector Token is a opaque bearer token used in lue of the Collector ID as to
        /// make guessing one improbable
        /// </summary>
        [PXDBString(255, IsUnicode = true)]//tokens can be up to 255 chars. we could consider lessening it 
        [PXUIField(DisplayName = "Token", IsReadOnly = true)]
        [PXDefault]
        public virtual string Token { get; set; }
        #endregion

        #region CollectedDate
        public abstract class collectedDate : BqlDateTime.Field<collectedDate> { }
        /// <summary>
        /// Specifies the date that the Survey was collected
        /// </summary>
        [PXDBDate(InputMask = "g", DisplayMask = "g", PreserveTime = true)]
        [PXUIField(DisplayName = "Collected Date", Enabled = false)]
        public virtual DateTime? CollectedDate { get; set; }
        #endregion

        #region ExpirationDate
        public abstract class expirationDate : BqlDateTime.Field<expirationDate> { }
        /// <summary>
        /// Specifies the date that this Collector expires. The user has up until this date to finish the survey.
        /// </summary>        
        [PXDBDate(InputMask = "g", DisplayMask = "g", PreserveTime = true)]
        [PXUIField(DisplayName = "Expiration Date", Enabled = false)]
        public virtual DateTime? ExpirationDate { get; set; }
        #endregion

        #region Status
        public abstract class status : BqlString.Field<status> { }
        /// <summary>
        /// Reference to the state the collector record is in   
        /// </summary>
        [PXDBString(1, IsUnicode = false, IsFixed = true)]
        [PXDefault(CollectorStatus.New)]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [CollectorStatus.List]
        public virtual string Status { get; set; }
        #endregion

        #region Rendered
        public abstract class rendered : BqlString.Field<rendered> { }
        [PXDBLocalizableString(IsUnicode = true)]
        [PXUIField(DisplayName = "Rendered", IsReadOnly = true)]
        public virtual string Rendered { get; set; }
        #endregion

        #region Message
        public abstract class message : BqlString.Field<message> { }
        [PXDBText]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Processing Message", Enabled = false)]
        public virtual string Message { get; set; }
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

        #region CollectedDatePart
        public abstract class collectedDatePart : BqlString.Field<collectedDatePart> { }
        /// <summary>
        /// Specifies the date part that the Survey was collected
        /// </summary>
        [PXString]
        [PXUIField(DisplayName = "Collected Date Mobile", Enabled = false)]
        [PXFormula(typeof(CollectedDateAsString<SurveyCollector.collectedDate>))]
        public virtual String CollectedDatePart { get; set; }
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
        [PXDBCreatedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Created Date Time")]
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
        [PXDBLastModifiedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region tstamp        
        public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }

        [PXDBTimestamp]
        public virtual Byte[] tstamp { get; set; }
        #endregion
    }
}