using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using PX.Objects.CS;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXCacheName("SurveyAnswer")]
    public partial class SurveyAnswer : IBqlTable, ISortOrder, INotable {

        #region Keys
        public class PK : PrimaryKeyOf<SurveyAnswer>.By<surveyID, lineNbr> {
            public static SurveyAnswer Find(PXGraph graph, string surveyID, int? lineNbr) => FindBy(graph, surveyID, lineNbr);
        }

        public static class FK {
            public class SUSurvey : Survey.PK.ForeignKeyOf<SurveyAnswer>.By<surveyID> { }
            public class SUSurveyDetail : SurveyDetail.PK.ForeignKeyOf<SurveyAnswer>.By<surveyID, detailLineNbr> { }
        }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlString.Field<surveyID> { }
        [SurveyID(IsKey = true)]
        [PXDBDefault(typeof(Survey.surveyID))]
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

        #region SortOrder
        public abstract class sortOrder : BqlInt.Field<sortOrder> { }
        [PXUIField(DisplayName = "Line Order", Visible = false, Enabled = false)]
        [PXDBInt]
        public virtual int? SortOrder { get; set; }
        #endregion

        #region DetailLineNbr
        public abstract class detailLineNbr : BqlInt.Field<detailLineNbr> { }
        [PXDBInt]
        [PXParent(typeof(Select<SurveyDetail, 
            Where<SurveyDetail.surveyID, Equal<Current<surveyID>>,
            And<SurveyDetail.lineNbr, Equal<Current<detailLineNbr>>>>>))]
        [PXSelector(typeof(Search<SurveyDetail.lineNbr, Where<SurveyDetail.surveyID, Equal<Current<surveyID>>>>))]
        [PXUIField(DisplayName = "Detail Line Nbr.", Visible = false)]
        public virtual int? DetailLineNbr { get; set; }
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
        [PXDBInt]
        [PXUIField(DisplayName = "Collector ID", IsReadOnly = true)]
        [PXSelector(typeof(Search<SurveyCollector.collectorID, Where<SurveyCollector.surveyID, Equal<Current<surveyID>>>>))]
        public virtual int? CollectorID { get; set; }
        #endregion

        #region AttributeID
        public abstract class attributeID : BqlString.Field<attributeID> { }
        [PXString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXSelector(typeof(CS.CSAttribute.attributeID), DescriptionField = typeof(CS.CSAttribute.description))]
        [PXFormula(typeof(Selector<detailLineNbr, SurveyDetail.attributeID>))]
        [PXUIField(DisplayName = "Answer Type", IsReadOnly = true)]
        public virtual string AttributeID { get; set; }
        #endregion

        #region Value
        public abstract class value : BqlString.Field<value> { }
        [SUAttributeValue]
        [PXUIField(DisplayName = "Value", IsReadOnly = true)]
        [CSAttributeValueValidation(typeof(attributeID))]
        [PXPersonalDataFieldAttribute.Value]
        public virtual string Value { get; set; }
        #endregion

        #region Description
        public abstract class description : BqlString.Field<description> { }
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
        [PXFormula(typeof(Selector<detailLineNbr, SurveyDetail.description>))]
        public virtual string Description { get; set; }
        #endregion

        #region PageNbr
        public abstract class pageNbr : BqlInt.Field<pageNbr> { }
        [PXInt]
        [PXUIField(DisplayName = "Page Nbr.", IsReadOnly = true)]
        [PXFormula(typeof(Selector<detailLineNbr, SurveyDetail.pageNbr>))]
        public virtual int? PageNbr { get; set; }
        #endregion

        #region QuestionNbr
        public abstract class questionNbr : BqlInt.Field<questionNbr> { }
        [PXInt]
        [PXUIField(DisplayName = "Question Nbr.", IsReadOnly = true)]
        [PXFormula(typeof(Selector<detailLineNbr, SurveyDetail.questionNbr>))]
        public virtual int? QuestionNbr { get; set; }
        #endregion

        #region TemplateType
        public abstract class templateType : BqlString.Field<templateType> { }
        [PXString(2, IsUnicode = false, IsFixed = true)]
        [PXUIField(DisplayName = "Template Type", IsReadOnly = true)]
        [PXFormula(typeof(Selector<detailLineNbr, SurveyDetail.templateType>))]
        [SUTemplateType.DetailList]
        public virtual string TemplateType { get; set; }
        #endregion

        #region IsQuestion
        public abstract class isQuestion : BqlBool.Field<isQuestion> { }
        [PXBool]
        [PXFormula(typeof(Switch<Case<Where<templateType, Equal<SUTemplateType.questionPage>>, True>, False>))]
        [PXUIField(DisplayName = "Is Question", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
        public virtual bool? IsQuestion { get; set; }
        #endregion

        #region IsComment
        public abstract class isComment : BqlBool.Field<isComment> { }
        [PXBool]
        [PXFormula(typeof(Switch<Case<Where<templateType, Equal<SUTemplateType.commentPage>>, True>, False>))]
        [PXUIField(DisplayName = "Is Comment", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
        public virtual bool? IsComment { get; set; }
        #endregion

        #region ControlType
        public abstract class controlType : BqlInt.Field<controlType> { }
        [PXInt]
        [SUControlType.List]
        [PXUIField(DisplayName = "Control Type", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
        [PXFormula(typeof(Selector<attributeID, CS.CSAttribute.controlType>))]
        public virtual int? ControlType { get; set; }
        #endregion

        #region AttrDesc
        public abstract class attrDesc : BqlString.Field<attrDesc> { }
        [PXString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Control Description", IsReadOnly = true)]
        [PXFormula(typeof(Selector<attributeID, CS.CSAttribute.description>))]
        public virtual string AttrDesc { get; set; }
        #endregion

        #region ContactID
        public abstract class contactID : BqlInt.Field<contactID> { }
        [PXInt]
        [PXUIField(DisplayName = "Contact", IsReadOnly = true)]
        [PXFormula(typeof(Selector<collectorID, Selector<SurveyCollector.userLineNbr, SurveyUser.contactID>>))]
        public virtual int? ContactID { get; set; }
        #endregion

        #region UserID
        public abstract class userID : BqlGuid.Field<userID> { }
        [PXGuid]
        [PXFormula(typeof(Selector<contactID, Contact.userID>))]
        [PXUIField(DisplayName = "User ID", IsReadOnly = true)]
        public virtual Guid? UserID { get; set; }
        #endregion

        #region FirstName
        public abstract class firstName : BqlString.Field<firstName> { }
        [PXString(IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.firstName>))]
        [PXUIField(DisplayName = "First Name", IsReadOnly = true)]
        public virtual string FirstName { get; set; }
        #endregion

        #region LastName
        public abstract class lastName : BqlString.Field<lastName> { }
        [PXString(IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.lastName>))]
        [PXUIField(DisplayName = "Last Name", IsReadOnly = true)]
        public virtual string LastName { get; set; }
        #endregion

        #region DisplayName
        public abstract class displayName : BqlString.Field<displayName> { }
        [PXString(IsUnicode = true)]
        [PXFormula(typeof(Selector<contactID, Contact.displayName>))]
        [PXUIField(DisplayName = "Display Name", IsReadOnly = true)]
        public virtual string DisplayName { get; set; }
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
