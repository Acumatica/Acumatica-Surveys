using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using System;
using System.Diagnostics;

namespace PX.Survey.Ext {

    [DebuggerDisplay("SurveyUser: SurveyID = {SurveyID}, DisplayName = {DisplayName}, ContactID = {ContactID}")]
    [Serializable]
    [PXCacheName(Messages.CacheNames.SurveyUser)]
    public class SurveyUser : IBqlTable, INotable {

        #region Keys
        public class PK : PrimaryKeyOf<SurveyUser>.By<surveyID, lineNbr> {
            public static SurveyUser Find(PXGraph graph, string surveyID, int? lineNbr) => FindBy(graph, surveyID, lineNbr);
        }
        public class UK : PrimaryKeyOf<SurveyUser>.By<surveyID, contactID> {
            public static SurveyUser Find(PXGraph graph, string surveyID, int? contactID) => FindBy(graph, surveyID, contactID);
        }
        public static class FK {
            public class SUSurvey : Survey.PK.ForeignKeyOf<SurveyDetail>.By<surveyID> { }
            public class SUContact : Contact.PK.ForeignKeyOf<SurveyDetail>.By<contactID> { }
        }
        #endregion

        #region Selected
        public abstract class selected : BqlBool.Field<selected> { }
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlString.Field<surveyID> { }
        [SurveyID(IsKey = true)]
        [PXDBDefault(typeof(Survey.surveyID))]
        [PXParent(typeof(FK.SUSurvey))]
        [PXSelector(typeof(Search<Survey.surveyID>), DescriptionField = typeof(Survey.title))]
        public virtual string SurveyID { get; set; }
        #endregion

        #region LineNbr
        public abstract class lineNbr : BqlInt.Field<lineNbr> { }
        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(Survey))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual int? LineNbr { get; set; }
        #endregion

        #region ContactID
        public abstract class contactID : BqlInt.Field<contactID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Contact")]
        [PXRestrictor(typeof(Where<Contact.isActive, Equal<True>>), "Contact '{0}' is inactive or closed.", new Type[] { typeof(Contact.displayName) })]
        [PXSelector(typeof(Search2<Contact.contactID, LeftJoin<BAccount, On<BAccount.bAccountID, Equal<Contact.bAccountID>>>>),
            DescriptionField = typeof(Contact.displayName), Filterable = true)]
        [PXCheckUnique(Where = typeof(Where<SurveyUser.surveyID, Equal<Current<surveyID>>>), ClearOnDuplicate = false)]
        [PXForeignReference(typeof(FK.SUContact))]
        public virtual int? ContactID { get; set; }
        #endregion

        #region Anonymous
        public abstract class anonymous : BqlBool.Field<anonymous> { }
        [PXDBBool]
        [PXUIField(DisplayName = "Anonymous", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual bool? Anonymous { get; set; }
        #endregion

        #region RecipientType
        public abstract class recipientType : BqlString.Field<recipientType> { }
        [PXString(2, IsFixed = true)]
        [ContactTypes]
        [PXFormula(typeof(Selector<contactID, Contact.contactType>))]
        [PXUIField(DisplayName = "Recipient Type", Enabled = false)]
        public virtual string RecipientType { get; set; }
        #endregion

        #region Active
        public abstract class active : BqlBool.Field<active> { }
        [PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        #endregion

        #region UserID
        public abstract class userID : BqlGuid.Field<userID> { }
        [PXGuid]
        [PXFormula(typeof(Selector<contactID, Contact.userID>))]
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
        [PXUIField(DisplayName = "Full Name", Enabled = false)]
        public virtual string DisplayName { get; set; }
        #endregion

        #region Phone1
        public abstract class phone1 : BqlString.Field<phone1> { }
        [PXString(50, IsUnicode = true)]
        [PXPersonalDataField]
        [PXFormula(typeof(Selector<contactID, Contact.phone1>))]
        [PXUIField(DisplayName = "Phone 1", Enabled = false)]
        public virtual string Phone1 { get; set; }
        #endregion

        #region Phone1Type
        public abstract class phone1Type : BqlString.Field<phone1Type> { }
        [PXString(3)]
        [PhoneTypes]
        [PXFormula(typeof(Selector<contactID, Contact.phone1Type>))]
        [PXUIField(DisplayName = "Phone 1 Type", Enabled = false)]
        public virtual string Phone1Type { get; set; }
        #endregion

        #region Phone2
        public abstract class phone2 : BqlString.Field<phone2> { }
        [PXString(50, IsUnicode = true)]
        [PXPersonalDataField]
        [PXFormula(typeof(Selector<contactID, Contact.phone2>))]
        [PXUIField(DisplayName = "Phone 2", Enabled = false)]
        public virtual string Phone2 { get; set; }
        #endregion

        #region Phone2Type
        public abstract class phone2Type : BqlString.Field<phone2Type> { }
        [PXString(3)]
        [PhoneTypes]
        [PXFormula(typeof(Selector<contactID, Contact.phone2Type>))]
        [PXUIField(DisplayName = "Phone 2 Type", Enabled = false)]
        public virtual string Phone2Type { get; set; }
        #endregion

        #region Email
        public abstract class email : BqlString.Field<email> { }
        [PXString(255, IsUnicode = true)]
        [PXPersonalDataField]
        [PXFormula(typeof(Selector<contactID, Contact.eMail>))]
        [PXUIField(DisplayName = "Email", Enabled = false)]
        public virtual string Email { get; set; }
        #endregion

        #region MobileDeviceOS
        public abstract class mobileDeviceOS : BqlString.Field<mobileDeviceOS> { }
        [PXString]
        [PXDependsOnFields(typeof(contactID), typeof(userID))]
        [PXFormula(typeof(MobileDeviceOS<userID>))]
        [PXUIField(DisplayName = "Mobile Device OS", Enabled = false)]
        public virtual string MobileDeviceOS { get; set; }
        #endregion

        #region UsingMobileApp
        public abstract class usingMobileApp : BqlBool.Field<usingMobileApp> { }
        [PXBool]
        [PXDependsOnFields(typeof(mobileDeviceOS))]
        [PXFormula(typeof(IIf<Where<mobileDeviceOS, IsNull>, False, True>))]
        [PXUIField(DisplayName = "Using Mobile", Enabled = false)]
        public virtual bool? UsingMobileApp { get; set; }
        #endregion

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
        [PXDBCreatedDateTime]
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
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region Tstamp
        public abstract class tstamp : BqlByteArray.Field<tstamp> { }
        [PXDBTimestamp]
        public virtual byte[] Tstamp { get; set; }
        #endregion
    }
}