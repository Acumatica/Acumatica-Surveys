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
    graphType: "PX.Survey.Ext.SurveyCollectorMaint",
    primaryView: "Collector",
})
export class SU301000 extends PXScreen {
    Collector = createSingle(SurveyCollector);
    CollectedAnswers = createCollection(SurveyCollectorData);
}

export class SurveyCollector extends PXView {
    CollectorID: PXFieldState;
    SurveyID: PXFieldState<PXFieldOptions.CommitChanges>;
    Status: PXFieldState;
    SentOn: PXFieldState;
    ExpirationDate: PXFieldState;
    Anonymous: PXFieldState;
    IsTest: PXFieldState;
    ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
    FirstName: PXFieldState;
    LastName: PXFieldState;
    DisplayName: PXFieldState;
    Source: PXFieldState;
    Message: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class SurveyCollectorData extends PXView {
    LineNbr: PXFieldState;
    Status: PXFieldState;
    ComponentID: PXFieldState;
    Description: PXFieldState;
    Value: PXFieldState;
}
