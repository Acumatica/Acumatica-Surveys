using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CR;

namespace Covid19.Lib.DAC
{
    public class ContactSrvExt : PXCacheExtension<Contact>
    {
		#region Add
        public abstract class add : PX.Data.BQL.BqlBool.Field<add> { }

        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Add")]
        public virtual bool? Add { get; set; }
        #endregion
	}
}
