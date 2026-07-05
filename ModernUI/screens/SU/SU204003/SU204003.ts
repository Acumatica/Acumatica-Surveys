import {
    PXScreen,
    createSingle,
    graphInfo,
    fieldConfig,
    IFieldConfig,
    IRichTextEditorConfig,
    PXView,
    PXFieldState,
    PXFieldOptions,
} from "client-controls";

@graphInfo({
    graphType: "PX.Survey.Ext.SurveyComponentMaint",
    primaryView: "SUComponent",
})
export class SU204003 extends PXScreen {
    SUComponent = createSingle(SurveyComponent);
}

export class SurveyComponent extends PXView {
    ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
    Description: PXFieldState;
    ComponentType: PXFieldState<PXFieldOptions.CommitChanges>;
    Active: PXFieldState;

    // Rich text editor with built-in Visual / HTML / Plain Text mode switcher
    // (the default toolbar includes "mode-group"), expands to content with
    // a comfortable minimum height.
    @fieldConfig({
        noLabel: true,
        controlType: "qp-rich-text-editor",
        controlConfig: {
            expandToContent: true,
            expandToContentMinHeight: 400,
            spellcheck: false,
        } as IRichTextEditorConfig,
    } as IFieldConfig)
    Body: PXFieldState;
}
