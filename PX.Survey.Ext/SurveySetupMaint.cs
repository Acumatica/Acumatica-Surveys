using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using System.Collections.Generic;

namespace PX.Survey.Ext
{
    public class SurveySetupMaint : PXGraph<SurveySetupMaint>
    {
        #region views
        public PXSave<SurveySetup> Save;
        public PXCancel<SurveySetup> Cancel;
        public PXSelect<SurveySetup> surveySetup;
        #endregion

        #region actions
        public PXAction<SurveySetup> AddDemoSurvey;
        [PXUIField(DisplayName = "Create Demo Survey", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton()]
        public virtual void addDemoSurvey()
        {
            SurveySetup setup = this.surveySetup.Current;
            if (setup!=null)
            {
                if (setup.DemoSurvey != true)
                {
                    try
                    {
                        //create the attributes
                        CSAttribute attribute = new CSAttribute();
                        CSAttributeMaint attrGraph = PXGraph.CreateInstance<CSAttributeMaint>();
                        short sortOrder = 1;

                        #region  COVSYMPTOM                    
                        attribute = attrGraph.Attributes.Search<CSAnswers.attributeID>("COVSYMPTOM");
                        if (attribute == null)
                        {
                            
                            CreateAttribute(attrGraph, "COVSYMPTOM", Messages.COVSYMPTOM, 2);
                            CreateAttributeDetails(attrGraph, "SymYes", "Yes", sortOrder++);
                            CreateAttributeDetails(attrGraph, "SymNo", "No", sortOrder++);                            
                        }
                        #endregion

                        #region  COVCONTACT
                        attribute = attrGraph.Attributes.Search<CSAnswers.attributeID>("COVCONTACT");
                        if (attribute == null)
                        {                            
                            CreateAttribute(attrGraph, "COVCONTACT", Messages.COVCONTACT, 2);

                            sortOrder = 1;
                            CreateAttributeDetails(attrGraph, "CTCNo", "No", sortOrder++);
                            CreateAttributeDetails(attrGraph, "CTCYes", "Yes", sortOrder++);
                        }
                        #endregion

                        #region  COVTEMP
                        attribute = attrGraph.Attributes.Search<CSAnswers.attributeID>("COVTEMP");
                        if (attribute == null)
                        {
                            CreateAttribute(attrGraph, "COVTEMP", Messages.COVTEMP, 2);

                            sortOrder = 1;
                            CreateAttributeDetails(attrGraph, "97", "Normal", sortOrder++);
                            CreateAttributeDetails(attrGraph, "99", "99", sortOrder++);
                            CreateAttributeDetails(attrGraph, "100", "100", sortOrder++);
                            CreateAttributeDetails(attrGraph, "101", "104", sortOrder++);
                            CreateAttributeDetails(attrGraph, "102", "102+", sortOrder++);                            
                        }
                        #endregion

                        #region  COVTRAVEL
                        attribute = attrGraph.Attributes.Search<CSAnswers.attributeID>("COVTRAVEL");
                        if (attribute == null)
                        {
                            CreateAttribute(attrGraph, "COVTRAVEL", Messages.COVTRAVEL, 1);                            
                        }
                        #endregion

                        attrGraph.Actions.PressSave();
                        attrGraph.Clear();

                        if (!string.IsNullOrEmpty(setup.SurveyNumberingID))
                        {
                            //now create the survey record
                            SurveyMaint surveyGraph = PXGraph.CreateInstance<SurveyMaint>();
                            Survey demoSurvey = surveyGraph.SurveyCurrent.Search<Survey.surveyCD>("DEMOCOVID");
                            if (demoSurvey == null)
                            {
                                demoSurvey = new Survey();
                                demoSurvey.SurveyCD = "DEMOCOVID";
                                demoSurvey.SurveyName = Messages.DEMOCOVID;
                                demoSurvey.Active = true;
                                surveyGraph.SurveyCurrent.Insert(demoSurvey);

                                //add the attributes to the survey created from above
                                sortOrder = 1;
                                AssignSurveyAttributes(surveyGraph, "COVSYMPTOM", sortOrder++);
                                AssignSurveyAttributes(surveyGraph, "COVCONTACT", sortOrder++);
                                AssignSurveyAttributes(surveyGraph, "COVTEMP", sortOrder++);
                                AssignSurveyAttributes(surveyGraph, "COVTRAVEL", sortOrder++);                                

                                surveyGraph.Actions.PressSave();
                                surveyGraph.Clear();
                            }
                        }

                        setup.DemoSurvey = true;
                        this.surveySetup.Update(setup);
                        this.Persist();
                    }
                    catch (PXException ex)
                    {
                        Clear();
                        throw ex;
                    }
                }
            }
        }


        #endregion

        #region events
        protected virtual void _(Events.RowSelected<SurveySetup> e)
        {
            SurveySetup setup = e.Row;
            if (setup == null) { return; }

            if (setup.DemoSurvey == true)
            {
                this.AddDemoSurvey.SetEnabled(false);
            }
        }

        #endregion

        #region methods

        private void CreateAttribute(CSAttributeMaint attrGraph, string attributeID, string description, int controlType)
        {
            CSAttribute attribute = new CSAttribute();
            attribute.AttributeID = attributeID;
            attribute.Description = description;
            attribute.ControlType = controlType;
            attrGraph.Attributes.Insert(attribute);
        }

        private void CreateAttributeDetails(CSAttributeMaint attrGraph, string valueID, string description, short sortOrder)
        {
            CSAttributeDetail detail = new CSAttributeDetail();
            detail.ValueID = valueID;
            detail.Description = description;
            detail.SortOrder = sortOrder;
            attrGraph.AttributeDetails.Insert(detail);
        }

        private void AssignSurveyAttributes(SurveyMaint surveyGraph, string attributeID, short sortOrder, bool required = true)
        {
            CSAttributeGroup surveyDetails = new CSAttributeGroup();
            surveyDetails.AttributeID = attributeID;
            surveyDetails.Required = required;
            surveyDetails.SortOrder = sortOrder;            
            surveyGraph.Mapping.Insert(surveyDetails);
        }

        #endregion
    }
}