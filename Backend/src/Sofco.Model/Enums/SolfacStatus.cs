namespace Sofco.Model.Enums
{
    public enum SolfacStatus
    {
        SendPending,
        PendingByManagementControl,
        ManagementControlRejected,
        PendingReviewByBudgetPlanning,
        BudgetPlanningReviewRejected,
        InvoicePending,
        Invoiced,
        AmountCashed
    }
}
