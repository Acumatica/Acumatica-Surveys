using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.IN.Matrix.Attributes;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXCacheName("SurveyDetail")]
    public partial class SurveyDetail : IBqlTable, ISortOrder, INotable {

        #region Keys
        public class PK : PrimaryKeyOf<SurveyDetail>.By<surveyID, lineNbr> {
            public static SurveyDetail Find(PXGraph graph, string surveyID, int? lineNbr) => FindBy(graph, surveyID, lineNbr);
        }

        public static class FK {
            public class SUSurvey : Survey.PK.ForeignKeyOf<SurveyDetail>.By<surveyID> { }
            public class SUSurveyTemplate : SurveyComponent.PK.ForeignKeyOf<SurveyDetail>.By<componentID> { }
        }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlString.Field<surveyID> { }
        [SurveyID(IsKey = true)]
        [PXDBDefault(typeof(Survey.surveyID))]
        [PXParent(typeof(Select<Survey, Where<Survey.surveyID, Equal<Current<surveyID>>>>))]
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

        #region Active
        public abstract class active : BqlBool.Field<active> { }
        [PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        #endregion

        #region PageNbr
        public abstract class pageNbr : BqlInt.Field<pageNbr> { }
        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = "Page Nbr.")]
        public virtual int? PageNbr { get; set; }
        #endregion

        #region ComponentType
        public abstract class componentType : BqlString.Field<componentType> { }
        [PXDBString(2, IsUnicode = false, IsFixed = true)]
        [PXDefault(SUComponentType.QuestionPage)]
        [PXUIField(DisplayName = "Component Type")]
        [SUComponentType.DetailList]
        public virtual string ComponentType { get; set; }
        #endregion

        #region TemplateID
        public abstract class componentID : BqlInt.Field<componentID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Component")]
        [PXDefault]
        [PXForeignReference(typeof(FK.SUSurveyTemplate))]
        [PXSelector(typeof(Search<SurveyComponent.componentID, Where<SurveyComponent.componentType, Equal<Current<componentType>>>>), new Type[] { typeof(SurveyComponent.componentID), typeof(SurveyComponent.description) },
            DescriptionField = typeof(SurveyComponent.description),
            SubstituteKey = typeof(SurveyComponent.description))]
        public virtual int? ComponentID { get; set; }
        #endregion

        #region IsQuestion
        public abstract class isQuestion : BqlBool.Field<isQuestion> { }
        [PXBool]
        [PXFormula(typeof(Switch<Case<Where<componentType, Equal<SUComponentType.questionPage>>, True>, False>))]
        [PXUIField(DisplayName = "Is Question", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
        public virtual bool? IsQuestion { get; set; }
        #endregion

        #region IsComment
        public abstract class isComment : BqlBool.Field<isComment> { }
        [PXBool]
        [PXFormula(typeof(Switch<Case<Where<componentType, Equal<SUComponentType.commentPage>>, True>, False>))]
        [PXUIField(DisplayName = "Is Comment", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
        public virtual bool? IsComment { get; set; }
        #endregion

        #region QuestionNbr
        public abstract class questionNbr : BqlInt.Field<questionNbr> { }
        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = "Question Nbr.")]
        [PXUIEnabled(typeof(Where<isQuestion, Equal<True>, Or<isComment, Equal<True>>>))]
        [PXUIRequired(typeof(Where<isQuestion, Equal<True>, Or<isComment, Equal<True>>>))]
        public virtual int? QuestionNbr { get; set; }
        #endregion

        #region Description
        public abstract class description : BqlString.Field<description> { }
        [DBMatrixLocalizableDescription(256, IsUnicode = true)]
        [PXFieldDescription]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Description { get; set; }
        #endregion

        #region AttributeID
        public abstract class attributeID : BqlString.Field<attributeID> { }
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(CS.CSAttribute.attributeID), DescriptionField = typeof(CS.CSAttribute.description))]
        [PXUIField(DisplayName = "Answer Type")]
        [PXUIEnabled(typeof(Where<isQuestion, Equal<True>, Or<isComment, Equal<True>>>))]
        [PXUIRequired(typeof(Where<isQuestion, Equal<True>, Or<isComment, Equal<True>>>))]
        public virtual string AttributeID { get; set; }
        #endregion

        #region ControlType
        public abstract class controlType : BqlInt.Field<controlType> { }
        [PXInt]
        [SUControlType.List]
        [PXUIField(DisplayName = "Control Type", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
        [PXFormula(typeof(Selector<attributeID, CS.CSAttribute.controlType>))]
        public virtual int? ControlType { get; set; }
        #endregion

        #region Required
        public abstract class required : BqlBool.Field<required> { }
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Required", Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIEnabled(typeof(Where<isQuestion, Equal<True>, Or<isComment, Equal<True>>>))]
        [PXUIRequired(typeof(Where<isQuestion, Equal<True>>))]
        public virtual bool? Required { get; set; }
        #endregion

        #region ReverseOrder
        public abstract class reverseOrder : BqlBool.Field<reverseOrder> { }
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Reverse", Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIEnabled(typeof(Where<controlType, In3<SUControlType.combo, SUControlType.multi, SUControlType.selector>>))]
        public virtual bool? ReverseOrder { get; set; }
        #endregion

        #region NbrOfRows
        public abstract class nbrOfRows : BqlInt.Field<nbrOfRows> { }
        [PXDBInt]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Nbr. Of Rows")]
        [PXUIEnabled(typeof(Where<controlType, Equal<SUControlType.text>>))]
        [PXUIRequired(typeof(Where<controlType, Equal<SUControlType.text>>))]
        public virtual int? NbrOfRows { get; set; }
        #endregion

        #region MaxLength
        public abstract class maxLength : BqlInt.Field<maxLength> { }
        [PXDBInt]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Max Length")]
        [PXUIEnabled(typeof(Where<controlType, Equal<SUControlType.text>>))]
        [PXUIRequired(typeof(Where<controlType, Equal<SUControlType.text>>))]
        public virtual int? MaxLength { get; set; }
        #endregion

        #region AttrDesc
        public abstract class attrDesc : BqlString.Field<attrDesc> { }
        [PXString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Control Description", IsReadOnly = true)]
        [PXFormula(typeof(Selector<attributeID, CS.CSAttribute.description>))]
        public virtual string AttrDesc { get; set; }
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
