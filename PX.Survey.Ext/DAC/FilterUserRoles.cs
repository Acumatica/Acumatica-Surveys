using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.SM;

namespace PX.Survey.Ext
{
    [Serializable]
    [PXCacheName("Filter users roles")]
    public class FilterUserRoles : IBqlTable
    {
        #region selectAll
        public abstract class selectAll : PX.Data.BQL.BqlBool.Field<selectAll> { }
        [PXBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Select All")]
        public virtual bool? SelectAll { get; set; }
        #endregion

        #region SelectedRole
        public abstract class selectedRole : PX.Data.BQL.BqlString.Field<selectedRole> { }

        [PXString(150)]
        [PXUIField(DisplayName = "Role Name", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXSelector(
            typeof(Search<Roles.rolename>),
            typeof(Roles.rolename))]
        public virtual String SelectedRole { get; set; }
        #endregion
    }
}
