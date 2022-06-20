﻿namespace Sofco.Domain.RequestNoteStates
{
    public enum RequestNoteStates
    {
        Borrador = 1,
        PendienteRevisiónAbastecimiento = 2,
        PendienteAprobaciónGerentesAnalítica = 3,
        PendienteAprobaciónAbastecimiento = 4,
        PendienteAprobaciónDAF = 5,
        Aprobada = 6,
        SolicitadaAProveedor = 7,
        RecibidoConforme = 8,
        FacturaPendienteAprobaciónGerente=9,
        PendienteProcesarGAF= 10,
        Reachazada=11,
        Cerrada=12
    }
}
