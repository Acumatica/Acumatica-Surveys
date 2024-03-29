﻿using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using System;
using System.Diagnostics;

namespace PX.Survey.Ext {

    /// <summary>
    /// This entity is used to coordinate gathering and attaching Survey answers to a specific time.
    /// </summary>
    [DebuggerDisplay("SurveyCollector: CollectorID = {CollectorID}, Token = {Token}, DisplayName = {DisplayName}, AnonCollectorID = {AnonCollectorID}")]
    [Serializable]
    [PXCacheName(Messages.CacheNames.SurveyCollector)]
    [PXPrimaryGraph(typeof(SurveyCollectorMaint))]
    public class SurveyCollector : IBqlTable, INotable {

        #region Keys
        public class PK : PrimaryKeyOf<SurveyCollector>.By<collectorID> {
            public static SurveyCollector Find(PXGraph graph, int? collectorID) => FindBy(graph, collectorID);
        }

        public static class FK {
            public class SUSurvey : Survey.PK.ForeignKeyOf<SurveyCollector>.By<surveyID> { }
            public class SUSurveyUser : SurveyUser.PK.ForeignKeyOf<SurveyCollector>.By<surveyID, userLineNbr> { }
        }
        public static class UK {
            public class ByToken : PrimaryKeyOf<SurveyCollector>.By<token> {
                public static SurveyCollector Find(PXGraph graph, string token) => FindBy(graph, token);
            }
            public class ByAnonCollectorID : PrimaryKeyOf<SurveyCollector>.By<anonCollectorID> {
                public static SurveyCollector Find(PXGraph graph, int? anonCollectorID) => FindBy(graph, anonCollectorID);
            }
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
        [PXUIField(DisplayName = "Collector ID", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? CollectorID { get; set; }
        #endregion

        /// <summary>
        /// Identifies the specific Survey this collector record belongs too.
        /// </summary>
        #region SurveyID
        public abstract class surveyID : BqlString.Field<surveyID> { }
        [SurveyID]
        [PXDBDefault(typeof(Survey.surveyID), DefaultForUpdate = false)]
        [PXParent(typeof(FK.SUSurvey))]
        [PXSelector(typeof(Search<Survey.surveyID>), DescriptionField = typeof(Survey.title))]
        public virtual string SurveyID { get; set; }
        #endregion

        #region UserLineNbr
        public abstract class userLineNbr : BqlInt.Field<userLineNbr> { }
        [PXDBInt]
        [PXDBDefault(typeof(SurveyUser.lineNbr), DefaultForUpdate = false)]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        [PXParent(typeof(FK.SUSurveyUser))]
        [PXSelector(typeof(Search<SurveyUser.lineNbr, Where<SurveyUser.surveyID, Equal<Current<surveyID>>>>))]
        public virtual int? UserLineNbr { get; set; }
        #endregion

        #region EntityType
        public abstract class entityType : BqlString.Field<entityType> { }
        [PXString(256, IsUnicode = true)]
        [SUEntityTypeList]
        [PXUnboundDefault(typeof(Selector<surveyID, Survey.entityType>))]
        public virtual string EntityType { get; set; }
        #endregion

        #region RefNoteID
        public abstract class refNoteID : BqlGuid.Field<refNoteID> { }
        [PXDBGuid(false)]
        [PXUIField(DisplayName = "Entity Link", Visible = false)]
        public virtual Guid? RefNoteID { get; set; }
        #endregion

        /// <summary>
        /// The description of the entity whose <tt>NoteID</tt> value is specified as <see cref="P:PX.Objects.CR.CRActivity.RefNoteID" />.
        /// </summary>
        /// <value>
        /// The description is retrieved by the
        /// <see cref="M:PX.Data.EntityHelper.GetEntityDescription(System.Nullable{System.Guid},System.Type)" /> method.
        /// </value>
        public abstract class source : BqlString.Field<source> { }
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Related Entity Description", Enabled = false)]
        [PXFormula(typeof(EntityDescription<refNoteID>))]
        public virtual string Source { get; set; }

        #region AnonCollectorID
        public abstract class anonCollectorID : BqlInt.Field<anonCollectorID> { }
        /// <summary>
        /// Uniquely identifies this Anonymous Collector record for this Collector.
        /// </summary>
        [PXDBInt]
        //[PXSelector(typeof(Search<collectorID, Where<userLineNbr, Equal<Current<userLineNbr>>>>))]
        [PXUIField(DisplayName = "Anon. ID")]
        public virtual int? AnonCollectorID { get; set; }
        #endregion

        #region ContactID
        public abstract class contactID : BqlInt.Field<contactID> { }
        [PXInt]
        [PXUIField(DisplayName = "Contact ID")]
        [PXSelector(typeof(Search<Contact.contactID>), DescriptionField = typeof(Contact.displayName), ValidateValue = false)]
        [PXFormula(typeof(Selector<userLineNbr, SurveyUser.contactID>))]
        public virtual int? ContactID { get; set; }
        #endregion

        #region Anonymous
        public abstract class anonymous : BqlBool.Field<anonymous> { }
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Anonymous", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual bool? Anonymous { get; set; }
        #endregion

        #region IsTest
        public abstract class isTest : BqlBool.Field<isTest> { }
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is Test", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual bool? IsTest { get; set; }
        #endregion

        #region UserID
        public abstract class userID : BqlGuid.Field<userID> { }
        [PXGuid]
        [PXFormula(typeof(Selector<contactID, Contact.userID>))]
        [PXUIField(DisplayName = "User ID")]
        public virtual Guid? UserID { get; set; }
        #endregion

        #region FirstName
        public abstract class firstName : BqlString.Field<firstName> { }
        [PXString(IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.firstName>))]
        [PXUIField(DisplayName = "First Name", Enabled = false)]
        public virtual string FirstName { get; set; }
        #endregion

        #region LastName
        public abstract class lastName : BqlString.Field<lastName> { }
        [PXString(IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.lastName>))]
        [PXUIField(DisplayName = "Last Name", Enabled = false)]
        public virtual string LastName { get; set; }
        #endregion

