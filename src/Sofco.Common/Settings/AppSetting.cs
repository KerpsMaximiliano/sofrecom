namespace Sofco.Common.Settings
{
    public class AppSetting
    {
        public string Domain { get; set; }

        public string SolfacGeneratorCode { get; set; }

        public string WorkTimeApprovalCode { get; set; }

        public string PurchaseOrderApprovalCode { get; set; }

        public string PurchaseOrderComplianceApprovalCode { get; set; }

        public string PurchaseOrderCommercialApprovalCode { get; set; }

        public string PurchaseOrderOperationApprovalCode { get; set; }

        public string PurchaseOrderDafApprovalCode { get; set; }

        public string DafPurchaseOrderGroupCode { get; set; }

        public string PurchaseOrderActiveViewCode { get; set; }

        public string PurchaseOrderActiveDelegateCode { get; set; }

        public string LicenseViewDelegateCode { get; set; }

        public string LicenseViewCode { get; set; }

        public string AnalyticBank { get; set; }

        public int WorkflowStatusDraft { get; set; }

        public int AdvacementWorkflowId { get; set; }

        public string SectorUserSource { get; set; }

        public string ManagerUserSource { get; set; }

        public string ApplicantUserSource { get; set; }

        public string GroupUserSource { get; set; }

        public string UserUserSource { get; set; }

        public int CurrencyPesos { get; set; }

        public int CurrencyDolares { get; set; }

        public int CurrencyEuros { get; set; }
    }
}
