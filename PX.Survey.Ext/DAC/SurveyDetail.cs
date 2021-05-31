﻿using PX.Data;
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
            public static SurveyDetail Find(PXGraph graph, int? surveyID, int? lineNbr) => FindBy(graph, surveyID, lineNbr);
        }

        public static class FK {
            public class SUSurvey : Survey.PK.ForeignKeyOf<SurveyDetail>.By<surveyID> { }
            public class SUSurveyTemplate : SurveyTemplate.PK.ForeignKeyOf<SurveyDetail>.By<templateID> { }
        }
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

        #region TemplateType
        public abstract class templateType : BqlString.Field<templateType> { }
        [PXDBString(2, IsUnicode = false, IsFixed = true)]
        [PXDefault(SUTemplateType.QuestionPage)]
        [PXUIField(DisplayName = "Template Type")]
        [SUTemplateType.DetailList]
        public virtual string TemplateType { get; set; }
        #endregion

        #region TemplateID
        public abstract class templateID : BqlInt.Field<templateID> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Template")]
        [PXDefault]
        [PXForeignReference(typeof(FK.SUSurveyTemplate))]
        [PXSelector(typeof(Search<SurveyTemplate.templateID, Where<SurveyTemplate.templateType, Equal<Current<templateType>>>>), new Type[] { typeof(SurveyTemplate.templateID), typeof(SurveyTemplate.description) },
            DescriptionField = typeof(SurveyTemplate.description),
            SubstituteKey = typeof(SurveyTemplate.description))]
        public virtual int? TemplateID { get; set; }
        #endregion

        #region IsQuestion
        public abstract class isQuestion : BqlBool.Field<isQuestion> { }
        [PXBool]
        [PXFormula(typeof(Switch<Case<Where<templateType, Equal<SUTemplateType.questionPage>>, True>, False>))]
        [PXUIField(DisplayName = "Is Question", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual bool? IsQuestion { get; set; }
        #endregion

        #region QuestionNbr
        public abstract class questionNbr : BqlInt.Field<questionNbr> { }
        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = "Question Nbr.")]
        [PXUIEnabled(typeof(Where<templateType, Equal<SUTemplateType.questionPage>>))]
        [PXUIRequired(typeof(Where<templateType, Equal<SUTemplateType.questionPage>>))]
        public virtual int? QuestionNbr { get; set; }
        #endregion

        public abstract class description : BqlString.Field<description> { }
        [DBMatrixLocalizableDescription(256, IsUnicode = true)]
        [PXFieldDescription]
        //[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Description { get; set; }

        #region AttributeID
        public abstract class attributeID : BqlString.Field<attributeID> { }
        [PXString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXFormula(typeof(Selector<templateID, SurveyTemplate.attributeID>))]
        [PXUIField(DisplayName = "Answer Meta")]
        public virtual string AttributeID { get; set; }
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