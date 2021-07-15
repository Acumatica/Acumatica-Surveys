//using PX.CS;
//using PX.Data;
//using PX.Data.BQL;
//using System;

//namespace PX.Survey.Ext {

//    [PXProjection(typeof(Select5<SurveyAnswer,
//            LeftJoin<SurveyDetail,
//                On<SurveyDetail.surveyID, Equal<SurveyAnswer.surveyID>,
//                And<SurveyDetail.lineNbr, Equal<SurveyAnswer.detailLineNbr>>>,
//            LeftJoin<CSAttribute,
//                On<CSAttribute.attributeID, Equal<SurveyDetail.attributeID>>>>,
//            Where<SurveyDetail.componentType, Equal<SUComponentType.questionPage>>,
//            Aggregate<
//                GroupBy<SurveyDetail.pageNbr,
//                GroupBy<SurveyDetail.questionNbr,
//                GroupBy<SurveyAnswer.value,
//                Count>>>>>))]
//    [Serializable]
//    [PXCacheName("SurveyAnswerSummary")]
//    public class SurveyAnswerSummary : IBqlTable {

//        #region SurveyID
//        public abstract class surveyID : BqlString.Field<surveyID> { }
//        [PXDBString(BqlField = typeof(SurveyDetail.surveyID), IsKey = true)]
//        [PXUIField(DisplayName = "Survey ID")]
//        public virtual string SurveyID { get; set; }
//        #endregion

//        #region PageNbr
//        public abstract class pageNbr : BqlInt.Field<pageNbr> { }
//        [PXDBInt(BqlField = typeof(SurveyDetail.pageNbr), IsKey = true)]
//        [PXUIField(DisplayName = "Page Nbr.")]
//        public virtual int? PageNbr { get; set; }
//        #endregion

//        #region QuestionNbr
//        public abstract class questionNbr : BqlInt.Field<questionNbr> { }
//        [PXDBInt(BqlField = typeof(SurveyDetail.questionNbr), IsKey = true)]
//        [PXUIField(DisplayName = "Question Nbr.")]
//        public virtual int? QuestionNbr { get; set; }
//        #endregion

//        #region Description
//        public abstract class description : BqlString.Field<description> { }
//        [PXDBString(BqlField = typeof(SurveyDetail.description))]
//        [PXUIField(DisplayName = "Description")]
//        public virtual string Description { get; set; }
//        #endregion

//        #region AttributeID
//        public abstract class attributeID : BqlString.Field<attributeID> { }
//        [PXDBString(BqlField = typeof(SurveyDetail.attributeID))]
//        [PXUIField(DisplayName = "Answer Type")]
//        public virtual string AttributeID { get; set; }
//        #endregion

//        #region AttrDesc
//        public abstract class attrDesc : BqlString.Field<attrDesc> { }
//        [PXDBString(BqlField = typeof(CSAttribute.description))]
//        [PXUIField(DisplayName = "Control Description")]
//        public virtual string AttrDesc { get; set; }
//        #endregion

//        #region Value
//        public abstract class value : BqlString.Field<value> { }
//        [SUAttributeValue(BqlField = typeof(SurveyAnswer.value))]
//        //[PXDBString(BqlField = typeof(SurveyAnswer.value))]
//        [PXUIField(DisplayName = "Value")]
//        public virtual string Value { get; set; }
//        #endregion

//        #region RowCount
//        public abstract class rowCount : BqlInt.Field<rowCount> { }
//        [PXDBInt]
//        [PXUIField(DisplayName = "Count")]
//        public virtual int? RowCount { get; set; }
//        #endregion

//    }
//}
