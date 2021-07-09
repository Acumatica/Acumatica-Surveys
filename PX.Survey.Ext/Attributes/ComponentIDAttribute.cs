using PX.Data;
using PX.Objects.GL;
using System;

namespace PX.Survey.Ext {

    [PXString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
    [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
    [PXUIField(DisplayName = "Component", Visibility = PXUIVisibility.SelectorVisible)]
    public class ComponentIDAttribute : AcctSubAttribute {

        public ComponentIDAttribute() : this(typeof(Where<True, Equal<True>>)) {
        }

        public ComponentIDAttribute(Type WhereType) {
            var type = BqlCommand.Compose(new Type[] { typeof(Search<,>), typeof(SurveyComponent.componentID), WhereType });
            var selectorAttribute = new PXSelectorAttribute(type) {
                DescriptionField = typeof(SurveyComponent.description),
                CacheGlobal = true
            };
            _Attributes.Add(selectorAttribute);
            _SelAttrIndex = _Attributes.Count - 1;
        }

        public ComponentIDAttribute(Type WhereType, Type JoinType) {
            Type type = BqlCommand.Compose(new Type[] { typeof(Search2<,,>), typeof(SurveyComponent.componentID), JoinType, WhereType });
            var selectorAttribute = new PXSelectorAttribute(type) {
                DescriptionField = typeof(SurveyComponent.description),
                CacheGlobal = true
            };
            _Attributes.Add(selectorAttribute);
            _SelAttrIndex = _Attributes.Count - 1;
        }
    }
}
