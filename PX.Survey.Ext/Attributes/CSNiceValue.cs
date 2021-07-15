using PX.Data;
using PX.Objects.CR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Survey.Ext {

    public class CSNiceValue<AttributeField, ValueField> : BqlFormulaEvaluator<AttributeField, ValueField>, IBqlOperand
        where AttributeField : IBqlField
        where ValueField : IBqlField {

        public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars) {
            string attributeID = (string)pars[typeof(AttributeField)];
            string rawValue = (string)pars[typeof(ValueField)];
            if (rawValue == null || attributeID == null) {
                return rawValue;
            }
            CRAttribute.Attribute attr = CRAttribute.Attributes[attributeID];
            if (attr == null || attr.ControlType == null 
                || attr.ControlType == SUControlType.Text
                || attr.ControlType == SUControlType.Datetime) {
                return rawValue;
            }
            List<CRAttribute.AttributeValue> attributeValues = attr.Values;
            if (attr == null || attr.Values == null || attr.Values.Count < 1) {
                return null;
            }
            var controlType = attr?.ControlType;
            string desc = null;
            switch (controlType) {
                case SUControlType.Combo:
                    desc = attributeValues.FirstOrDefault(av => av.ValueID == rawValue && av.Disabled != true)?.Description;
                    return desc;
                case SUControlType.Multi:
                    List<string> strs = new List<string>();
                    List<string> strs1 = new List<string>();
                    foreach (CRAttribute.AttributeValue attributeValue in attributeValues) {
                        if (attributeValue.Disabled && rawValue != attributeValue.ValueID) {
                            continue;
                        }
                        strs.Add(attributeValue.ValueID);
                        strs1.Add(attributeValue.Description);
                    }
                    desc = string.Join(", ", rawValue.Split(new char[] { ',' }).Select<string, string>((string i) => {
                        int num = strs.IndexOf(i.Trim());
                        if (num < 0) {
                            return i;
                        }
                        return strs1[num];
                    }));
                    return desc;
                case SUControlType.Selector:
                    // TODO
                    break;
            }
            return null;
        }
    }
}