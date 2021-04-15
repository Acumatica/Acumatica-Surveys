//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using PX.Common;
//using PX.Data;
//using PX.SM;

//namespace PX.Survey.Ext {

//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
//    public class PXDBLocalizableTextAttribute : PXDBTextAttribute {

//        private Type _localeField;
//        private Definition _Definition;
//        private Dictionary<string, int> _Indexes;
//        private Dictionary<string, int> _FieldIndexes;
//        private int _PositionInTranslations;

//        public PXDBLocalizableTextAttribute(Type localeField) {
//            this._localeField = localeField;
//            this.IsKey = false;
//            this.InputMask = null;
//        }

//        public override void CacheAttached(PXCache sender) {
//            base.CacheAttached(sender);
//            this._Definition = PXContext.GetSlot<Definition>();
//            if (this._Definition == null) {
//                Definition slot = PXDatabase.GetSlot<Definition>("Definition", new Type[] { typeof(Locale) });
//                Definition definition = slot;
//                this._Definition = slot;
//                PXContext.SetSlot<Definition>(definition);
//            }
//            sender.Graph._RecordCachedSlot(base.GetType(), this._Definition, () => PXContext.SetSlot<Definition>(PXDatabase.GetSlot<Definition>("Definition", new Type[] { typeof(Locale) })));
//            if (this._Definition != null && string.IsNullOrEmpty(this._Definition.DefaultLocale)) {
//                this._Definition = null;
//            }
//            if (this._Definition != null) {
//                this._Indexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
//                this._FieldIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
//                for (int num = 0; num < this._Definition.DefaultPlusAlternative.Count; num++) {
//                    this._Indexes[this._Definition.DefaultPlusAlternative[num]] = num;
//                    this._FieldIndexes[string.Concat((this.IsProjection ? this._DatabaseFieldName : this._FieldName), this._Definition.DefaultPlusAlternative[num].ToUpper())] = num;
//                }
//                if (this.NonDB) {
//                    sender.RowSelecting += new PXRowSelecting(RowSelecting);
//                }
//                string lower = string.Concat(this._FieldName, "Translations");
//                if (!sender.Fields.Contains(lower)) {
//                    sender.Fields.Add(lower);
//                    lower = lower.ToLower();
//                    PXCache.EventDictionary<PXFieldSelecting> fieldSelectingEvents = sender.FieldSelectingEvents;
//                    string str = lower;
//                    fieldSelectingEvents[str] = (PXFieldSelecting)Delegate.Combine(fieldSelectingEvents[str], new PXFieldSelecting(Translations_FieldSelecting));
//                    PXCache.EventDictionary<PXFieldUpdating> fieldUpdatingEvents = sender.FieldUpdatingEvents;
//                    str = lower;
//                    fieldUpdatingEvents[str] = (PXFieldUpdating)Delegate.Combine(fieldUpdatingEvents[str], new PXFieldUpdating(Translations_FieldUpdating));
//                }
//                int count = this._Definition.DefaultPlusAlternative.Count;
//                this._PositionInTranslations = sender.SetupSlot<string[]>(() => new string[count], (string[] item, string[] copy) => {
//                    for (int i = 0; i < count && i < (int)item.Length && i < (int)copy.Length; i++) {
//                        item[i] = copy[i];
//                    }
//                    return item;
//                }, (string[] item) => (string[])item.Clone());
//            }
//        }

//        public override void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e) {
//            var locale = GetLocale(sender, e.Row);
//            if (e.NewValue != null && !e.Cancel) {
//                string newValue = e.NewValue as string ?? Convert.ToString(e.NewValue);
//                e.NewValue = newValue;
//                //if (!this._IsFixed) {
//                //    e.NewValue = ((string)e.NewValue).TrimEnd(Array.Empty<char>());
//                //}
//                //if (this._Length >= 0) {
//                //    int length = ((string)e.NewValue).Length;
//                //    if (length > this._Length) {
//                //        e.NewValue = ((string)e.NewValue).Substring(0, this._Length);
//                //        return;
//                //    }
//                //    if (this._IsFixed && length < this._Length) {
//                //        StringBuilder stringBuilder = new StringBuilder((string)e.NewValue, this._Length);
//                //        for (int i = length; i < this._Length; i++) {
//                //            stringBuilder.Append(' ');
//                //        }
//                //        e.NewValue = stringBuilder.ToString();
//                //    }
//                //}
//            }
//        }

