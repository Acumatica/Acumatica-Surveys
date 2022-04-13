using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PX.Survey.Ext {

    public class ComponentLookup<RowToSelect, RowFilter> : PXSelectBase<RowToSelect>
        where RowToSelect : class, IBqlTable, IPXSelectable, new()
        where RowFilter : class, IBqlTable, new() {

        private const string Selected = "Selected";

        private PXView intView;

        public ComponentLookup(PXGraph graph) {
            Type[] typeArray = new Type[] { BqlCommand.Compose(new Type[] { typeof(Select<>), typeof(RowToSelect) }) };
            View = new PXView(graph, false, BqlCommand.CreateInstance(typeArray), new PXSelectDelegate(viewHandler));
            InitHandlers(graph);
        }

        public ComponentLookup(PXGraph graph, Delegate handler) {
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
            return new PXView(graph, true, BqlCommand.CreateInstance(new Type[] { type }));
        }

        protected static Type CreateWhere(PXGraph graph) {
            Type type = typeof(Where<boolTrue, Equal<boolTrue>>);
            return type;
        }

        private void InitHandlers(PXGraph graph) {
            graph.RowSelected.AddHandler(typeof(RowToSelect), new PXRowSelected(OnRowSelected));
        }

        protected virtual void OnRowSelected(PXCache sender, PXRowSelectedEventArgs e) {
            PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
            PXUIFieldAttribute.SetEnabled(sender, e.Row, Selected, true);
        }

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
                        Cache.RestoreCopy(statu2, statu);
                        Cache.SetValue(statu2, Selected, true);
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
    }
}
