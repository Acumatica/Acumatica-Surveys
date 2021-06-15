using PX.Data;
using PX.Objects.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PX.Survey.Ext {

    public class TypeSelectorAttribute : PXCustomSelectorAttribute {

        public Func<Type, bool> predicate;
        private IEnumerable<Type> _matchingValues;

        public TypeSelectorAttribute(Type interfaceToList) :
            this(GetPredicate(interfaceToList)) {
        }

        public TypeSelectorAttribute(Func<Type, bool> predicate) : base(typeof(SelectorRecord.name)) {
            DescriptionField = typeof(SelectorRecord.description);
            this.predicate = predicate;
            CacheGlobal = true;
            DirtyRead = false;
        }

        public static Func<Type, bool> GetPredicate(Type inter) {
            Func<Type, bool> predicate = ((t) => IsImplementationOfInterface(inter, t));
            return predicate;
        }

        public static bool IsImplementationOfInterface(Type inter, Type t) {
            return (t != null && inter.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        }

        public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e) {
            if (!this.ValidateValue) {
                return;
            }
            if (e.NewValue == null) {
                return;
            }
            var typeName = e.NewValue.ToString();
            var matchingTypes = GetMatchingValues();
            var matchingType = matchingTypes.FirstOrDefault(ty => ty.FullName == typeName);
            if (matchingType == null) {
                throw new PXSetPropertyException(Messages.ValueCannotBefound, _FieldName, typeName);
            }
        }

        public virtual IEnumerable GetRecords() {
            foreach (var type in GetMatchingValues()) {
                yield return new SelectorRecord { Name = type.FullName, Description = type.Name };
            }
        }

        protected IEnumerable<Type> GetMatchingValues() {
            if (_matchingValues == null) {
                IList<Type> allTypes = new List<Type>();
                foreach (var ass in AppDomain.CurrentDomain.GetAssemblies()) {
                    if (PXSubstManager.IsSuitableTypeExportAssembly(ass, true)) {
                        Type[] types = null;
                        try {
                            if (!ass.IsDynamic)
                                types = ass.GetExportedTypes();
                        } catch (ReflectionTypeLoadException te) {
                            types = te.Types;
                        } catch {
                            continue;
                        }
                        if (types != null) {
                            allTypes.AddRange(types.Where(ty => predicate(ty)));
                        }
                    }
                }
                _matchingValues = allTypes.Distinct();
            }
            return _matchingValues;
        }
    }
}
