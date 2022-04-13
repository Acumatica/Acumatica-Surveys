//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using PX.Data;
//using System;
//using System.Collections;
//using System.Linq;

//namespace PX.Survey.Ext {

//    public partial class EntityFieldSelector : PXCustomSelectorAttribute {

//        private readonly Type _EntityTypeField;

//        public EntityFieldSelector(Type entityTypeField) : base(typeof(SelectorRecord.name)) {
//            _EntityTypeField = entityTypeField;
//            DescriptionField = typeof(SelectorRecord.description);
//            CacheGlobal = true;
//        }

//        private string GetSelection(object data) {
//            var cache = _Graph.Caches[_BqlTable];
//            if (data == null) {
//                data = cache.Current;
//            }
//            return (string)GetValueFromCache(cache, data, _EntityTypeField.Name);
//        }

//        public static object GetValueFromCache(PXCache cache, object row, string fieldName) {
//            object result = cache.GetValue(row, fieldName); // No event called, might be needed
//            if (result == null) {
//                result = cache.GetValueExt(row, fieldName);
//            }
//            if (result == null) {
//                result = cache.GetValuePending(row, fieldName);
//            }
//            result = PXFieldState.UnwrapValue(result);
//            return result;
//        }

//        public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e) {
//            if (!ValidateValue) {
//                return;
//            }
//            if (e.NewValue == null) {
//                return;
//            }
//            var fieldName = e.NewValue.ToString();
//            var entityType = GetSelection(e.Row);
//            if (entityType == null) {
//                return;
//            }
//            var entityHelper = new 
//            //var status = sender.GetStatus(e.Row);
//            //if (status == PXEntryStatus.)
//            try {
//                IModuleHandler mh = Modules.GetModuleHandler(moduleID);
//                var keys = mh.KeyFields;
//                var key = keys?.FirstOrDefault(field => field.Name == fieldName);
//                if (key == null) {
//                    throw new PXSetPropertyException(Messages.FieldCannotBefound, fieldName, moduleID);
//                }
//            } catch {
//                // When module is not created yet.
//            }
//        }

//        public virtual IEnumerable GetRecords() {
//            var moduleID = GetSelection(null);
//            if (moduleID == null) {
//                yield break;
//            }
//            IModuleHandler mh;
//            try {
//                mh = Modules.GetModuleHandler(moduleID);
//            } catch {
//                yield break;
//            }
//            var keys = mh.KeyFields;
//            if (keys != null) {
//                foreach (var key in keys) {
//                    yield return new SelectorRecord { Name = key.Name.ToUpperFirst(), Description = key.Name.ToUpperFirst() };
//                }
//            }
//        }
//    }
//}