//        public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e) {
//            var locale = GetLocale(sender, e.Row);
//            var state = e.ReturnState;
//            var val = e.ReturnValue;
//            if (state is PXStringState pxSS) {
//                int? length = new int?(_Length);
//                bool? isUni = new bool?(_IsUnicode);
//                string fieldName = _FieldName;
//                e.ReturnState = PXStringState.CreateInstance(state, length, isUni, fieldName, false, null, null, null, null, null, null, null);
//            }
//        }

//        private object GetLocale(PXCache sender, object row) {
//            var locale = (string)sender.GetValue(row, _localeField.Name);
//            if (row == null || locale == null) {
//                locale = "en";
//            }
//            throw new NotImplementedException();
//        }

//        public static List<string> EnabledLocales {
//            get {
//                Definition slot = PXContext.GetSlot<Definition>();
//                if (slot == null) {
//                    Definition definition = PXDatabase.GetSlot<PXDBLocalizableStringAttribute.("Definition", new Type[] { typeof(Locale) });
//                    slot = definition;
//                    PXContext.SetSlot<Definition>(definition);
//                }
//                if (slot == null) {
//                    return new List<string>();
//                }
//                return slot.DefaultPlusAlternative;
//            }
//        }

//        public static bool HasMultipleLocales {
//            get {
//                Definition slot = PXContext.GetSlot<Definition>();
//                if (slot == null) {
//                    Definition definition = PXDatabase.GetSlot<Definition>("Definition", new Type[] { typeof(Locale) });
//                    slot = definition;
//                    PXContext.SetSlot<Definition>(definition);
//                }
//                if (slot != null && slot.HasMultipleLocales) {
//                    return true;
//                }
//                return false;
//            }
//        }

//        public static bool IsEnabled {
//            get {
//                Definition slot = PXContext.GetSlot<Definition>();
//                if (slot == null) {
//                    Definition definition = PXDatabase.GetSlot<Definition>("Definition", new Type[] { typeof(Locale) });
//                    slot = definition;
//                    PXContext.SetSlot<Definition>(definition);
//                }
//                if (slot != null && !string.IsNullOrEmpty(slot.DefaultLocale)) {
//                    return true;
//                }
//                return false;
//            }
//        }

//        protected class Definition : IPrefetchable, IPXCompanyDependent {

//            public string DefaultLocale;
//            public List<string> DefaultPlusAlternative;
//            public bool HasMultipleLocales;
//            public Dictionary<string, List<string>> LocalesByLanguage = new Dictionary<string, List<string>>();

//            public Definition() { }

//            public void Prefetch() {
//                List<string> strs;
//                DefaultPlusAlternative = new List<string>();
//                int num = 0;
//                HashSet<string> strs1 = new HashSet<string>();
//                foreach (PXDataRecord pXDataRecord in PXDatabase.SelectMulti<Locale>(new PXDataField[] { new PXDataField<Locale.localeName>(), new PXDataField<Locale.isDefault>(), new PXDataField<Locale.isAlternative>(), new PXDataFieldValue<Locale.isActive>(PXDbType.Bit, new int?(1), true), new PXDataFieldOrder<Locale.number>() })) {
//                    string str = pXDataRecord.GetString(0);
//                    if (string.IsNullOrEmpty(str)) {
//                        continue;
//                    }
//                    string str1 = str;
//                    num++;
//                    str = (new CultureInfo(str)).TwoLetterISOLanguageName;
//                    bool? flag = pXDataRecord.GetBoolean(1);
//                    if (!(flag.GetValueOrDefault() & flag.HasValue)) {
//                        flag = pXDataRecord.GetBoolean(2);
//                        if (!(flag.GetValueOrDefault() & flag.HasValue)) {
//                            continue;
//                        }
//                    } else {
//                        this.DefaultLocale = str;
//                    }
//                    if (strs1.Add(str)) {
//                        this.DefaultPlusAlternative.Add(str);
//                    }
//                    if (!this.LocalesByLanguage.TryGetValue(str, out strs)) {
//                        Dictionary<string, List<string>> localesByLanguage = this.LocalesByLanguage;
//                        List<string> strs2 = new List<string>();
//                        strs = strs2;
//                        localesByLanguage[str] = strs2;
//                    }
//                    strs.Add(str1);
//                }
//                this.HasMultipleLocales = num > 1;
//            }
//        }
//    }
//}
