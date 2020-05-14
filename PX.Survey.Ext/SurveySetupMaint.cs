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
                        CSAttributeDetail detail = new CSAttributeDetail();

                        #region  COVSYMPTOM                    
                        attribute = attrGraph.Attributes.Search<CSAnswers.attributeID>("COVSYMPTOM");
                        if (attribute == null)
                        {
                            attribute.AttributeID = "COVSYMPTOM";
                            attribute.Description = Messages.COVSYMPTOM;
                            attribute.ControlType = 2;
                            attrGraph.Attributes.Insert(attribute);
                            attrGraph.Actions.PressSave();

                            detail.ValueID = "SymYes";
                            detail.Description = "Yes";
                            detail.SortOrder = 1;
                            attrGraph.AttributeDetails.Insert(detail);
                            detail = null;

                            detail = new CSAttributeDetail();
                            detail.ValueID = "SymNo";
                            detail.Description = "No";
                            detail.SortOrder = 2;
                            attrGraph.AttributeDetails.Insert(detail);
                            attrGraph.Actions.PressSave();
                            detail = null;
                            attrGraph.Clear();
                        }
                        #endregion

                        #region  COVCONTACT
                        attribute = attrGraph.Attributes.Search<CSAnswers.attributeID>("COVCONTACT");
                        if (attribute == null)
                        {
                            attribute.AttributeID = "COVCONTACT";
                            attribute.Description = Messages.COVCONTACT;
                            attribute.ControlType = 2;
                            attrGraph.Attributes.Insert(attribute);
                            attrGraph.Actions.PressSave();

                            detail.ValueID = "CTCNo";
                            detail.Description = "No";
                            detail.SortOrder = 1;
                            attrGraph.AttributeDetails.Insert(detail);
                            detail = null;

                            detail = new CSAttributeDetail();
                            detail.ValueID = "CTCYes";
                            detail.Description = "Yes";
                            detail.SortOrder = 2;
                            attrGraph.AttributeDetails.Insert(detail);
                            attrGraph.Actions.PressSave();
                            attrGraph.Clear();
                            detail = null;
                        }
                        #endregion

                        #region  COVTEMP
                        attribute = attrGraph.Attributes.Search<CSAnswers.attributeID>("COVTEMP");
                        if (attribute == null)
                        {
                            attribute.AttributeID = "COVTEMP";
                            attribute.Description = Messages.COVTEMP;
                            attribute.ControlType = 2;
                            attrGraph.Attributes.Insert(attribute);
                            attrGraph.Actions.PressSave();

                            detail.ValueID = "97";
                            detail.Description = "Normal";
                            detail.SortOrder = 1;
                            attrGraph.AttributeDetails.Insert(detail);
                            detail = null;

                            detail = new CSAttributeDetail();
                            detail.ValueID = "99";
                            detail.Description = "99";
                            detail.SortOrder = 2;
                            attrGraph.AttributeDetails.Insert(detail);
                            detail = null;

                            detail = new CSAttributeDetail();
                            detail.ValueID = "100";
                            detail.Description = "100";
                            detail.SortOrder = 3;
                            attrGraph.AttributeDetails.Insert(detail);
                            detail = null;

                            detail = new CSAttributeDetail();
                            detail.ValueID = "101";
                            detail.Description = "101";
                            detail.SortOrder = 4;
                            attrGraph.AttributeDetails.Insert(detail);
                            detail = null;

                            detail = new CSAttributeDetail();
                            detail.ValueID = "102";
                            detail.Description = "102+";
                            detail.SortOrder = 5;
                            attrGraph.AttributeDetails.Insert(detail);
                            attrGraph.Actions.PressSave();
                            attrGraph.Clear();
                            detail = null;
                        }
                        #endregion

                        #region  COVTRAVEL
                        attribute = attrGraph.Attributes.Search<CSAnswers.attributeID>("COVTRAVEL");
                        if (attribute == null)
                        {
                            attribute.AttributeID = "COVTRAVEL";
                            attribute.Description = Messages.COVTRAVEL;
                            attribute.ControlType = 1;
                            attrGraph.Attributes.Insert(attribute);
                            attrGraph.Actions.PressSave();
                        }
                        #endregion

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

                                //add the attributes created from above
                                short sortOrder = 1;
                                CSAttributeGroup surveyDetails = new CSAttributeGroup();
                                surveyDetails.AttributeID = "COVSYMPTOM";
                                surveyDetails.IsActive = true;
                                surveyDetails.Required = true;
                                surveyDetails.SortOrder = sortOrder;
                                sortOrder++;
                                surveyGraph.Mapping.Insert(surveyDetails);
                                surveyDetails = null;

                                surveyDetails = new CSAttributeGroup();
                                surveyDetails.AttributeID = "COVCONTACT";
                                surveyDetails.IsActive = true;
                                surveyDetails.Required = true;
                                surveyDetails.SortOrder = sortOrder;
                                sortOrder++;
                                surveyGraph.Mapping.Insert(surveyDetails);
                                surveyDetails = null;

                                surveyDetails = new CSAttributeGroup();
                                surveyDetails.AttributeID = "COVTEMP";
                                surveyDetails.IsActive = true;
                                surveyDetails.Required = true;
                                surveyDetails.SortOrder = sortOrder;
                                sortOrder++;
                                surveyGraph.Mapping.Insert(surveyDetails);
                                surveyDetails = null;

                                surveyDetails = new CSAttributeGroup();
                                surveyDetails.AttributeID = "COVTRAVEL";
                                surveyDetails.IsActive = true;
                                surveyDetails.Required = true;
                                surveyDetails.SortOrder = sortOrder;
                                sortOrder++;
                                surveyGraph.Mapping.Insert(surveyDetails);
                                surveyDetails = null;

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
    }
}