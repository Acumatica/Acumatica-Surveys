using PX.Data;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXHidden]
    public class SelectorRecord : IBqlTable {

        #region Name
        public abstract class name : IBqlField { }
        [PXUIField(DisplayName = "Name", Visibility = PXUIVisibility.SelectorVisible)]
        [PXString(128, InputMask = "", IsKey = true)]
        public virtual string Name { get; set; }
        #endregion

        #region Description
        public abstract class description : IBqlField { }
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDBString(128, InputMask = "", IsUnicode = true)]
        public virtual string Description { get; set; }
        #endregion

    }
}
