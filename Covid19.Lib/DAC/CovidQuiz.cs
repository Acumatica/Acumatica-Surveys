using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.SM;
using PX.Web.UI.Frameset.Model.DTO;

namespace Covid19.Lib
{
    public class CovidQuiz : IBqlTable
    {
		#region QuizCD
        public abstract class quizCD : PX.Data.BQL.BqlString.Field<quizCD> { }
        
        
        [PXDefault()]
        [PXDBString(30, IsFixed = true, IsKey = true)]
		[PXUIField(DisplayName = "Quiz CD")]
		[PXSelector(typeof(CovidQuiz.quizCD))]
        public virtual String QuizCD
        {
            get;set;
        }
		#endregion

        #region QuizID
        public abstract class quizID : PX.Data.BQL.BqlInt.Field<quizID> { }


        [PXDefault()]
		[PXDBIdentity]
        public virtual int? QuizID
		{
            get; set;
        }
		#endregion

		#region QuizedUser
        public abstract class quizedUser : PX.Data.BQL.BqlGuid.Field<quizedUser> { }
        [PXSelector(typeof(Users.pKID), SubstituteKey = typeof(Users.username))]
		[PXUIField(DisplayName = "Quized user")]
		[PXDBGuid()]
        public virtual Guid? QuizedUser
		{
            get;
            set;
        }
        #endregion

		#region NoteID
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
		protected Guid? _NoteID;

		[PXNote(PopupTextEnabled = true)]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion

        #region Attributes
        public abstract class attributes : BqlAttributes.Field<attributes> { }

        [CRAttributesField(typeof(CovidQuiz.covidClassID))]
        public virtual string[] Attributes { get; set; }

		#endregion

		#region CovidClassID
		public abstract class covidClassID : PX.Data.BQL.BqlInt.Field<covidClassID> { }

        [PXDBInt]
		[PXDefault(1)]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual int? CovidClassID { get; set; }
        #endregion




		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		[PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false, IsReadOnly = true)]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		[PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime, Enabled = false, IsReadOnly = true)]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
	}
}
