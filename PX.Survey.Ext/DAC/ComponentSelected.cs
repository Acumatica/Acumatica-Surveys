using PX.Data;
using PX.Data.BQL;
using System;

namespace PX.Survey.Ext {

    [Serializable]
    [PXCacheName("ComponentSelected")]
    [PXProjection(typeof(Select<SurveyComponent,
        Where<SurveyComponent.active, Equal<True>,
        And<SurveyComponent.componentType, NotEqual<SUComponentType.survey>,
        And<SurveyComponent.componentType, NotEqual<SUComponentType.badRequest>,
        And<Where<CurrentValue<ComponentFilter.componentType>, IsNull,
            Or<SurveyComponent.componentType, Equal<CurrentValue<ComponentFilter.componentType>>>>>>>>,
        OrderBy<Asc< SurveyComponent.description>>>))]
    public class ComponentSelected : IBqlTable, IPXSelectable {

        #region Selected
        public abstract class selected : BqlBool.Field<selected> { }
        [PXBool]
        [PXUnboundDefault]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region ComponentID
        public abstract class componentID : BqlString.Field<componentID> { }
        [ComponentID]
        public virtual string ComponentID { get; set; }
        #endregion

        #region ComponentType
        public abstract class componentType : BqlString.Field<componentType> { }
        [PXDBString(2, IsFixed = true, BqlField = typeof(SurveyComponent.componentType))]
        [SUComponentType.DetailList]
        [PXUIField(DisplayName = "Type", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string ComponentType { get; set; }
        #endregion

        #region Description
        public abstract class description : BqlString.Field<description> { }
        [PXDBString(256, IsUnicode = true, BqlField = typeof(SurveyComponent.description))]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        #endregion
    }
}
