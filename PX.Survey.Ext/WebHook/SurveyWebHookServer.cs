using PX.Data;
using PX.Data.Webhooks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PX.Survey.Ext.WebHook {
    public class SurveyWebhookServerHandler : IWebhookHandler {

        public const string TOKEN_PARAM = "CollectorToken";
        public const string PAGE_PARAM = "PageNbr";

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        async Task<IHttpActionResult> IWebhookHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            using (var scope = GetUserScope()) {
                string message;
                string collectorToken = "NO_TOKEN";
                HttpStatusCode status = HttpStatusCode.OK;
                try {
                    var _queryParameters = HttpUtility.ParseQueryString(request.RequestUri.Query);
                    collectorToken = _queryParameters.Get(TOKEN_PARAM);
                    if (string.IsNullOrEmpty(collectorToken)) {
                        throw new Exception($"The {TOKEN_PARAM} Parameter was not specified in the Query String");
                    }
                    //            //todo: if the survey has already been awnsered or expired for this collector we need to pass back an alternate to indicate so to the 
                    //            //      user who  clicked the link.
                    var pageNbrStr = _queryParameters.Get(PAGE_PARAM);
                    var pageNbr = SurveyUtils.GetPageNumber(pageNbrStr);
                    if (pageNbrStr != null && request.Method == HttpMethod.Post) {
                        SubmitSurvey(collectorToken, request, pageNbr);
                        pageNbr = SurveyUtils.GetNextOrPrevPageNbr(request, pageNbr);
                    }
                    message = GetSurveyPage(collectorToken, pageNbr);
                } catch (Exception ex) {
                    status = HttpStatusCode.BadRequest;
                    message = GetBadRequestPage(collectorToken, ex.Message);
                }
                return new HtmlActionResult(message, status);
            }
        }

        private string GetBadRequestPage(string token, string message) {
            var generator = new SurveyGenerator();
            var content = generator.GenerateBadRequestPage(token, message);
            return content;
        }

        private void SubmitSurvey(string collectorToken, HttpRequestMessage request, int? pageNbr) {
            var body = request.Content.ReadAsStringAsync().Result;
            var uri = request.RequestUri;
            var props = request.Properties;
            SaveSurveySubmission(collectorToken, body, uri, props, pageNbr);
        }

        private void SaveSurveySubmission(string token, string payload, Uri uri, IDictionary<string, object> props, int? pageNbr) {
            var graph = PXGraph.CreateInstance<SurveyCollectorMaint>();
            var (survey, user) = SurveyUtils.GetSurveyAndUser(graph, token);
            //var queryParams = props != null ? JsonConvert.SerializeObject(props) : null;
            var data = new SurveyCollectorData {
                Token = token,
                Uri = uri.ToString(),
                Payload = payload,
                SurveyID = survey?.SurveyID,
                PageNbr = pageNbr
                //QueryParameters = queryParams
            };
            var inserted = graph.CollectedAnswers.Insert(data);
            graph.Persist();
        }

        private string GetSurveyPage(string collectorToken, int pageNbr) {
            var generator = new SurveyGenerator();
            var content = generator.GenerateSurveyPage(collectorToken, pageNbr);
            return content;
        }

        /// <summary>
        /// Defines the LoginScope to be used for the WebHooks
        /// </summary>
        /// <returns></returns>
        private IDisposable GetUserScope() {
            //todo: For now we will use admin but we will want to throttle back to a 
            //      user with restricted access as to reduce any risk of attack.
            //      perhaps this can be configured in the Surveys Preferences/Setup page.
            var userName = "admin";
            if (PXDatabase.Companies.Length > 0) {
                var company = PXAccess.GetCompanyName();
                if (string.IsNullOrEmpty(company)) {
                    company = PXDatabase.Companies[0];
                }
                userName = userName + "@" + company;
            }
            return new PXLoginScope(userName);
        }

    }

    public class HtmlActionResult : IHttpActionResult {

        private string _message;
        private HttpStatusCode _status;

        public HtmlActionResult(string message, HttpStatusCode status) {
            _message = message;
            _status = status;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
            var response = new HttpResponseMessage(_status);
            response.Content = new StringContent(_message);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
        }
    }
}

