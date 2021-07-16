using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        //public static PXCache InstallAnswers(PXGraph graph, object row, List<CSAnswers> destAnswers) {
        //    var helper = new EntityHelper(graph);
        //    var destNoteId = helper.GetEntityNoteID(row);
        //    if (!destNoteId.HasValue) {
        //        throw new PXException(Messages.NoteIDNotFound, row.GetType());
        //    }
        //    var destClassId = GetClassId(graph, row);
        //    var destEntityType = GetEntityTypeFromAttribute(graph, row);
        //    var classAttrs = new CRAttribute.ClassAttributeList();
        //    if (destEntityType != null && destClassId != null) {
        //        classAttrs = CRAttribute.EntityAttributes(destEntityType, destClassId);
        //    }
        //    var destOnlyNames = destAnswers.Select(x => x.AttributeID).Except(classAttrs.Select(x => x.ID)).Distinct().ToList();
        //    foreach (var name in destOnlyNames) {
        //        // searchValue can be either AttributeId or Description
        //        var attributeDefinition = CRAttribute.Attributes[name] ?? CRAttribute.AttributesByDescr[name];
        //        if (attributeDefinition == null) {
        //            throw new PXSetPropertyException(PX.Objects.CR.Messages.AttributeNotValid);
        //        }
        //        // avoid duplicates
        //        if (classAttrs[attributeDefinition.ToString()] == null) {
        //            classAttrs.Add(new CRAttribute.AttributeExt(attributeDefinition, null, false, true));
        //        }
        //    }
        //    var answerCache = graph.Caches[typeof(CSAnswers)];
        //    var cacheIsDirty = answerCache.IsDirty;
        //    var output = new List<CSAnswers>();
        //    var originAnswers = GetCurrentAnswerList(graph, row);
        //    foreach (var destAnswer in destAnswers) {
        //        var attributeId = destAnswer.AttributeID;
        //        var originAttrExt = classAttrs[attributeId];
        //        if (!originAttrExt.IsActive) {
        //            continue;
        //        }
        //        var answer = (CSAnswers)answerCache.CreateInstance();
        //        answer.AttributeID = originAttrExt.ID;
        //        answer.RefNoteID = destNoteId;
        //        answer = (CSAnswers)(answerCache.Insert(answer) ?? answerCache.Locate(answer));
        //        // Start with default value
        //        answer.Value = originAttrExt.DefaultValue;
        //        // Continue with current value if available
        //        if (TryGetOriginAttributeValue(answer, originAnswers, out var originValue)) {
        //            answer.Value = originValue;
        //        }
        //        var newValue = CheckValue(graph, destAnswer, originAttrExt);
        //        // Try with new value if available
        //        if (newValue != null) {
        //            answer.Value = newValue;
        //        }
        //        answer.IsRequired = originAttrExt.Required;
        //        answerCache.Update(answer);
        //        output.Add(answer);
        //    }
        //    answerCache.IsDirty = cacheIsDirty;
        //    output = output.OrderBy(x => classAttrs.Contains(x.AttributeID) ? classAttrs.IndexOf(x.AttributeID) : (x.Order ?? 0)).ThenBy(x => x.AttributeID).ToList();
        //    short attributeOrder = 0;
        //    // Re-order before saving
        //    foreach (var answer in output) {
        //        answer.Order = attributeOrder++;
        //    }
        //    return answerCache;
        //}

        //private static CSAttribute GetAttribute(PXGraph graph, string attributeId) {
        //    return new PXSelect<CSAttribute, 
        //        Where<CSAttribute.attributeID, Equal<Required<CSAttribute.attributeID>>>>(graph).SelectSingle(new object[] { attributeId });
        //}

        public static (Survey survey, SurveyUser user, SurveyCollector answerCollector, SurveyCollector userCollector) GetSurveyAndUser(SurveyMaint graph, string token) {
            SurveyCollector answerCollector;
            SurveyCollector userCollector = null;
            Survey survey;
            SurveyUser user;
            token = token?.Trim();
            if (token.Length <= 15) {
                // Anonymous survey, token is SurveyID
                survey = Survey.PK.Find(graph, token);
                (user, answerCollector) = InsertAnonymous(graph, survey, null);
                token = answerCollector.Token;
            } else {
                answerCollector = SurveyCollector.UK.ByToken.Find(graph, token);
                survey = Survey.PK.Find(graph, answerCollector.SurveyID);
                // If answers must be kept anonymous, then retrieve the anonymous collector of the collector
                if (survey.KeepAnswersAnonymous == true && answerCollector.AnonCollectorID != null) {
                    userCollector = answerCollector;
                    answerCollector = SurveyCollector.UK.ByAnonCollectorID.Find(graph, answerCollector.AnonCollectorID);
                }
                //if (survey.KeepAnswersAnonymous == true && answerCollector.Anonymous != true) {
                //    var anonCollector = SurveyCollector.PK.Find(graph, answerCollector.AnonCollectorID);
                //    userCollector = answerCollector;
                //    if (anonCollector == null) {
                //        // Rare case where AnonCollectorID points to a deleted Collector, should have been cleared.
                //        var (_, anon) = InsertAnonymous(graph, survey, null);
                //        answerCollector.AnonCollectorID = anon?.CollectorID;
                //        graph.Collectors.Update(answerCollector);
                //        answerCollector = anon;
                //    } else {
                //        answerCollector = anonCollector;
                //    }
                //}
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

        public static (SurveyUser user, SurveyCollector coll) InsertAnonymous(SurveyMaint graph, Survey survey, Guid? refNoteID) {
            SurveySetup setup = PXSelect<SurveySetup>.SelectWindowed(graph, 0, 1);
            var contactID = setup.AnonContactID;
            if (contactID == null) {
                throw new PXException("An Anonymous user needs to be setup in the Survey Preferences");
            }
            var user = graph.InsertOrFindUser(survey, contactID, true);
            var collector = graph.DoUpsertCollector(survey, user, refNoteID);
            return (user, collector);
        }

        public static IEnumerable<CSAttributeDetail> GetAttributeDetails(PXGraph graph, string attributeId) {
            return PXSelect<CSAttributeDetail,
                Where<CSAttributeDetail.attributeID, Equal<Required<CSAttributeDetail.attributeID>>,
                And<CSAttributeDetail.disabled, NotEqual<True>>>>.Select(graph, new object[] { attributeId }).FirstTableItems;
        }

        private static string CheckValue(PXGraph graph, CSAnswers question, CRAttribute.AttributeExt originAttrExt) {
            var currentVal = question.Value;
            var controlType = originAttrExt.ControlType;
            var attrID = question.AttributeID;
            switch (controlType) {
                case 1: // Text
                    return currentVal;
                case 2: // Combo
                    var details = GetAttributeDetails(graph, attrID);
                    var detail = details.FirstOrDefault(det => det.ValueID == currentVal);
                    if (detail == null) {

                    }
                    // TODO Check value
                    return currentVal;
                case 6: // Multi Select Combo
                    var multiDetails = GetAttributeDetails(graph, attrID);
                    var currentVals = currentVal.Split(';'); // ??
                    foreach (var val in currentVals) {
                        var multiDetail = multiDetails.FirstOrDefault(det => det.ValueID == val);
                        if (multiDetail == null) {

                        }
                    }
                    // TODO Check values
                    return currentVal;
                case 4: // Checkbox
                    if (bool.TryParse(currentVal, out bool boolVal)) {
                        return Convert.ToInt32(boolVal).ToString(CultureInfo.InvariantCulture);
                    } else if (currentVal == null) {
                        return 0.ToString();
                    }
                    break;
                case 5: // Datetime
                    // TODO Check value
                    return currentVal;
                case 7: // Selector
                    // TODO Check value
                    return currentVal;
            }
            return null;
        }

        //private static bool TryGetOriginAttributeValue(CSAnswers classAnswer, List<CSAnswers> originAnswers, out string originDefault) {
        //    originDefault = null;
        //    if (classAnswer == null || originAnswers == null) {
        //        return false;
        //    }
        //    foreach (var originAttribute in originAnswers) {
        //        if (originAttribute.AttributeID != classAnswer.AttributeID) {
        //            continue;
        //        }
        //        originDefault = originAttribute.Value;
        //        return true;
        //    }
        //    return false;
        //}

        //private static List<CSAnswers> GetCurrentAnswerList(PXGraph graph, object row) {
        //    var helper = new EntityHelper(graph);
        //    var noteId = helper.GetEntityNoteID(row);
        //    return PXSelect<CSAnswers, Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>.Select(graph, noteId).FirstTableItems.ToList();
        //}

        //private static List<CSAnswers> DeleteCurrentAnswerList(PXGraph graph, object row) {
        //    var helper = new EntityHelper(graph);
        //    var noteId = helper.GetEntityNoteID(row);
        //    var cache = graph.Caches[typeof(CSAnswers)];
        //    var answers = PXSelect<CSAnswers, Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>.Select(graph, noteId).FirstTableItems.ToList();
        //    foreach (var answer in answers) {
        //        cache.Delete(answer);
        //    }
        //    return answers;
        //}

        //private static string GetClassId(PXGraph graph, object row) {
        //    var classIdField = GetClassIdField(graph, row);
        //    if (classIdField == null) {
        //        return null;
        //    }
        //    var cache = graph.Caches[row.GetType()];
        //    return cache.GetValueExt(row, classIdField.Name)?.ToString()?.Trim();
        //}

        //private static Type GetClassIdField(PXGraph graph, object row) {
        //    if (row == null) {
        //        return null;
        //    }
        //    var cache = graph.Caches[row.GetType()];
        //    return cache.GetAttributes(row, null).OfType<CRAttributesFieldAttribute>().FirstOrDefault()?.ClassIdField;
        //}

        //private static Type GetEntityTypeFromAttribute(PXGraph graph, object row) {
        //    var classIdField = GetClassIdField(graph, row);
        //    return classIdField?.DeclaringType;
        //}

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
