using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXPrimaryGraph(typeof(SurveyMaint))]
    [PXCacheName(Messages.CacheNames.Survey)]
    public class Survey : IBqlTable, INotable {

        #region Keys
        public class PK : PrimaryKeyOf<Survey>.By<surveyID> {
            public static Survey Find(PXGraph graph, int? surveyID) => FindBy(graph, surveyID);
            public static Survey FindDirty(PXGraph graph, int? surveyID)
                => (Survey)PXSelect<Survey, Where<surveyID, Equal<Required<surveyID>>>>.SelectWindowed(graph, 0, 1, surveyID);
        }
        public class UK : PrimaryKeyOf<Survey>.By<surveyCD> {
            public static Survey Find(PXGraph graph, string surveyCD) => FindBy(graph, surveyCD);
        }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlInt.Field<surveyID> { }
        [PXDBIdentity]
        public virtual int? SurveyID { get; set; }
        #endregion

        #region SurveyCD
        public abstract class surveyCD : BqlString.Field<surveyCD> { }
        [PXDefault]
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Survey.surveyCD),
                    typeof(Survey.surveyCD),
                    typeof(Survey.surveyName))]
        [AutoNumber(typeof(SurveySetup.surveyNumberingID), typeof(AccessInfo.businessDate))]
        public virtual string SurveyCD { get; set; }
        #endregion

        #region SurveyName
        public abstract class surveyName : BqlString.Field<Survey.surveyName> { }

        [PXDefault]
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Survey Name")]
        public virtual string SurveyName { get; set; }
        #endregion

        #region Active
        public abstract class active : BqlBool.Field<Survey.active> { }

        [PXDBBool]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : BqlGuid.Field<Survey.noteID> { }
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion

        #region Template
        public abstract class template : BqlString.Field<template> { }
        [PXDBLocalizableString(IsUnicode = true)]
        [PXUIField(DisplayName = "Template")]
        public virtual string Template { get; set; }
        #endregion

        //#region Rendered
        //public abstract class rendered : BqlString.Field<rendered> { }
        //[PXDBLocalizableString(IsUnicode = true)]
        //[PXUIField(DisplayName = "Rendered", IsReadOnly = true)]
        //public virtual string Rendered { get; set; }
        //#endregion

        #region CreatedByID
        public abstract class createdByID : BqlGuid.Field<Survey.createdByID> { }
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        #endregion
        #region CreatedByScreenID
        public abstract class createdByScreenID : BqlString.Field<Survey.createdByScreenID> { }
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        #endregion
        #region CreatedDateTime
        public abstract class createdDateTime : BqlDateTime.Field<Survey.createdDateTime> { }
        [PXDBCreatedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Created Date Time", Enabled = false)]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion
        #region LastModifiedByID
        public abstract class lastModifiedByID : BqlGuid.Field<Survey.lastModifiedByID> { }
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : BqlString.Field<Survey.lastModifiedByScreenID> { }
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        #endregion
        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : BqlDateTime.Field<Survey.lastModifiedDateTime> { }
        [PXDBLastModifiedDateTime(InputMask = "g", DisplayMask = "g")]
        [PXUIField(DisplayName = "Last Modified Date Time", Enabled = false)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion
        #region tstamp
        public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }
        [PXDBTimestamp]
        public virtual Byte[] tstamp { get; set; }
        #endregion

        #region SurveyIDStringID
        public abstract class surveyIDStringID : BqlString.Field<surveyIDStringID> { }
        [PXString]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual string SurveyIDStringID => this.SurveyID.ToString();
        #endregion

        #region IsSurveyInUse
        public abstract class isSurveyInUse : BqlBool.Field<Survey.isSurveyInUse> { }
        /// <summary>
        /// Field to identify if Survey is in use
        /// </summary>
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXBool]
        public virtual bool? IsSurveyInUse { get; set; }
        #endregion
    }
}