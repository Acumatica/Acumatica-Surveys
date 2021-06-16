using PX.Common;
using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PX.Survey.Ext {
    public class SUAttributeValueAttribute : PXDBStringAttribute {
        private Dictionary<string, Dictionary<string, short>> AttributesWithValues {
            get {
                return PXDatabase.GetSlot<ValuesWithSortOrder>("PXAttributeValueAttribute", new Type[] { typeof(CSAttribute), typeof(CSAttributeDetail) }).AllValues;
            }
        }

        private List<string> MultiSelectAttributes {
            get {
                return PXDatabase.GetSlot<ValuesWithSortOrder>("PXAttributeValueAttribute", new Type[] { typeof(CSAttribute) }).MultiSelectAttributes;
            }
        }

        public SUAttributeValueAttribute() : base(255) {
            base.IsUnicode = true;
        }

        public override void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e) {
            string attributeID;
            if (e.NewValue is DateTime) {
                e.NewValue = Convert.ToString(e.NewValue, CultureInfo.InvariantCulture);
            }
            SurveyAnswer row = e.Row as SurveyAnswer;
            if (row != null) {
                attributeID = row.AttributeID;
            } else {
                attributeID = null;
            }
            string str = attributeID;
            if (!string.IsNullOrWhiteSpace(str) && e.NewValue != null && MultiSelectAttributes.Contains(str)) {
                Dictionary<string, short> item = AttributesWithValues[str];
                IEnumerable<string> strs =
                    from val in (e.NewValue as string).Split(new char[] { ',' })
                    where !string.IsNullOrEmpty(val)
                    select val;
                e.NewValue = string.Join(",",
                    from val in item
                    where strs.Contains<string>(val.Key)
                    orderby val.Value, val.Key
                    select val.Key);
            }
            base.FieldUpdating(sender, e);
        }

        private class ValuesWithSortOrder : IPrefetchable, IPXCompanyDependent {
            public Dictionary<string, Dictionary<string, short>> AllValues { get; private set; }

            public List<string> MultiSelectAttributes { get; private set; }

            public ValuesWithSortOrder() {
            }

            public void Prefetch() {
                Func<CSAttributeDetail, bool> func = null;
                MultiSelectAttributes = PXDatabase.SelectMulti<CSAttribute>(new PXDataField[] { new PXDataField<CSAttribute.attributeID>(), new PXDataField<CSAttribute.controlType>() }).Where((PXDataRecord r) => {
                    int? num = r.GetInt32(1);
                    return num.GetValueOrDefault() == 6 & num.HasValue;
                }).Select((PXDataRecord r) => r.GetString(0)).ToList();
                List<CSAttributeDetail> list = (
                    from r in PXDatabase.SelectMulti<CSAttributeDetail>(new PXDataField[] { new PXDataField<CSAttributeDetail.attributeID>(), new PXDataField<CSAttributeDetail.valueID>(), new PXDataField<CSAttributeDetail.sortOrder>() })
                    where MultiSelectAttributes.Contains(r.GetString(0))
                    select new CSAttributeDetail() {
                        AttributeID = r.GetString(0),
                        ValueID = r.GetString(1),
                        SortOrder = r.GetInt16(2)
                    }).ToList();
                AllValues = new Dictionary<string, Dictionary<string, short>>();
                foreach (string multiSelectAttribute in MultiSelectAttributes) {
                    Dictionary<string, short> strs = new Dictionary<string, short>();
                    List<CSAttributeDetail> cSAttributeDetails = list;
                    Func<CSAttributeDetail, bool> func1 = func;
                    if (func1 == null) {
                        Func<CSAttributeDetail, bool> attributeID = (CSAttributeDetail a) => a.AttributeID == multiSelectAttribute;
                        Func<CSAttributeDetail, bool> func2 = attributeID;
                        func = attributeID;
                        func1 = func2;
                    }
                    foreach (CSAttributeDetail cSAttributeDetail in cSAttributeDetails.Where(func1)) {
                        strs[cSAttributeDetail.ValueID] = cSAttributeDetail.SortOrder.GetValueOrDefault();
                    }
                    AllValues[multiSelectAttribute] = strs;
                }
            }
        }
    }
}