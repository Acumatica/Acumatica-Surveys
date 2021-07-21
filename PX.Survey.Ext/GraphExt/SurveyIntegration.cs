namespace PX.Survey.Ext {
    public static class SurveyIntegration {

        // Objects.AR integrations
        public class CustomerMaintExt : AbstractSurveyHandlerExt<Objects.AR.CustomerMaint, Objects.AR.Customer> {
            public static bool IsActive() => true;
        }

        // Objects.AP integrations
        public class VendorMaintExt : AbstractSurveyHandlerExt<Objects.AP.VendorMaint, Objects.AP.Vendor> {
            public static bool IsActive() => true;
        }

        // Objects.CR integrations
        public class CampaignMaintExt : AbstractSurveyHandlerExt<Objects.CR.CampaignMaint, Objects.CR.CRCampaign> {
            public static bool IsActive() => true;
        }
        public class CRActivityMaintExt : AbstractSurveyHandlerExt<Objects.EP.CRActivityMaint, Objects.CR.CRActivity> {
            public static bool IsActive() => true;
        }
        public class CRCaseMaintExt : AbstractSurveyHandlerExt<Objects.CR.CRCaseMaint, Objects.CR.CRCase> {
            public static bool IsActive() => true;
        }
        public class CRTaskMaintExt : AbstractSurveyHandlerExt<Objects.CR.CRTaskMaint, Objects.CR.CRActivity> {
            public static bool IsActive() => true;
        }
        public class LeadMaintExt : AbstractSurveyHandlerExt<Objects.CR.LeadMaint, Objects.CR.CRLead> {
            public static bool IsActive() => true;
        }
        public class OpportunityMaintExt : AbstractSurveyHandlerExt<Objects.CR.OpportunityMaint, Objects.CR.CROpportunity> {
            public static bool IsActive() => true;
        }
        public class QuoteMaintExt : AbstractSurveyHandlerExt<Objects.CR.QuoteMaint, Objects.CR.CRQuote> {
            public static bool IsActive() => true;
        }

        // Objects.EP integrations
        public class EPEventMaintExt : AbstractSurveyHandlerExt<Objects.EP.EPEventMaint, Objects.CR.CRActivity> {
            public static bool IsActive() => true;
        }
        public class EmployeeMaintExt : AbstractSurveyHandlerExt<Objects.EP.EmployeeMaint, Objects.EP.EPEmployee> {
            public static bool IsActive() => true;
        }

        // Objects.PO integrations
        public class POOrderEntryExt : AbstractSurveyHandlerExt<Objects.PO.POOrderEntry, Objects.PO.POOrder> {
            public static bool IsActive() => true;
        }
        public class POReceiptEntryExt : AbstractSurveyHandlerExt<Objects.PO.POReceiptEntry, Objects.PO.POReceipt> {
            public static bool IsActive() => true;
        }

        // Objects.SO integrations
        public class SOOrderEntryExt : AbstractSurveyHandlerExt<Objects.SO.SOOrderEntry, Objects.SO.SOOrder> {
            public static bool IsActive() => true;
        }
        public class SOShipmentEntryExt : AbstractSurveyHandlerExt<Objects.SO.SOShipmentEntry, Objects.SO.SOShipment> {
            public static bool IsActive() => true;
        }

        // Objects.FS integrations

        // Objects.PM integrations
        public class PMQuoteMaintExt : AbstractSurveyHandlerExt<Objects.PM.PMQuoteMaint, Objects.PM.PMQuote> {
            public static bool IsActive() => true;
        }

        // Objects.PJ integrations
        public class ProjectIssueMaintExt : AbstractSurveyHandlerExt<Objects.PJ.ProjectsIssue.PJ.Graphs.ProjectIssueMaint, Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue> {
            public static bool IsActive() => true;
        }
        public class SubmittalEntryExt : AbstractSurveyHandlerExt<Objects.PJ.Submittals.PJ.Graphs.SubmittalEntry, Objects.PJ.Submittals.PJ.DAC.PJSubmittal> {
            public static bool IsActive() => true;
        }
        public class RequestForInformationMaintExt : AbstractSurveyHandlerExt<Objects.PJ.RequestsForInformation.PJ.Graphs.RequestForInformationMaint, Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation> {
            public static bool IsActive() => true;
        }
    }
}
