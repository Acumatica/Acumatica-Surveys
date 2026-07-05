using PX.Api.Webhooks;
using PX.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PX.Survey.Ext.WebHook {
    public class SurveyWebhookServerHandler : IWebhookHandler {

        public const string TOKEN_PARAM = "CollectorToken";
        public const string PAGE_PARAM = "PageNbr";

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        async Task IWebhookHandler.HandleAsync(WebhookContext context, CancellationToken cancellationToken) {
            using (var scope = GetUserScope()) {
                string collectorToken = "NO_TOKEN";
                try {
                    var request = context.Request;
                    collectorToken = GetQueryValue(request, TOKEN_PARAM);
                    if (string.IsNullOrEmpty(collectorToken)) {
                        throw new Exception($"The {TOKEN_PARAM} Parameter was not specified in the Query String");
                    }
                    var pageNbrStr = GetQueryValue(request, PAGE_PARAM);
                    var pageNbr = SurveyUtils.GetPageNumber(pageNbrStr);
                    var requestBody = SurveyUtils.ReadRequestBody(request.Body);
                    if (pageNbrStr != null && string.Equals(request.Method, "POST", StringComparison.OrdinalIgnoreCase)) {
                        SurveyUtils.SubmitSurvey(collectorToken, requestBody, BuildRequestUri(request), pageNbr);
                        pageNbr = SurveyUtils.GetNextOrPrevPageNbr(requestBody, pageNbr);
                    }
                    var (content, newToken) = GetSurveyPage(collectorToken, pageNbr);
                    if (newToken != null && newToken != collectorToken) {
                        WriteRedirectResponse(context.Response, BuildRedirectLocation(request, newToken));
                    } else {
                        WriteHtmlResponse(context.Response, content, 200);
                    }
                } catch (Exception ex) {
                    PXTrace.WriteError(ex);
                    var content = GetBadRequestPage(collectorToken, $"{ex.Message}:\n{ex.StackTrace}");
                    WriteHtmlResponse(context.Response, content, 400);
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

        private static string GetQueryValue(WebhookRequest request, string key) {
            if (request?.Query != null && request.Query.TryGetValue(key, out var value)) {
                return value.ToString();
            }
            return null;
        }

        private static string GetHeaderValue(WebhookRequest request, string key) {
            if (request?.Headers != null && request.Headers.TryGetValue(key, out var value)) {
                return value.ToString();
            }
            return null;
        }

        private static Uri BuildRequestUri(WebhookRequest request) {
            var scheme = GetHeaderValue(request, "X-Forwarded-Proto")
                ?? GetHeaderValue(request, "X-Forwarded-Scheme")
                ?? "http";
            var host = GetHeaderValue(request, "X-Forwarded-Host")
                ?? GetHeaderValue(request, "Host")
                ?? "localhost";
            var path = GetHeaderValue(request, "X-Original-Path")
                ?? GetHeaderValue(request, "X-Rewrite-Url")
                ?? "/";

            var ub = new UriBuilder(scheme, host) {
                Path = string.IsNullOrEmpty(path) ? "/" : path,
                Query = BuildQueryString(request)
            };
            return ub.Uri;
        }

        private static string BuildRedirectLocation(WebhookRequest request, string newToken) {
            var query = HttpUtility.ParseQueryString(BuildQueryString(request));
            query.Set(TOKEN_PARAM, newToken);
            return "?" + query;
        }

        private static string BuildQueryString(WebhookRequest request) {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (request?.Query != null) {
                foreach (var pair in request.Query) {
                    query[pair.Key] = pair.Value.ToString();
                }
            }
            return query.ToString();
        }

        private static void WriteHtmlResponse(WebhookResponse response, string content, int statusCode) {
            response.StatusCode = statusCode;
            using (var writer = response.CreateTextWriter("text/html")) {
                writer.Write(content ?? string.Empty);
                writer.Flush();
            }
        }

        private static void WriteRedirectResponse(WebhookResponse response, string location) {
            response.StatusCode = 302;
            response.Headers["Location"] = location;
            response.ContentLength = 0;
        }
    }
}

