using System;
using PX.Data;
using PX.Data.BQL;
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo


namespace PX.Survey.Ext.DAC
{

    /// <summary>
    /// This entity is used to coordinate gathering and attaching Survey answers to a specific time. 
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.SurveyCollectorDataCacheName)]
    //todo: determine if we need a new graph I would assume we would use SurveyCollectorMaint
    //[PXPrimaryGraph(typeof(SurveyCollectorMaint))]
    public class SurveyCollectorData : IBqlTable
    {

        /* will use this for the short term to manually create the table.
         once we have a Customization object to auto create this table we will then
         purge this from here as we will not need the superfluous commentary.


      --Drop Table SurveyCollectorData
CREATE TABLE [dbo].[SurveyCollectorData](
	[CompanyID] [int] NOT NULL,
	--[SurveyID] [int] NOT NULL,
	[CollectorDataID] [int] IDENTITY(1,1) NOT NULL,
	--we want to allow for retrieving the collector at a later time
	[CollectorID] [int] NULL, 
	[CollectorToken] [nvarchar](255) NULL,
	[Payload] [nvarchar](MAX) NULL,
	[QueryParameters] [nvarchar](255) NULL,
	[IpAddress] [varchar] (32) Null,
	--[UserID] [uniqueidentifier] NOT NULL,
	[NoteID] [uniqueidentifier] NOT NULL,
	[CreatedByID] [uniqueidentifier] NOT NULL,
	[CreatedByScreenID] [char](8) NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[LastModifiedByID] [uniqueidentifier] NOT NULL,
	[LastModifiedByScreenID] [char](8) NOT NULL,
	[LastModifiedDateTime] [datetime] NOT NULL,
	[tstamp] [timestamp] NOT NULL,
 CONSTRAINT [SurveyCollectorData_PK] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[CollectorDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SurveyCollectorData] ADD  DEFAULT ((0)) FOR [CompanyID]
GO


ALTER TABLE [dbo].[SurveyCollectorData] alter column QueryParameters varchar(8000)

         */

        #region CollectorDataID
        public abstract class collectorDataID : BqlInt.Field<collectorDataID> { }

        /// <summary>
        /// Uniquely Identifies this Collector record.
        /// </summary>
        [PXUIField(DisplayName = "Collector ID", Visible = false)]
        [PXDBIdentity(IsKey = true, BqlField = typeof(collectorID))]
        [PXSelector(typeof(Search<SurveyCollector.collectorID>))]
        public virtual int? CollectorDataID { get; set; }
        #endregion

        #region CollectorToken
        public abstract class collectorToken : BqlInt.Field<collectorToken> { }

        /// <summary>
        /// Collector Token is a opaque bearer token used in lue of the Collector ID as to
        /// make guessing one improbable
        /// </summary>
        [PXUIField(DisplayName = "Collector Token", Enabled = false)]
        [PXDBString(255, //tokens can be up to 255 chars. we could consider lessening it 
            IsUnicode = true)]
        public virtual string CollectorToken { get; set; }
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
        [PXUIField(DisplayName = "Collector ID", Enabled = false)]
        [PXDBInt]
        //todo: determine if we need to establish a parent child relationship. 
        //      i see this more as a backend parking lot to temporarily store 
        //      data in a super easy manor so we are unlikely going to need 
        //      to have it reference on the UI. 
        public virtual int? CollectorID { get; set; }
        #endregion

        
        #region Payload
        public abstract class payload : BqlString.Field<payload> { }
        //DBDefinition: [Payload] [nvarchar] (MAX) NULL,
        /// <summary>
        /// This will hold the content from the html survey submission 
        /// </summary>
        [PXUIField(DisplayName = "Payload", Enabled = false)]
        [PXDBString(8000, //todo: Find out if we can use Mas and the ramification of doing so. DB is currently defined as MAX
            IsUnicode = true)]
        public virtual string Payload { get; set; }
        #endregion



        #region QueryParameters
        public abstract class queryParameters : BqlString.Field<queryParameters> { }
        //DBDefinition: //[QueryParameters] [nvarchar] (255) NULL,
        //todo: we might not technically need this but going to have it here
        //      in-case we want to pass flags along as to invoke dynamic behavior.
        //      it could prove as a mechanism to turn stuff on or off during processing.
        /// <summary>
        /// This will hold the query parameter string
        /// </summary>
        [PXUIField(DisplayName = "queryParameters", Enabled = false)]
        [PXDBString(8000, IsUnicode = true)]
        public virtual string QueryParameters { get; set; }
        #endregion

        //[IpAddress] [varchar] (32) Null,

        #region NoteID
        public abstract class noteID : IBqlField { }

        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion
        
        #region CreatedByID
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        #endregion
        #region CreatedByScreenID
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        #endregion

        #region CreatedDateTime
        //todo: this field should satisfy the Date requirements for building the actual time the survey was answered.
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        [PXDBCreatedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Created Date Time")]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion
        
        //todo: doubt these will ever be modified. Ask team what they think
        #region LastModifiedByID
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        [PXDBLastModifiedByScreenID()]
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