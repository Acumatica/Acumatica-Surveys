using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXCacheName(Messages.CacheNames.SurveyUser)]
    public class SurveyUser : IBqlTable, INotable {

        #region Selected
        public abstract class selected : BqlBool.Field<selected> { }

        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlInt.Field<surveyID> { }

        [PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(Survey.surveyID))]
        [PXParent(typeof(Select<Survey, Where<Survey.surveyID, Equal<Current<surveyID>>>>))]
        public virtual int? SurveyID { get; set; }
        #endregion

        #region LineNbr
        public abstract class lineNbr : BqlInt.Field<lineNbr> { }

        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(Survey.lineCntr))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual Int32? LineNbr { get; set; }
        #endregion

        #region ContactID
        public abstract class contactID : BqlInt.Field<contactID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Recipient Name")]
        [PXSelector(typeof(Search<Contact.contactID,
                                    Where<Contact.contactType, Equal<ContactTypesAttribute.employee>,
                                           And<Contact.isActive, Equal<True>, And<Contact.userID, IsNotNull>>>>),
                    DescriptionField = typeof(Contact.displayName))]
        [PXCheckUnique(Where = typeof(Where<SurveyUser.surveyID, Equal<Current<SurveyUser.surveyID>>>),
                       ClearOnDuplicate = false)]
        public virtual int? ContactID { get; set; }
        #endregion

        #region Active
        public abstract class active : BqlBool.Field<active> { }

        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        #endregion

        #region RecipientType
        public abstract class recipientType : BqlString.Field<recipientType> { }

        [PXString(2, IsFixed = true)]
        [ContactTypes]
        [PXFormula(typeof(Selector<contactID, Contact.contactType>))]
        [PXUIField(DisplayName = "Recipient Type", Enabled = false)]
        public virtual string RecipientType { get; set; }
        #endregion

        #region UserID
        public abstract class userID : BqlGuid.Field<userID> { }

        [PXGuid]
        [PXFormula(typeof(Selector<contactID, Contact.userID>))]
        public virtual Guid? UserID { get; set; }
        #endregion

        #region RecipientFirstName
        public abstract class recipientFirstName : BqlString.Field<recipientFirstName> { }

        [PXString(IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.firstName>))]
        [PXUIField(DisplayName = "First Name", Enabled = false)]
        public virtual string RecipientFirstName { get; set; }
        #endregion

        #region RecipientLastName
        public abstract class recipientLastName : BqlString.Field<recipientLastName> { }

        [PXString(IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.lastName>))]
        [PXUIField(DisplayName = "Last Name", Enabled = false)]
        public virtual string RecipientLastName { get; set; }
        #endregion

        #region RecipientPhone
        public abstract class recipientPhone : BqlString.Field<recipientPhone> { }

        [PXString(50, IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.phone1>))]
        [PXUIField(DisplayName = "Phone", Enabled = false)]
        public virtual string RecipientPhone { get; set; }
        #endregion

        #region RecipientEmail
        public abstract class recipientEmail : BqlString.Field<recipientEmail> { }

        [PXString(255, IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.eMail>))]
        [PXUIField(DisplayName = "Email", Enabled = false)]
        public virtual string RecipientEmail { get; set; }
        #endregion

        #region MobileAppDeviceOS
        public abstract class mobileAppDeviceOS : BqlString.Field<mobileAppDeviceOS> { }

        [PXString]
        [PXDependsOnFields(typeof(SurveyUser.contactID), typeof(SurveyUser.userID))]
        [PXFormula(typeof(MobileAppDeviceOS<SurveyUser.userID>))]
        [PXUIField(DisplayName = "Mobile App Device OS", Enabled = false)]
        public virtual string MobileAppDeviceOS { get; set; }
        #endregion

        #region UsingMobileApp
        public abstract class usingMobileApp : BqlBool.Field<usingMobileApp> { }

        [PXBool]
        [PXDependsOnFields(typeof(SurveyUser.mobileAppDeviceOS))]
        [PXFormula(typeof(IIf<Where<SurveyUser.mobileAppDeviceOS, IsNull>, False, True>))]
        [PXUIField(DisplayName = "Mobile App Notifications", Enabled = false)]
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
        [PXDBLastModifiedByScreenID()]
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