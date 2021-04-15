using PX.Data;
using PX.Data.BQL.Fluent;
using System;
using PX.Objects.CR;
using PX.Data.BQL;
using PX.SM;
using PX.Data.EP;

namespace PX.Survey.Ext {

    [Serializable]
    [PXCacheName("RecipientSelected")]
    [PXProjection(typeof(Select<Contact, 
        Where<Contact.isActive, Equal<True>,
        And<Where<CurrentValue<RecipientFilter.onlyUsers>, NotEqual<True>,
            Or<Contact.userID, IsNotNull,
        And<Where<CurrentValue<RecipientFilter.contactType>, IsNull, 
            Or<Contact.contactType, Equal<CurrentValue<RecipientFilter.contactType>>>>>>>>>,
        OrderBy<Asc<Contact.displayName>>>))]
    public class RecipientSelected : IBqlTable, IPXSelectable {

        #region Selected
        public abstract class selected : BqlType<IBqlBool, bool>.Field<selected> { }
        [PXBool]
        [PXUnboundDefault]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        [PXDBInt(BqlField = typeof(Contact.contactID))]
        [PXUIField(DisplayName = "Contact ID", Visibility = PXUIVisibility.Invisible)]
        public virtual int? ContactID { get; set; }

        [ContactTypes]
        [PXDBString(2, IsFixed = true, BqlField = typeof(Contact.contactType))]
        [PXUIField(DisplayName = "Type", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string ContactType { get; set; }

        [PXDBString(50, IsUnicode = true, BqlField = typeof(Contact.firstName))]
        [PXUIField(DisplayName = "First Name")]
        public virtual string FirstName { get; set; }

        [PXDBString(100, IsUnicode = true, BqlField = typeof(Contact.lastName))]
        [PXUIField(DisplayName = "Last Name")]
        public virtual string LastName { get; set; }

        [PXDBString(100, IsUnicode = true, BqlField = typeof(Contact.displayName))]
        [PXUIField(DisplayName = "Contact", Enabled = false)]
        public virtual string DisplayName { get; set; }

        [PXDBEmail(BqlField = typeof(Contact.eMail))]
        [PXUIField(DisplayName = "Email")]
        public virtual string EMail { get; set; }

        [PXDBString(50, IsUnicode = true, BqlField = typeof(Contact.phone1))]
        [PXPhone]
        [PXUIField(DisplayName = "Phone 1")]
        public virtual string Phone1 { get; set; }

        [PhoneTypes]
        [PXDBString(3, BqlField = typeof(Contact.phone1Type))]
        [PXUIField(DisplayName = "Phone 1 Type")]
        public virtual string Phone1Type { get; set; }

        [PXDBString(50, IsUnicode = true, BqlField = typeof(Contact.phone2))]
        [PXPhone]
        [PXUIField(DisplayName = "Phone 2")]
        public virtual string Phone2 { get; set; }

        [PhoneTypes]
        [PXDBString(3, BqlField = typeof(Contact.phone2Type))]
        [PXUIField(DisplayName = "Phone 2 Type")]
        public virtual string Phone2Type { get; set; }

        [PXDBInt(BqlField = typeof(Contact.bAccountID))]
        [PXSelector(typeof(BAccount.bAccountID), SubstituteKey = typeof(BAccount.acctCD), DescriptionField = typeof(BAccount.acctName), DirtyRead = true)]
        [PXUIField(DisplayName = "Business Account")]
        public virtual int? BAccountID { get; set; }
    }
}
