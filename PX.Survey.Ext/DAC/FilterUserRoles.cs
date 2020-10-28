using PX.Data;
//using PX.SM;
using System;
using PX.Objects.EP;
using PX.Objects.GL;

namespace AcumaticaSurveysLibr
{
    [Serializable]
    public class FilterUserRoles : IBqlTable
    {
        #region DepartmentID
        public abstract class departmentID : PX.Data.BQL.BqlString.Field<departmentID> { }
        protected String _DepartmentID;
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
        [PXUIField(DisplayName = "Department", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String DepartmentID { get; set; }
        #endregion
        #region VendorClassID
        public new abstract class vendorClassID : PX.Data.BQL.BqlString.Field<vendorClassID> { }
        [PXDBString(10, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Employee Class", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(EPEmployeeClass.vendorClassID), DescriptionField = typeof(EPEmployeeClass.descr))]
        public virtual String VendorClassID { get; set; }
        #endregion
        #region ParentBAccountID
        public abstract class parentBAccountID : PX.Data.BQL.BqlInt.Field<parentBAccountID> { }
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch")]
        [PXDimensionSelector("BIZACCT", typeof(Search<Branch.bAccountID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
        public virtual Int32? ParentBAccountID { get; set; }
        #endregion
    }
}