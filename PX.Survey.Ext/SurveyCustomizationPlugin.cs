using System;
using System.Collections.Generic;
using System.Linq;
using Customization;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.CT;
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
                var insertResult = (SurveyComponent)graph.SUComponent.Cache.Insert(surveyComponent);
                if (insertResult != null)
                {
                    WriteLog($"surveyComponent: {insertResult.ComponentID} created successfully");
                    graph.Actions.PressSave();
                }
                else
                {
                    WriteLog($"Failed to create: {surveyComponent.ComponentID}");
                }
                graph.Clear();
            }
            catch (Exception e)
            {
                WriteLog($"Error: {e.Message}");
                PXTrace.WriteError(e);
                throw;
            }
        }


        private void InitializeSurveyAttribute(PX.Objects.CS.CSAttribute attribute, List<PX.Objects.CS.CSAttributeDetail> attributeDetails)
        {
            try
            {
                var graph = PXGraph.CreateInstance<CSAttributeMaint>();
                var lookupResult = (PX.Objects.CS.CSAttribute)SelectFrom<PX.Objects.CS.CSAttribute>
                    .Where<PX.Objects.CS.CSAttribute.attributeID.IsEqual<@P.AsString>>
                    .View.Select(graph, attribute.AttributeID).FirstOrDefault();
                if (lookupResult != null) return; //ignore records that already exist.
                var insertResult =  (PX.Objects.CS.CSAttribute)graph.Attributes.Cache.Insert(attribute);
                if (insertResult != null)
                {
                    WriteLog($"survey Attribute: {insertResult.AttributeID} created successfully");
                    graph.Actions.PressSave();
                    foreach(PX.Objects.CS.CSAttributeDetail detail in attributeDetails)
                    {
                        var insertedDetail = (PX.Objects.CS.CSAttributeDetail)graph.AttributeDetails.Cache.Insert(detail);
                        if (insertedDetail != null)
                        {
                            WriteLog($"survey AttributeDetail: {insertedDetail.Description} created successfully");
                        }
                        else
                        {
                            WriteLog($"Failed to create Attribute detail: {detail.Description}");
                        }
                    }
                    graph.Actions.PressSave();
                }
                else
                {
                    WriteLog($"Failed to create: {attribute.AttributeID}");
                }
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
