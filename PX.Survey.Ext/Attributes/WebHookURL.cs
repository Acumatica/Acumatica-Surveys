using PX.Data;
using System;
using System.Collections.Generic;

namespace PX.Survey.Ext {

    public class WebHookURL<WebHookField> : BqlFormulaEvaluator<WebHookField>, IBqlOperand
        where WebHookField : IBqlField {

        public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars) {
            var webHookID = (Guid?)pars[typeof(WebHookField)];
            if (webHookID == null) {
                return null;
            }
            Api.Webhooks.Graph.WebhookMaint whGraph = PXGraph.CreateInstance<Api.Webhooks.Graph.WebhookMaint>();
            //Api.Webhooks.DAC.WebHook wh = PXSelect<Api.Webhooks.DAC.WebHook,
            //        Where<Api.Webhooks.DAC.WebHook.webHookID, Equal<Required<Api.Webhooks.DAC.WebHook.webHookID>>>>.Select(whGraph, survey.WebHookID);
            //whGraph.Webhook.Current = wh;
            whGraph.Webhook.Current = whGraph.Webhook.Search<Api.Webhooks.DAC.WebHook.webHookID>(webHookID);
            return whGraph.Webhook.Current?.Url;
        }
    }
}