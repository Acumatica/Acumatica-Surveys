using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace Covid19.Lib
{
    public class CovidClass : IBqlTable
    {
		#region ItemClassID
        public abstract class covidClassID : PX.Data.BQL.BqlInt.Field<covidClassID> { }

        [PXDBIdentity]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual int? CovidClassID { get; set; }
        #endregion
	}
}