        #region DisplayName
        public abstract class displayName : BqlString.Field<displayName> { }
        [PXString(IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.displayName>))]
        [PXUIField(DisplayName = "Display Name", Enabled = false)]
        public virtual string DisplayName { get; set; }
        #endregion

        //public abstract class isEncrypted : BqlBool.Field<isEncrypted> { }
        //[PXDBBool]
        //[PXDefault(false)]
        //public virtual bool? IsEncrypted { get; set; }

        //public abstract class isEncryptionRequired : BqlBool.Field<isEncryptionRequired> { }
        //[PXDBBool]
        //[PXDefault(false)]
        //public virtual bool? IsEncryptionRequired { get; set; }

        #region Token
        public abstract class token : BqlInt.Field<token> { }
        /// <summary>
        /// Collector Token is a opaque bearer token used in lieu of the Collector ID as to make guessing one improbable
        /// </summary>
        //[PXDefault(typeof(collectorID))]
        //[PXRSACryptStringWithConditional(255, typeof(isEncryptionRequired), typeof(isEncrypted))]
        //[PXRSACryptString(255, IsViewDecrypted = true)]
        [PXDBString(255, IsUnicode = true)]//tokens can be up to 255 chars. we could consider lessening it
        [PXUIField(DisplayName = "Token", IsReadOnly = true)]
        //[PXFormula(typeof(AccessInfo.businessDate))]
        [PXFormula(typeof(DateAsString<PXDateAndTimeAttribute.now, DateAsStringFormat.roundTrip>))]
        //[PXDefault]
        public virtual string Token { get;  set; }
        #endregion

        #region SentOn
        public abstract class sentOn : BqlDateTime.Field<sentOn> { }
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = "Sent On", Enabled = false)]
        public virtual DateTime? SentOn { get; set; }
        #endregion

        #region ExpirationDate
        public abstract class expirationDate : BqlDateTime.Field<expirationDate> { }
        /// <summary>
        /// Specifies the date that this Collector expires. The user has up until this date to finish the survey.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
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

        #region Message
        public abstract class message : BqlString.Field<message> { }
        [PXDBText]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Processing Message", Enabled = false)]
        public virtual string Message { get; set; }
        #endregion

        //#region BaseURL
        //public abstract class baseURL : BqlString.Field<baseURL> { }
        //[PXString(IsUnicode = true)]
        //[PXUIField(DisplayName = "Base URL")]
        //[PXFormula(typeof(Selector<surveyID, Survey.baseURL>))]
        //public virtual string BaseURL { get; set; }
        //#endregion

        #region NoteID
        public abstract class noteID : BqlGuid.Field<noteID> { }
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