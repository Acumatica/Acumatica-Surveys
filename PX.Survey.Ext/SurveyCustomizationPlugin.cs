using System;
using System.Collections.Generic;
using System.Linq;
using Customization;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using CSAttribute = PX.CS.CSAttribute;
using CSAttributeDetail = PX.CS.CSAttributeDetail;

namespace PX.Survey.Ext
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable RedundantVerbatimPrefix
    public partial class SurveyCustomizationPlugin : CustomizationPlugin
    {

        public override void UpdateDatabase()
        {
            PXTrace.WriteInformation("UpdateDatabase() entered");
            WriteLog("UpdateDatabase");
            InitializeSurveyComponents();
            InitializeSurveyAttributes();
        }

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
                var lookupResult = (SurveyComponent)SelectFrom<SurveyComponent>
                    .Where<SurveyComponent.componentID.IsEqual<@P.AsString>>
                    .View.Select(graph, surveyComponent.ComponentID).FirstOrDefault();
                if (lookupResult != null) return; //ignore records that already exist.
                //graph.CurrentSUComponent.Cache.Insert(surveyComponent);
                graph.SUComponent.Cache.Insert(surveyComponent);
                graph.Actions.PressSave();
                graph.Clear();
            }
            catch (Exception e)
            {
                WriteLog($"Error: {e.Message}");
                PXTrace.WriteError(e);
                throw;
            }
        }


        private void InitializeSurveyAttribute(CSAttribute attribute, List<CSAttributeDetail> attributeDetails)
        {
            try
            {
                WriteLog($"Creating Survey Attribute: {attribute.AttributeID}");
                var graph = PXGraph.CreateInstance<CSAttributeMaint>();
                WriteLog($"Graph Created");
                var lookupResult = (CSAttribute)SelectFrom<CSAttribute>
                    .Where<CSAttribute.attributeID.IsEqual<@P.AsString>>
                    .View.Select(graph, attribute.AttributeID).FirstOrDefault();
                WriteLog($"Lookup done");
                if (lookupResult != null) return; //ignore records that already exist.
                graph.Attributes.Cache.Insert(attribute);
                WriteLog($"Insert Attribute header");
                graph.AttributeDetails.Cache.Insert(attributeDetails);
                WriteLog($"Insert Attribute detail ");
                graph.Actions.PressSave();
                WriteLog($"Save Pressed ");
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
