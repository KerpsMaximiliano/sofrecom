namespace Sofco.Domain.RequestNoteStates
{
    public enum RequestNoteFileTypes
    {
        Borrador = 1,
        OrdenDeCompra = 2,
        DocumentacionParaProveedor = 3,
        DocumentacionRecibidoConforme = 4,
        Facturas = 5,

    //1 para estado Borrador
    //2 para Orden de Compra (estado Pendiente Aprobación Abastecimiento)
    //3 para Documentación para Proveedor (estado Aprobada)
    //4 para Documentación Recibido Conforme (estado Solicitada a Proveedor)
    //5 para Facturas (estado Recibido Conforme)
    }
}
