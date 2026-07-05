import {
    PXScreen,
    createSingle,
    createCollection,
    graphInfo,
    gridConfig,
    GridPreset,
    PXView,
    PXFieldState,
    PXFieldOptions,
} from "client-controls";

@graphInfo({
    graphType: "PX.Survey.Ext.SurveyProcess",
    primaryView: "Filter",
})
export class SU501000 extends PXScreen {
    Filter = createSingle(SurveyFilter);
    Documents = createCollection(SurveyCollector);
}

export class SurveyFilter extends PXView {
    Action: PXFieldState<PXFieldOptions.CommitChanges>;
    SurveyID: PXFieldState<PXFieldOptions.CommitChanges>;
    DurationTimeSpan: PXFieldState<PXFieldOptions.CommitChanges>;
    ShowClosed: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Processing })
export class SurveyCollector extends PXView {
    Selected: PXFieldState;
    CollectorID: PXFieldState;
    SurveyID: PXFieldState;
    ContactID: PXFieldState;
    DisplayName: PXFieldState;
    Status: PXFieldState;
    SentOn: PXFieldState;
    ExpirationDate: PXFieldState;
    Message: PXFieldState;
}
