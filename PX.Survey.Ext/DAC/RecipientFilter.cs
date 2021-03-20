using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Survey.Ext {

    [PXCacheName("RecipientFilter")]
    public class RecipientFilter : IBqlTable {

        #region OnlyUsers
        public abstract class onlyUsers : BqlBool.Field<onlyUsers> { }
        [PXBool]
        [PXUnboundDefault(true)]
        [PXUIField(DisplayName = "Only Users")]
        public virtual bool? OnlyUsers { get; set; }
        #endregion

        #region ContactType
        public abstract class contactType : BqlString.Field<contactType> { }
        [ContactTypes]
        [PXDBString(2, IsFixed = true)]
        [PXDefault(ContactTypesAttribute.Employee)]
        [PXUIField(DisplayName = "Recipient Type")]
        public virtual string ContactType { get; set; }
        #endregion
    }
}
