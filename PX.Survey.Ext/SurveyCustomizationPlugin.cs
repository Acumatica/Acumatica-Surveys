using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Customization;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;

namespace PX.Survey.Ext
{
    public partial class SurveyCustomizationPlugin : CustomizationPlugin
    {

        public override void UpdateDatabase()
        {
            //todo: determine if this is needed may only need OnPublish. Purge when determined not needed 
            PXTrace.WriteInformation("UpdateDatabase() entered");
            WriteLog("UpdateDatabase");
            //calling here causes errors
            InitializeSurveyComponents();
        }

        /*
        //todo: follow up to add the auto code gen portion of this section
        public override void OnPublished()
        {
            //20211005 there is no evidence of this even firing.
            PXTrace.WriteInformation("OnPublished() entered");
            WriteLog("OnPublished");
            //InitializeSurveyComponents();
        }
        */

        /// <summary>
        /// Performs the logic to create a survey component.
        /// </summary>
        /// <param name="surveyComponent"></param>
        public void InitializeSurveyComponent(SurveyComponent surveyComponent)
        {
            try
            {
                WriteLog($"Creating surveyComponent: {surveyComponent.ComponentID}");
                var graph = PXGraph.CreateInstance<SurveyComponentMaint>();
                WriteLog($"graph created");
                var lookupResult = (SurveyComponent)SelectFrom<SurveyComponent>
                    .Where<SurveyComponent.componentID.IsEqual<@P.AsString>>
                    .View.Select(graph,surveyComponent.ComponentID).FirstOrDefault();
                WriteLog($"lookup done");
                if (lookupResult != null) return; //ignore records that already exist.
                WriteLog($"Saving new record");
                //graph.CurrentSUComponent.Cache.Insert(surveyComponent);
                graph.SUComponent.Cache.Insert(surveyComponent);
                WriteLog($"Insert done");
                graph.Actions.PressSave();
                WriteLog($"press save done");
                graph.Clear();
            }
            catch (Exception e)
            {
                WriteLog($"Error: {e.Message}");
                PXTrace.WriteError(e);
                throw;
            }
           
            
        }
    }
}
