using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.IN.Matrix.Attributes;
using System;

namespace PX.Survey.Ext {

    [PXPrimaryGraph(typeof(SurveyTemplateMaint))]
    [Serializable]
    [PXCacheName("Survey Template")]
    public class SurveyTemplate : IBqlTable, INotable {

        public class PK : PrimaryKeyOf<SurveyTemplate>.By<templateID> {
            public static SurveyTemplate Find(PXGraph graph, int? templateID) => FindBy(graph, templateID);
            public static SurveyTemplate FindDirty(PXGraph graph, int? templateID)
                => PXSelect<SurveyTemplate, Where<templateID, Equal<Required<templateID>>>>.SelectWindowed(graph, 0, 1, templateID);
        }

        public abstract class templateID : BqlInt.Field<templateID> { }
        [PXDBIdentity(IsKey = true)]
        [PXSelector(typeof(templateID), DescriptionField = typeof(description))]
        [PXUIField(DisplayName = "Template ID")]
        public virtual int? TemplateID { get; set; }

        #region Active
        public abstract class active : BqlBool.Field<active> { }
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Active", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual bool? Active { get; set; }
        #endregion

        #region TemplateType
        public abstract class templateType : BqlString.Field<templateType> { }
        [PXDBString(2, IsUnicode = false, IsFixed = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Template Type")]
        [SUTemplateType.List]
        public virtual string TemplateType { get; set; }
        #endregion

        #region Description
        public abstract class description : BqlString.Field<description> { }
        [DBMatrixLocalizableDescription(256, IsUnicode = true)]
        [PXFieldDescription]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Description { get; set; }
        #endregion

        #region Body
        public abstract class body : BqlString.Field<body> { }
        [PXDBText(IsUnicode = true)]
        [PXUIField(DisplayName = "Body")]
        public virtual string Body { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : BqlGuid.Field<noteID> { }
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion

        #region CreatedByID
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        [PXDBCreatedByID]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedByID, Enabled = false)]
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
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false)]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion

        #region LastModifiedByID
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        [PXDBLastModifiedByID]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedByID, Enabled = false)]
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
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime, Enabled = false)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region Tstamp
        public abstract class tstamp : BqlByteArray.Field<tstamp> { }
        [PXDBTimestamp]
        public virtual byte[] Tstamp { get; set; }
        #endregion
    }
}