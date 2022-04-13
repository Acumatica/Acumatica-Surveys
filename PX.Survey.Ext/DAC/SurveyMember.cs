using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using PX.Objects.GL;
using System;

namespace PX.Survey.Ext {

    [PXCacheName("Survey Members")]
    [PXEMailSource]
    [Serializable]
    public class SurveyMember : IBqlTable {

        public class PK : PrimaryKeyOf<SurveyMember>.By<surveyID, contactID> {

            public static SurveyMember Find(PXGraph graph, string surveyID, int? contactID) {
                return FindBy(graph, surveyID, contactID);
            }
        }

        public static class FK {
            public class SUSurvey : Survey.PK.ForeignKeyOf<SurveyMember>.By<surveyID> { }
        }

        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public abstract class selected : BqlType<IBqlBool, bool>.Field<selected> { }
        public bool? Selected { get; set; }

        public abstract class surveyID : BqlString.Field<surveyID> { }
        [SurveyID(IsKey = true)]
        [PXDBDefault(typeof(Survey.surveyID))]
        [PXParent(typeof(Select<Survey, Where<Survey.surveyID, Equal<Current<surveyID>>>>))]
        [PXSelector(typeof(Search<Survey.surveyID>))]
        public virtual string SurveyID { get; set; }

        public abstract class contactID : BqlInt.Field<contactID> { }
        [PXDBInt(IsKey = true)]
        [PXDefault]
        [PXParent(typeof(Select<Contact, Where<Contact.contactID, Equal<Current<contactID>>>>))]
        [PXSelector(typeof(Search2<Contact.contactID, LeftJoin<Branch, On<Branch.bAccountID, Equal<Contact.bAccountID>, And<Contact.contactType, Equal<ContactTypesAttribute.bAccountProperty>>>, LeftJoin<BAccount, On<BAccount.bAccountID, Equal<Contact.bAccountID>>>>, Where<Branch.bAccountID, IsNull, And<Where<BAccount.bAccountID, IsNull, Or<Match<BAccount, Current<AccessInfo.userName>>>>>>>), new Type[] { typeof(Contact.fullName), typeof(Contact.displayName), typeof(Contact.eMail), typeof(Contact.phone1), typeof(Contact.bAccountID), typeof(Contact.salutation), typeof(Contact.contactType), typeof(Contact.isActive), typeof(Contact.memberName) }, DescriptionField = typeof(Contact.memberName), Filterable = true, DirtyRead = true)]
        [PXUIField(DisplayName = "Name")]
        public virtual int? ContactID { get; set; }

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
        [PXUIField(DisplayName = "Created Date Time", Enabled = false)]
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
        [PXUIField(DisplayName = "Last Modified Date Time", Enabled = false)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion
        #region tstamp
        public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        #endregion
    }
}