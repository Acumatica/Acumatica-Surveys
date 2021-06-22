using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using System;

namespace PX.Survey.Ext {

    /// <summary>
    /// This entity is used to coordinate gathering and attaching Survey answers to a specific time. 
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.CacheNames.SurveyCollectorData)]
    [PXPrimaryGraph(typeof(SurveyCollectorMaint))]
    public class SurveyCollectorData : IBqlTable, INotable {

        #region Keys
        public class PK : PrimaryKeyOf<SurveyCollectorData>.By<collectorDataID> {
            public static SurveyCollectorData Find(PXGraph graph, int? collectorDataID) => FindBy(graph, collectorDataID);
        }

        public static class FK {
            public class SUSurvey : Survey.PK.ForeignKeyOf<SurveyCollectorData>.By<surveyID> { }
            public class SUCollectorToken : SurveyCollector.UK.ForeignKeyOf<SurveyCollectorData>.By<token> { }
        }


        #endregion

        #region CollectorDataID
        public abstract class collectorDataID : BqlInt.Field<collectorDataID> { }
        /// <summary>
        /// Uniquely Identifies this Collector record.
        /// </summary>
        [PXDBIdentity(IsKey = true)]
        public virtual int? CollectorDataID { get; set; }
        #endregion

        #region Token
        public abstract class token : BqlInt.Field<token> { }
        /// <summary>
        /// Collector Token is a opaque bearer token used in lue of the Collector ID as to
        /// make guessing one improbable
        /// </summary>
        [PXUIField(DisplayName = "Token", IsReadOnly = true)]
        [PXDBString(255, IsUnicode = true)]//tokens can be up to 255 chars. we could consider lessening it 
        //[PXParent(typeof(FK.SUCollectorToken))] // Crashes
        public virtual string Token { get; set; }
        #endregion

        #region CollectorID
        public abstract class collectorID : BqlInt.Field<collectorID> { }
        /// <summary>
        /// Identifies the collector that this Data record belongs to.
        /// </summary>
        /// <remarks>
        /// This may have a null value upfront with a processing page that populates the value
        /// once the processing page kicks in.
        /// </remarks>
        [PXUIField(DisplayName = "Collector ID", IsReadOnly = true)]
        [PXDBInt]
        public virtual int? CollectorID { get; set; }
        #endregion

        #region Uri
        public abstract class uri : BqlString.Field<uri> { }
        [PXUIField(DisplayName = "URI", IsReadOnly = true)]
        [PXDBText(IsUnicode = true)]
        public virtual string Uri { get; set; }
        #endregion

        #region Payload
        public abstract class payload : BqlString.Field<payload> { }
        /// <summary>
        /// This will hold the content from the html form submission 
        /// </summary>
        [PXUIField(DisplayName = "Payload", IsReadOnly = true)]
        [PXDBText(IsUnicode = true)]
        public virtual string Payload { get; set; }
        #endregion

        #region QueryParameters
        public abstract class queryParameters : BqlString.Field<queryParameters> { }
        /// <summary>
        /// This will hold the query parameter string
        /// </summary>
        [PXUIField(DisplayName = "Query Parameters", IsReadOnly = true)]
        [PXDBText(IsUnicode = true)]
        public virtual string QueryParameters { get; set; }
        #endregion

        #region PageNbr
        public abstract class pageNbr : BqlInt.Field<pageNbr> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Page Nbr.")]
        public virtual int? PageNbr { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlString.Field<surveyID> { }
        [SurveyID]
        [PXForeignReference(typeof(FK.SUSurvey))]
        [PXSelector(typeof(Search<Survey.surveyID>), DescriptionField = typeof(Survey.title))]
        public virtual string SurveyID { get; set; }
        #endregion

        #region Status
        public abstract class status : BqlString.Field<status> { }
        /// <summary>
        /// Reference to the state the collector record is in   
        /// </summary>
        [PXDBString(1, IsUnicode = false, IsFixed = true)]
        [PXDefault(CollectorDataStatus.New)]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [CollectorDataStatus.List]
        public virtual string Status { get; set; }
        #endregion

        #region Message
        public abstract class message : BqlString.Field<message> { }
        [PXDBText]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Processing Message", Enabled = false)]
        public virtual string Message { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : IBqlField { }

        [PXNote]
        public virtual Guid? NoteID { get; set; }
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
        //todo: this field should satisfy the Date requirements for building the actual time the survey was answered.
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
        public virtual byte[] tstamp { get; set; }
        #endregion
    }
}