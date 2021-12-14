//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using PX.Data;
//using Customization;
//using System.IO;
//using PX.Objects.IN;
//using PX.Data.BQL.Fluent;
//using System.Globalization;
//using PX.DbServices.Model.ImportExport;
//using System.Web.Hosting;

//namespace PX.Survey.Ext {

//    public class SurveyDataLoader : CustomizationPlugin {

//        //This method executed after customization was published and website was restarted.  
//        public override void UpdateDatabase() {
//            var compGraph = PXGraph.CreateInstance<SurveyComponentMaint>();
//            SurveyComponent comp = SelectFrom<SurveyComponent>.View.ReadOnly.Select(compGraph);
//            if (comp == null) {
//                //FileInfo item = PXContext.SessionTyped<PXSessionStatePXData>().FileInfo["XmlUploadEntity"];
//                try {
//                    strs = compGraph.ImportEntitiesFromXml(item.BinData, RecordImportMode.Replace, out var dataUploader);
//                } catch (InvalidImportOperationException invalidImportOperationException1) {
//                    InvalidImportOperationException invalidImportOperationException = invalidImportOperationException1;
//                    throw new PXException(PXLocalizer.LocalizeFormat("Impossible to import selected file. Records in the file and in the database table {0} don't match.", new object[] { invalidImportOperationException.TableName }));
//                }
//                //using (StreamReader file = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "InputData\\RSSVRepairService.csv")) {
//                //    string header = file.ReadLine();
//                //    if (header != null) {
//                //        string[] headerParts = header.Split(';');
//                //        while (true) {
//                //            string line = file.ReadLine();
//                //            if (line != null) {
//                //                string[] lineParts = line.Split(';');
//                //                IDictionary<string, string> dic = headerParts.Select((k, i) => new { k, v = lineParts[i] }).ToDictionary(x => x.k, x => x.v);
//                //                RSSVRepairService service = new RSSVRepairService {
//                //                    ServiceCD = dic["ServiceCD"],
//                //                    Description = dic["Description"],
//                //                    Active = Convert.ToBoolean(Convert.ToInt32(dic["Active"])),
//                //                    WalkInService = Convert.ToBoolean(Convert.ToInt32(dic["WalkInService"])),
//                //                    Prepayment = Convert.ToBoolean(Convert.ToInt32(dic["Prepayment"])),
//                //                    PreliminaryCheck = Convert.ToBoolean(Convert.ToInt32(dic["PreliminaryCheck"]))
//                //                };
//                //                repairServiceGraph.RepairService.Insert(service);
//                //                repairServiceGraph.Actions.PressSave();
//                //                repairServiceGraph.Clear();
//                //            } else break;
//                //        }
//                //    }
//                    this.WriteLog("RSSVRepairService updated");
//                }
//            }
//        }

//        public static ExportTemplate GetExportTemplateForType(Type tGraph) {
//            PXSiteMapNode pXSiteMapNode = PXSiteMap.Provider.FindSiteMapNodeUnsecure(tGraph);
//            if (pXSiteMapNode == null) {
//                return null;
//            }
//            return PXCopyPasteAction<TNode>.GetExportTemplateForScreen(pXSiteMapNode.ScreenID);
//        }

//        private static string getXmlExportRulesFileForScreen(string screenId) {
//            return HostingEnvironment.MapPath(string.Concat("~/App_Data/XmlExportDefinitions/", screenId, ".xml"));
//        }

//        private static IEnumerable<SM.RowWesiteFile> GetFiles() {
//            bool flag;
//            string applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;
//            applicationPhysicalPath = applicationPhysicalPath.TrimEnd(new char[] { '\\' });
//            //Dictionary<string, string> dictionary = WebsiteFileSelector.GetOriginalFiles().ToDictionary<string, string>((string _) => _, StringComparer.OrdinalIgnoreCase);
//            //bool flag1 = dictionary.Any<KeyValuePair<string, string>>();
//            string[] files = Directory.GetFiles(applicationPhysicalPath, "*", SearchOption.AllDirectories);
//            List<SM.RowWesiteFile> rowWesiteFiles = new List<SM.RowWesiteFile>();
//            string[] strArrays = files;
//            for (int i = 0; i < (int)strArrays.Length; i++) {
//                string str = strArrays[i];
//                string str1 = str.Substring(applicationPhysicalPath.Length + 1);
//                if (!flag1) {
//                    flag = true;
//                } else {
//                    flag = (!flag1 ? false : !dictionary.ContainsKey(str1));
//                }
//                if (flag) {
//                    FileInfo fileInfo = new FileInfo(str);
//                    rowWesiteFiles.Add(new SM.RowWesiteFile() {
//                        Path = str1,
//                        Size = new int?((int)fileInfo.Length),
//                        Modified = new DateTime?(fileInfo.LastWriteTime)
//                    });
//                }
//            }
//            return rowWesiteFiles;
//        }
//    }
//}