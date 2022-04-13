using PX.Data;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Survey.Ext {

    public class SUGraphSelectorAttribute : TypeSelectorAttribute {

        private IEnumerable<Graph> _matchingValues;

        public SUGraphSelectorAttribute() : base((Type)null) {
        }

        public virtual new IEnumerable GetRecords() {
            foreach (var graph in GetMatchingValues()) {
                yield return new SelectorRecord { Name = graph.GraphName, Description = graph.Text };
            }
        }

        protected new IEnumerable<Graph> GetMatchingValues() {
            if (_matchingValues == null) {
                _matchingValues = GraphHelper.GetGraphAll(true).Where(gr => KeepGraph(gr));
            }
            return _matchingValues;
        }

        protected virtual bool KeepGraph(Graph gr) {
            if (gr.IsReport == true || gr.IsNamespace == true) {
                return false;
            }
            //return gr.GraphName.StartsWith("PX.Objects.") || gr.GraphName.StartsWith("PB.Objects.");
            return true;
        }

        public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e) {
            if (!this.ValidateValue) {
                return;
            }
            if (e.NewValue == null) {
                return;
            }
            var graphName = e.NewValue.ToString();
            var graphs = GetMatchingValues();
            var graph = graphs.FirstOrDefault(gr => gr.GraphName == graphName);
            if (graph == null) {
                throw new PXSetPropertyException(Messages.GraphCannotBefound, _FieldName, graphName);
            }
        }

        //public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e) {
        //    if (!ValidateValue || e.NewValue == null ||
        //        (sender.Keys.Count != 0 && _FieldName == sender.Keys[sender.Keys.Count - 1])) return;
        //    var objectList = sender.Graph.Views[_ViewName].SelectMultiBound(new object[1]
        //    {
        //        e.Row
        //    });
        //    var cach = sender.Graph.Caches[BqlCommand.GetItemType(ForeignField)];
        //    foreach (var row in objectList) {
        //        object data = PXResult.UnwrapMain(row);
        //        var objA = cach.GetValue(data, ForeignField.Name);
        //        if (Equals(objA, e.NewValue))
        //            return;
        //        if (objA is Array myArray && e.NewValue is Array eNewArray && myArray.Length == eNewArray.Length) {
        //            var flag = true;
        //            var index = 0;
        //            while (index < myArray.Length && (flag = flag && Equals(myArray.GetValue(index), eNewArray.GetValue(index))))
        //                ++index;
        //            if (flag)
        //                return;
        //        }
        //    }
        //    throw new PXSetPropertyException(PXMessages.LocalizeFormat("'{0}' cannot be found in the system.", _FieldName));
        //}
    }
}
