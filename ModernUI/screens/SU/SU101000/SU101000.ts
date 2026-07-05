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
    graphType: "PX.Survey.Ext.SurveySetupMaint",
    primaryView: "surveySetup",
})
export class SU101000 extends PXScreen {
    surveySetup = createSingle(SurveySetup);
    DefaultSurveys = createCollection(SurveySetupEntity);
}

export class SurveySetup extends PXView {
    SurveyNumberingID: PXFieldState<PXFieldOptions.CommitChanges>;
    BadRequestID: PXFieldState<PXFieldOptions.CommitChanges>;
    TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
    DefHeaderID: PXFieldState<PXFieldOptions.CommitChanges>;
    DefPageHeaderID: PXFieldState<PXFieldOptions.CommitChanges>;
    DefQuestionID: PXFieldState<PXFieldOptions.CommitChanges>;
    DefQuestAttrID: PXFieldState<PXFieldOptions.CommitChanges>;
    DefCommentID: PXFieldState<PXFieldOptions.CommitChanges>;
    DefCommAttrID: PXFieldState<PXFieldOptions.CommitChanges>;
    DefPageFooterID: PXFieldState<PXFieldOptions.CommitChanges>;
    DefFooterID: PXFieldState<PXFieldOptions.CommitChanges>;
    DefNbrOfRows: PXFieldState;
    DefMaxLength: PXFieldState;
    WebHookID: PXFieldState<PXFieldOptions.CommitChanges>;
    NotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
    RemindNotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
    ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
    AnonContactID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Details })
export class SurveySetupEntity extends PXView {
    LineNbr: PXFieldState;
    GraphType: PXFieldState<PXFieldOptions.CommitChanges>;
    EntityType: PXFieldState<PXFieldOptions.CommitChanges>;
    ContactField: PXFieldState<PXFieldOptions.CommitChanges>;
    SurveyID: PXFieldState<PXFieldOptions.CommitChanges>;
}
