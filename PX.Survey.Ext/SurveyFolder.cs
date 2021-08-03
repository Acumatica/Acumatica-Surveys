//using PX.Data;
//using System;
//using System.Collections;

//namespace PX.Survey.Ext {

//    public class SurveyFolder<TNode> : PXAction<TNode>
//    where TNode : class, IBqlTable, new() {
//        public SurveyFolder(PXGraph graph, string name) : base(graph, name) {
//        }

//        public SurveyFolder(PXGraph graph, Delegate handler) : base(graph, handler) {
//        }

//        [PXButton(MenuAutoOpen = true, SpecialType = PXSpecialButtonType.ReportsFolder)]
//        [PXUIField(DisplayName = "Surveys", MapEnableRights = PXCacheRights.Select)]
//        protected override IEnumerable Handler(PXAdapter adapter) {
//            return adapter.Get();
//        }
//    }
//}
