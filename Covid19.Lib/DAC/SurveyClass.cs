using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace Covid19.Lib
{
    public class SurveyClass : IBqlTable
    {
		#region ItemClassID
        public abstract class surveyClassID : PX.Data.BQL.BqlInt.Field<surveyClassID> { }

        [PXDBIdentity]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual int? SurveyClassID { get; set; }
        #endregion
	}
}
