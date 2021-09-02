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
                string collectorToken = "NO_TOKEN";
                try {
                    var _queryParameters = HttpUtility.ParseQueryString(request.RequestUri.Query);
                    collectorToken = _queryParameters.Get(TOKEN_PARAM);
                    if (string.IsNullOrEmpty(collectorToken)) {
                        throw new Exception($"The {TOKEN_PARAM} Parameter was not specified in the Query String");
                    }
                    var pageNbrStr = _queryParameters.Get(PAGE_PARAM);
                    var pageNbr = SurveyUtils.GetPageNumber(pageNbrStr);
                    if (pageNbrStr != null && request.Method == HttpMethod.Post) {
                        SurveyUtils.SubmitSurvey(collectorToken, request, pageNbr);
                        pageNbr = SurveyUtils.GetNextOrPrevPageNbr(request, pageNbr);
                    }
                    var (content, newToken) = GetSurveyPage(collectorToken, pageNbr);
                    if (newToken != null && newToken != collectorToken) {
                        return new RedirectResult(request.RequestUri, newToken, "Was anonymous");
                    } else {
                        return new HtmlActionResult(content, HttpStatusCode.OK);
                    }
                } catch (Exception ex) {
                    PXTrace.WriteError(ex);
                    var content = GetBadRequestPage(collectorToken, $"{ex.Message}:\n{ex.StackTrace}");
                    return new HtmlActionResult(content, HttpStatusCode.BadRequest);
                }
            }
        }

        private string GetBadRequestPage(string token, string message) {
            var generator = new SurveyGenerator();
            var content = generator.GenerateBadRequestPage(token, message);
            return content;
        }

        private (string content, string token) GetSurveyPage(string collectorToken, int pageNbr) {
            var generator = new SurveyGenerator();
            var (content, token) = generator.GenerateSurveyPage(collectorToken, pageNbr);
            return (content, token);
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

        public class RedirectResult : IHttpActionResult {

            private Uri _location;
            private string _reason;

            public RedirectResult(Uri uri, string newToken, string reason) {
                var ub = new UriBuilder(uri);
                var qs = HttpUtility.ParseQueryString(ub.Query);
                qs.Set(TOKEN_PARAM, newToken);
                ub.Query = qs.ToString();
                _location = ub.Uri;
                _reason = reason;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
                var redirect = new HttpResponseMessage(HttpStatusCode.Redirect);
                redirect.Headers.Location = _location;
                redirect.ReasonPhrase = _reason;
                return Task.FromResult(redirect);
            }
        }
    }
}

