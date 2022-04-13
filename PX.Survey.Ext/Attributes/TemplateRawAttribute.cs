//using PX.Data;
//using PX.Objects.GL;
//using System;

//namespace PX.Survey.Ext {

//    [PXDBString(InputMask = "", IsUnicode = true)]
//    [PXUIField(DisplayName = "Inventory ID", Visibility = PXUIVisibility.SelectorVisible)]
//    public sealed class TemplateRawAttribute : AcctSubAttribute {

//        public const string DimensionName = "SURVEYTEMPLATE";
//        private readonly Type _whereType;

//        public TemplateRawAttribute() {
//            Type type = typeof(Search<SurveyTemplate.templateCD>);
//            PXDimensionSelectorAttribute pXDimensionSelectorAttribute = new PXDimensionSelectorAttribute(DimensionName, type, typeof(SurveyTemplate.templateCD)) {
//                DescriptionField = typeof(SurveyTemplate.description),
//                CacheGlobal = true
//            };
//            this._Attributes.Add(pXDimensionSelectorAttribute);
//            this._SelAttrIndex = this._Attributes.Count - 1;
//        }

//        public TemplateRawAttribute(Type WhereType) : this() {
//            if (WhereType != null) {
//                this._whereType = WhereType;
//                Type type = BqlCommand.Compose(new Type[] { typeof(Search<,>), typeof(SurveyTemplate.templateCD), this._whereType });
//                PXDimensionSelectorAttribute pXDimensionSelectorAttribute = new PXDimensionSelectorAttribute(DimensionName, type, typeof(SurveyTemplate.templateCD)) {
//                    DescriptionField = typeof(SurveyTemplate.description),
//                    CacheGlobal = true
//                };
//                this._Attributes[this._SelAttrIndex] = pXDimensionSelectorAttribute;
//            }
//        }
//    }
//}
