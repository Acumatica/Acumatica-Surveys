using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace PX.Survey.Ext {

    public static class SurveyUtils {

        public const string PAGE_NBR = "pageNbr";
        public const string QUES_NBR = "questionNbr";
        public const string LINE_NBR = "lineNbr";
        public const string DIGITS = "\\d+";
        public static Regex ANSWER_CODE = new Regex($"(?<{PAGE_NBR}>{DIGITS})\\.(?<{QUES_NBR}>{DIGITS})\\.(?<{LINE_NBR}>{DIGITS})");

        public static Func<SurveyDetail, bool> ALL_PAGES = (pd) => { return pd.PageNbr != null && pd.PageNbr > 0; };
        public static Func<SurveyDetail, bool> ACTIVE_PAGES_ONLY = (pd) => { return ALL_PAGES(pd) && pd.Active == true; };
        public static Func<SurveyDetail, bool> EXCEPT_HF_PAGES = (pd) => { return ALL_PAGES(pd) && pd.ComponentType != SUComponentType.Header && pd.ComponentType != SUComponentType.Footer; };
        public static Func<SurveyDetail, bool> ALL_QUESTIONS = (pd) => { return pd.QuestionNbr != null && pd.QuestionNbr > 0; };
        public static Func<SurveyDetail, bool> ACTIVE_QUESTIONS_ONLY = (pd) => { return ALL_QUESTIONS(pd) && pd.Active == true; };
        public static Func<SurveyDetail, int> GET_PAGE_NBR = (pd) => { return pd.PageNbr.Value; };
        public static Func<SurveyDetail, int> GET_QUES_NBR = (pd) => { return pd.QuestionNbr.Value; };
        public static Func<SurveyDetail, Tuple<int, int>> GET_QUES_AND_PAGE_NBR = (pd) => { return Tuple.Create(pd.QuestionNbr.Value, pd.PageNbr.Value); };

        public class surveyScreen : BqlString.Constant<surveyScreen> {
            public surveyScreen() : base("SU301000") { }
        }

        public static int GetNextOrPrevPageNbr(HttpRequestMessage request, int pageNbr) {
            var body = request.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(body)) {
                return 1;
            }
            var qscoll = HttpUtility.ParseQueryString(body);
            var action = qscoll.Get("action");
            if (string.IsNullOrEmpty(action)) {
                return 1;
            }
            switch (action) {
                case "prev":
                    return --pageNbr;
                case "start":
                    return 1;
            }
            return ++pageNbr;
        }

        public static int GetPageNumber(string page) {
            if (string.IsNullOrEmpty(page) || !int.TryParse(page, out int pageNbr)) {
                return 0;
            }
            return pageNbr;
        }


        public static IEnumerable<SurveyDetail> SelectPages(IEnumerable<SurveyDetail> details, int pageNbr) {
            IEnumerable<SurveyDetail> pages = Enumerable.Empty<SurveyDetail>();
            var max = details.Max(det => det.PageNbr);
            while (!pages.Any() && pageNbr <= max) {
                pages = details.Where(det => det.PageNbr != null && det.Active == true && det.PageNbr == pageNbr).OrderBy(pa => pa.SortOrder.Value).ToArray(); // Force eval, otherwise, we get pages for the next number
                pageNbr++;
            }
            return pages;
        }

        public static void SubmitSurvey(string collectorToken, HttpRequestMessage request, int? pageNbr) {
            var body = request.Content.ReadAsStringAsync().Result;
            var uri = request.RequestUri;
            var props = request.Properties;
            SaveSurveySubmission(collectorToken, body, uri, props, pageNbr);
        }


        private static void SaveSurveySubmission(string token, string payload, Uri uri, IDictionary<string, object> props, int? pageNbr) {
            var graph = PXGraph.CreateInstance<SurveyMaint>();
            var (survey, _, answerCollector, userCollector) = GetSurveyAndUser(graph, token);
            if (survey.Status == SurveyStatus.Preparing && answerCollector.IsTest != true) {
                throw new Exception($"The survey is not opened yet, come back later.");
            }
            if (survey.Status == SurveyStatus.Closed && answerCollector.IsTest != true) {
                throw new Exception($"The survey is closed, you cannot answer anymore.");
            }
            if (survey.AllowAnonymous != true && answerCollector.Anonymous == true && answerCollector == userCollector) {
                throw new Exception($"The survey does not allow anonymous answers");
            }
            if (answerCollector.Status == CollectorStatus.Processed) {
                throw new Exception($"Your answers were processed, you cannot change them anymore");
            }
            var data = FindCollectorData(graph, answerCollector, pageNbr);
            if (data == null) {
                data = new SurveyCollectorData {
                    Token = token,
                    Uri = uri.ToString(),
                    Payload = payload,
                    SurveyID = survey?.SurveyID,
                    CollectorID = answerCollector.CollectorID,
                    PageNbr = pageNbr
                };
                data = graph.CollectorDataRecords.Insert(data);
            } else {
                data.Payload = payload;
                data.Status = CollectorDataStatus.Updated;
                data = graph.CollectorDataRecords.Update(data);
            }
            if (answerCollector.Status == CollectorStatus.Deleted) {
                answerCollector.Status = CollectorStatus.New;
            }
            if (answerCollector.Status == CollectorStatus.New || answerCollector.Status == CollectorStatus.Sent) {
                answerCollector.Status = CollectorStatus.Partially;
            }
            var lastPageNbr = graph.GetLastQuestionPageNumber(survey);
            if (answerCollector.Status == CollectorStatus.Partially && pageNbr >= lastPageNbr) {
                answerCollector.Status = CollectorStatus.Completed;
            }
            graph.Collectors.Update(answerCollector);
            if (userCollector.CollectorID != answerCollector.CollectorID) {
                userCollector.Status = answerCollector.Status;
                graph.Collectors.Update(userCollector);
            }
            if (survey.Status == SurveyStatus.Started) {
                survey.Status = SurveyStatus.InProgress;
                graph.Survey.Update(survey);
            }
            graph.Actions.PressSave();
        }

        private static SurveyCollectorData FindCollectorData(SurveyMaint graph, SurveyCollector collector, int? pageNbr) {
            SurveyCollectorData collData = PXSelect<SurveyCollectorData,
                Where<SurveyCollectorData.token, Equal<Required<SurveyCollectorData.token>>,
                And<SurveyCollectorData.pageNbr, Equal<Required<SurveyCollectorData.pageNbr>>>>>.Select(graph, collector.Token, pageNbr);
            return collData;
        }

        public static (Survey survey, SurveyUser user, SurveyCollector answerCollector, SurveyCollector userCollector) GetSurveyAndUser(SurveyMaint graph, string token) {
            SurveyCollector answerCollector;
            SurveyCollector userCollector = null;
            Survey survey;
            SurveyUser user;
            token = token?.Trim();
            if (token.Length <= 15) {
                // Anonymous survey, token is SurveyID
                survey = Survey.PK.Find(graph, token);
                if (survey == null) {
                    throw new PXException("Cannot find a survey with token {0}", token);
                }
                (user, answerCollector) = InsertAnonymous(graph, survey, null, true, false);
                token = answerCollector.Token;
            } else {
                answerCollector = SurveyCollector.UK.ByToken.Find(graph, token);
                survey = Survey.PK.Find(graph, answerCollector.SurveyID);
                // If answers must be kept anonymous, then retrieve the anonymous collector of the user collector
                if (survey.KeepAnswersAnonymous == true && answerCollector.Anonymous != true) {
                    userCollector = answerCollector;
                    answerCollector = GetAnonymousCollector(graph, survey, userCollector);
                }
                user = SurveyUser.PK.Find(graph, survey.SurveyID, answerCollector.UserLineNbr);
            }
            if (answerCollector == null) {
                throw new PXException(Messages.TokenNoFound, token);
            }
            if (survey == null) {
                throw new PXException(Messages.TokenNoSurvey, token);
            }
            if (user == null) {
                throw new PXException(Messages.TokenNoUser, token);
            }
            if (userCollector == null) {
                userCollector = answerCollector;
            }
            return (survey, user, answerCollector, userCollector);
        }

        private static SurveyCollector GetAnonymousCollector(SurveyMaint graph, Survey survey, SurveyCollector userCollector) {
            var coll = SurveyCollector.PK.Find(graph, userCollector.AnonCollectorID);
            if (coll == null) {
                // Rare case where AnonCollectorID points to a deleted Collector, should have been cleared.
                var (_, anon) = InsertAnonymous(graph, survey, null, true, coll.IsTest == true);
                userCollector.AnonCollectorID = anon?.CollectorID;
                graph.Collectors.Update(userCollector);
                graph.Actions.PressSave();
                coll = anon;
            }
            return coll;
        }

        public static (SurveyUser user, SurveyCollector coll) InsertAnonymous(SurveyMaint graph, Survey survey, Guid? refNoteID, bool saveNow, bool isTest) {
            SurveySetup setup = PXSelect<SurveySetup>.SelectWindowed(graph, 0, 1);
            var contactID = setup.AnonContactID;
            if (contactID == null) {
                throw new PXException("An Anonymous user needs to be setup in the Survey Preferences");
            }
            if (survey.AllowAnonymous != true || survey.KeepAnswersAnonymous != true) {
                throw new PXException("Survey {0} ({1}) does not allow anonymous answers", survey.SurveyID, survey.Title);
            }
            var user = graph.InsertOrFindUser(survey, contactID, true);
            var collector = graph.DoUpsertCollector(survey, user, refNoteID, saveNow, isTest);
            return (user, collector);
        }

        public static IEnumerable<CSAttributeDetail> GetAttributeDetails(PXGraph graph, string attributeId) {
            return PXSelect<CSAttributeDetail,
                Where<CSAttributeDetail.attributeID, Equal<Required<CSAttributeDetail.attributeID>>,
                And<CSAttributeDetail.disabled, NotEqual<True>>>>.Select(graph, new object[] { attributeId }).FirstTableItems;
        }

        public static bool HasLetter(string value) {
            return !string.IsNullOrEmpty(value) && value.Any(char.IsLetter);
        }

        public static bool IsOnlyDigit(string value) {
            return !string.IsNullOrEmpty(value) && value.All(char.IsDigit);
        }

        public static bool HasDigit(string value) {
            return !string.IsNullOrEmpty(value) && value.Any(char.IsDigit);
        }

        public static bool IsGuid(string value) {
            return !string.IsNullOrEmpty(value) && Guid.TryParse(value, out var fieldGuid);
        }

        public static string GetHash(Guid guid) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                string hash = GetHash(sha256Hash, guid.ToString());
                return hash;
            }
        }

        public static bool VerifyHash(Guid guid, string hash) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                var isSame = VerifyHash(sha256Hash, guid.ToString(), hash);
                return isSame;
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input) {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash) {
            // Hash the input.
            var hashOfInput = GetHash(hashAlgorithm, input);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
        }
    }
}
