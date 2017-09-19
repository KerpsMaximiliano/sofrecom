namespace Sofco.Model.Enums
{
    public enum SolfacStatus
    {
        // Pendiente de envio
        SendPending,

        // Pendiente revision control de gestion (CDG)
        PendingByManagementControl,

        // Rechazado por control de gestion (CDG)
        ManagementControlRejected,

        // Pendiente de facturacion
        InvoicePending,

        // Facturada
        Invoiced,

        // Cobrada
        AmountCashed
    }
}
