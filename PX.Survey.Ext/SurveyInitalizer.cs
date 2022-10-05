using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Customization;
using PX.Data;
using PX.DbServices.Model.ImportExport;
using PX.Objects.CS;

namespace PX.Survey.Ext
{
    public class SurveyInitalizer : CustomizationPlugin
    {
        protected byte[] ReadFromFile(string fileName)
        {
            var context = File.ReadAllBytes(fileName);
            return context;
        }

        public void ImportFilesFromDirectory(string directory, string graphName)
        {
            var xmlFiles = Directory.GetFiles(directory).Where(p => p.EndsWith(".xml"));
            foreach (var xmlFile in xmlFiles)
            {
                ImportFilesAtName(xmlFile, graphName);
                this.WriteLog($"Imported {Path.GetFileName(xmlFile)} into {graphName}");
            }
        }
        public void ImportFilesAtName(string fileName, string graphName)
        {
            var content = ReadFromFile(fileName);
            Type type = ByName(graphName);
            var graph = PXGraph.CreateInstance(type);
            var item = graph.ImportEntitiesFromXml(content, RecordImportMode.Replace, out var dataUploader);
        }
        public List<Tuple<int, string, string>> TakeDictionaryGraphs(string currentDirectory)
        {
            var dictionary = new List<Tuple<int, string, string>>();
            var directories = Directory.GetDirectories(currentDirectory);
            foreach (var folder in directories)
            {
                dictionary.Add(Tuple.Create(Convert.ToInt32(folder.Split('\\').LastOrDefault().Split('-')[0]),
                    folder.Split('\\').LastOrDefault().Split('-').LastOrDefault(), folder));
            }

            return dictionary;
        }
        private static IEnumerable<string> TryEnumerate(Func<IEnumerable<string>> action)
        {
            try
            {
                return action.Invoke();
            }
            catch
            {
                //TODO logging
                return Enumerable.Empty<string>();
            }
        }
        private static Type ByName(string name)
        {
            foreach (var lib in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                try
                {
                    foreach (var type in lib.GetTypes())
                    {
                        if (type.Name == name)
                            return type;
                    }
                }
                catch // TODO find more efficient workaround. Didn't figure out how to bypass some reflection errors.
                {
                    continue;
                }
            }
            return null;
        }
        public override void UpdateDatabase()
        {
            this.WriteLog("Starting XML import");
            try
            {
                string folderName = $"{AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\')}\\SUContent\\";
                this.WriteLog(folderName);
                var nameOfGraphs = TakeDictionaryGraphs(folderName).OrderBy(p => p.Item1);

                foreach (var item in nameOfGraphs)
                {
                    this.WriteLog(item.Item2 + item.Item3);
                    ImportFilesFromDirectory(item.Item3, item.Item2);
                }

            }
            catch (Exception e)
            {
                this.WriteLog(e.Message);
            }
        }
    }
}