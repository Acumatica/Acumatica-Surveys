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
    graphType: "PX.Survey.Ext.SurveyMaint",
    primaryView: "Survey",
})
export class SU201000 extends PXScreen {
    Survey = createSingle(Survey);
    Details = createCollection(SurveyDetail);
    Users = createCollection(SurveyUser);
    Collectors = createCollection(SurveyCollector);
    Answers = createCollection(SurveyAnswer);
}

export class Survey extends PXView {
    SurveyID: PXFieldState<PXFieldOptions.CommitChanges>;
    Title: PXFieldState<PXFieldOptions.CommitChanges>;
    Status: PXFieldState;
    Target: PXFieldState<PXFieldOptions.CommitChanges>;
    Layout: PXFieldState<PXFieldOptions.CommitChanges>;
    TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
    EntityType: PXFieldState<PXFieldOptions.CommitChanges>;
    AllowAnonymous: PXFieldState<PXFieldOptions.CommitChanges>;
    KeepAnswersAnonymous: PXFieldState<PXFieldOptions.CommitChanges>;
    AllowDuplicate: PXFieldState<PXFieldOptions.CommitChanges>;
    NotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
    RemindNotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Details })
export class SurveyDetail extends PXView {
    Active: PXFieldState;
    PageNbr: PXFieldState<PXFieldOptions.CommitChanges>;
    ComponentType: PXFieldState<PXFieldOptions.CommitChanges>;
    ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
    QuestionNbr: PXFieldState<PXFieldOptions.CommitChanges>;
    Description: PXFieldState;
    AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
    AttrDesc: PXFieldState<PXFieldOptions.CommitChanges>;
    Required: PXFieldState<PXFieldOptions.CommitChanges>;
    ControlType: PXFieldState<PXFieldOptions.CommitChanges>;
    ReverseOrder: PXFieldState<PXFieldOptions.CommitChanges>;
    NbrOfRows: PXFieldState<PXFieldOptions.CommitChanges>;
    MaxLength: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Details })
export class SurveyUser extends PXView {
    Active: PXFieldState;
    ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
    RecipientType: PXFieldState<PXFieldOptions.CommitChanges>;
    Phone1Type: PXFieldState<PXFieldOptions.CommitChanges>;
    Phone2Type: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Details })
export class SurveyCollector extends PXView {
    Selected: PXFieldState;
    CollectorID: PXFieldState;
    SurveyID: PXFieldState;
    ContactID: PXFieldState;
    Status: PXFieldState;
    SentOn: PXFieldState;
    ExpirationDate: PXFieldState;
    Anonymous: PXFieldState;
    IsTest: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class SurveyAnswer extends PXView {
    CollectorID: PXFieldState;
    DetailLineNbr: PXFieldState;
    Value: PXFieldState;
}
