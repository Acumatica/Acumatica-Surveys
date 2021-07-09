using PX.Data;
using PX.Data.BQL;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXCacheName("TemplateSelected")]
    [PXProjection(typeof(Select<SurveyComponent,
        Where<SurveyComponent.active, Equal<True>,
        And<SurveyComponent.componentType, NotEqual<SUComponentType.survey>,
        And<SurveyComponent.componentType, NotEqual<SUComponentType.badRequest>,
        And<Where<CurrentValue<TemplateFilter.templateType>, IsNull,
            Or<SurveyComponent.componentType, Equal<CurrentValue<TemplateFilter.templateType>>>>>>>>,
        OrderBy<Asc< SurveyComponent.description>>>))]
    public class TemplateSelected : IBqlTable, IPXSelectable {

        #region Selected
        public abstract class selected : BqlType<IBqlBool, bool>.Field<selected> { }
        [PXBool]
        [PXUnboundDefault]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        [ComponentID]
        public virtual string ComponentID { get; set; }

        [PXDBString(2, IsFixed = true, BqlField = typeof(SurveyComponent.componentType))]
        [SUComponentType.DetailList]
        [PXUIField(DisplayName = "Type", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string TemplateType { get; set; }

        [PXDBString(256, IsUnicode = true, BqlField = typeof(SurveyComponent.description))]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }

        //[PXDBString(50, IsUnicode = true, BqlField = typeof(Contact.firstName))]
        //[PXUIField(DisplayName = "First Name")]
        //public virtual string FirstName { get; set; }


        //[PXDBString(100, IsUnicode = true, BqlField = typeof(Contact.displayName))]
        //[PXUIField(DisplayName = "Contact", Enabled = false)]
        //public virtual string DisplayName { get; set; }

        //[PXDBEmail(BqlField = typeof(Contact.eMail))]
        //[PXUIField(DisplayName = "Email")]
        //public virtual string EMail { get; set; }

        //[PXDBString(50, IsUnicode = true, BqlField = typeof(Contact.phone1))]
        //[PXPhone]
        //[PXUIField(DisplayName = "Phone 1")]
        //public virtual string Phone1 { get; set; }

        //[PhoneTypes]
        //[PXDBString(3, BqlField = typeof(Contact.phone1Type))]
        //[PXUIField(DisplayName = "Phone 1 Type")]
        //public virtual string Phone1Type { get; set; }

        //[PXDBString(50, IsUnicode = true, BqlField = typeof(Contact.phone2))]
        //[PXPhone]
        //[PXUIField(DisplayName = "Phone 2")]
        //public virtual string Phone2 { get; set; }

        //[PhoneTypes]
        //[PXDBString(3, BqlField = typeof(Contact.phone2Type))]
        //[PXUIField(DisplayName = "Phone 2 Type")]
        //public virtual string Phone2Type { get; set; }

        //[PXDBInt(BqlField = typeof(Contact.bAccountID))]
        //[PXSelector(typeof(BAccount.bAccountID), SubstituteKey = typeof(BAccount.acctCD), DescriptionField = typeof(BAccount.acctName), DirtyRead = true)]
        //[PXUIField(DisplayName = "Business Account")]
        //public virtual int? BAccountID { get; set; }
    }
}
