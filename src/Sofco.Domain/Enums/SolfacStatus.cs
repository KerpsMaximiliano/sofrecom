﻿namespace Sofco.Domain.Enums
{
    public enum SolfacStatus
    {
        None,

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
        AmountCashed,

        // Rechazada por la daf
        RejectedByDaf
    }
}
