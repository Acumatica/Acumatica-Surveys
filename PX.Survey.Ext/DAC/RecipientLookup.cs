using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PX.Survey.Ext {

    public class RecipientLookup<RowToSelect, RowFilter> : PXSelectBase<RowToSelect>
        where RowToSelect : class, IBqlTable, IPXSelectable, new()
        where RowFilter : class, IBqlTable, new() {

        private const string Selected = "Selected";

        private PXView intView;

        public RecipientLookup(PXGraph graph) {
            Type[] typeArray = new Type[] { BqlCommand.Compose(new Type[] { typeof(Select<>), typeof(RowToSelect) }) };
            View = new PXView(graph, false, BqlCommand.CreateInstance(typeArray), new PXSelectDelegate(viewHandler));
            InitHandlers(graph);
        }

        public RecipientLookup(PXGraph graph, Delegate handler) {
            View = new PXView(graph, false, BqlCommand.CreateInstance(new Type[] { typeof(Select<>), typeof(RowToSelect) }), handler);
            InitHandlers(graph);
        }

        protected virtual PXView CreateIntView(PXGraph graph) {
            Type type = BqlCommand.Compose((new List<Type>()
            {
                typeof(Select<,>),
                typeof(RowToSelect),
                CreateWhere(graph)
            }).ToArray());
            //return new LookupView(graph, BqlCommand.CreateInstance(new Type[] { type }));
            return new PXView(graph, true, BqlCommand.CreateInstance(new Type[] { type }));
        }

        protected static Type CreateWhere(PXGraph graph) {
            Type type = typeof(Where<boolTrue, Equal<boolTrue>>);
            return type;
        }

        private void InitHandlers(PXGraph graph) {
            //graph.RowSelected.AddHandler(typeof(RowFilter), new PXRowSelected(OnFilterSelected));
            graph.RowSelected.AddHandler(typeof(RowToSelect), new PXRowSelected(OnRowSelected));
            //graph.FieldUpdated.AddHandler(typeof(RowToSelect), Selected, new PXFieldUpdated(OnSelectedUpdated));
        }

        //protected virtual void OnFilterSelected(PXCache sender, PXRowSelectedEventArgs e) {
        //    if (e.Row is RowFilter row) {
        //        //PXUIFieldAttribute.SetVisible(sender.Graph.Caches[typeof(RowToSelect)], "siteID", !row.SiteID.HasValue);
        //    }
        //}

        protected virtual void OnRowSelected(PXCache sender, PXRowSelectedEventArgs e) {
            PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
            PXUIFieldAttribute.SetEnabled(sender, e.Row, Selected, true);
            //PXUIFieldAttribute.SetEnabled(sender, e.Row, QtySelected, true);
        }

        //protected virtual void OnSelectedUpdated(PXCache sender, PXFieldUpdatedEventArgs e) {
        //    var value = (bool?)sender.GetValue(e.Row, Selected);
        //    //var nullable = (decimal?)sender.GetValue(e.Row, QtySelected);
        //    bool? nullable1 = value;
        //    if (!(nullable1.GetValueOrDefault() & nullable1.HasValue)) {
        //        //sender.SetValue(e.Row, QtySelected, decimal.Zero);
        //    } else {
        //        //if (nullable.HasValue) {
        //        //    decimal? nullable2 = nullable;
        //        //    decimal num = new decimal();
        //        //    if (!((nullable2.GetValueOrDefault() == num) & nullable2.HasValue)) {
        //        //        return;
        //        //    }
        //        //}
        //        //sender.SetValue(e.Row, QtySelected, decimal.One);
        //    }
        //}

        protected virtual IEnumerable viewHandler() {
            if (intView == null) {
                intView = CreateIntView(View.Graph);
            }
            int startRow = PXView.StartRow;
            int num = 0;
            PXDelegateResult pXDelegateResult = new PXDelegateResult();
            foreach (object obj in intView.Select(null, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref num)) {
                var statu = PXResult.Unwrap<RowToSelect>(obj);
                var statu1 = statu;
                var statu2 = (Cache.Locate(statu) as RowToSelect);
                if (statu2 != null) {
                    bool? value = (Cache.GetValue(statu2, Selected) as bool?);
                    if (value.GetValueOrDefault() & value.HasValue) {
                        //decimal? nullable = Cache.GetValue(statu2, QtySelected) as decimal?;
                        Cache.RestoreCopy(statu2, statu);
                        Cache.SetValue(statu2, Selected, true);
                        //Cache.SetValue(statu2, QtySelected, nullable);
                        statu1 = statu2;
                    }
                }
                pXDelegateResult.Add(statu1);
            }
            PXView.StartRow = 0;
            if (PXView.ReverseOrder) {
                pXDelegateResult.Reverse();
            }
            pXDelegateResult.IsResultSorted = true;
            return pXDelegateResult;
        }

        //protected class LookupView : PXView {
        //    public LookupView(PXGraph graph, BqlCommand command) : base(graph, true, command) { }

        //    public LookupView(PXGraph graph, BqlCommand command, Delegate handler) : base(graph, true, command, handler) { }

        //    protected PXSearchColumn CorrectFieldName(PXSearchColumn orig, bool idFound) {
        //        return orig;
        //    }

        //    protected override List<object> InvokeDelegate(object[] parameters) {
        //        Context array = PXView._Executing.Peek();
        //        PXSearchColumn[] sorts = array.Sorts;
        //        bool flag = false;
        //        List<PXSearchColumn> pXSearchColumns = new List<PXSearchColumn>();
        //        for (int i = 0; i < sorts.Length - Cache.Keys.Count; i++) {
        //            pXSearchColumns.Add(CorrectFieldName(sorts[i], false));
        //            if (sorts[i].Column == "InventoryCD") {
        //                flag = true;
        //            }
        //        }
        //        for (int j = (int)sorts.Length - Cache.Keys.Count; j < sorts.Length; j++) {
        //            PXSearchColumn pXSearchColumn = CorrectFieldName(sorts[j], flag);
        //            if (pXSearchColumn != null) {
        //                pXSearchColumns.Add(pXSearchColumn);
        //            }
        //        }
        //        array.Sorts = pXSearchColumns.ToArray();
        //        return base.InvokeDelegate(parameters);
        //    }
        //}
    }
}
