﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sofco.Resources.es.Billing {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///    Clase de recurso fuertemente tipada para buscar cadenas localizadas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Invoice {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal Invoice() {
        }
        
        /// <summary>
        ///    Devuelve la instancia de ResourceManager en caché que usa la clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sofco.Resources.es.Billing.Invoice", typeof(Invoice).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///    Reemplaza la propiedad CurrentUICulture del subproceso actual para todas
        ///    las búsquedas de recursos que usan esta clase de recursos fuertemente tipada.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Remito aprobado exitosamente.
        /// </summary>
        public static string Approved {
            get {
                return ResourceManager.GetString("Approved", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Remito anulado exitosamente.
        /// </summary>
        public static string Cancelled {
            get {
                return ResourceManager.GetString("Cancelled", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a No se puede aprobar el remito en el estado actual.
        /// </summary>
        public static string CannotApprove {
            get {
                return ResourceManager.GetString("CannotApprove", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Solo se pueden eliminar los remitos que esten pendientes de envío.
        /// </summary>
        public static string CannotDelete {
            get {
                return ResourceManager.GetString("CannotDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a No se puede rechazar el remito en el estado actual.
        /// </summary>
        public static string CannotReject {
            get {
                return ResourceManager.GetString("CannotReject", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a No se puede enviar a la DAF en el estado actual del remito.
        /// </summary>
        public static string CannotSendToDaf {
            get {
                return ResourceManager.GetString("CannotSendToDaf", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Remito eliminado exitosamente.
        /// </summary>
        public static string Deleted {
            get {
                return ResourceManager.GetString("Deleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a La descripción del detalle es requerida.
        /// </summary>
        public static string DescriptionRequired {
            get {
                return ResourceManager.GetString("DescriptionRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Debe incluir al menos un detalle.
        /// </summary>
        public static string DetailsRequired {
            get {
                return ResourceManager.GetString("DetailsRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Excel guardo exitosamente.
        /// </summary>
        public static string ExcelUpload {
            get {
                return ResourceManager.GetString("ExcelUpload", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Remito generado exitosamente.
        /// </summary>
        public static string InvoiceCreated {
            get {
                return ResourceManager.GetString("InvoiceCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Número de remito es requerido.
        /// </summary>
        public static string InvoiceNumerRequired {
            get {
                return ResourceManager.GetString("InvoiceNumerRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Se necesita adjuntar el remito en formato excel antes de ser enviado a la DAF.
        /// </summary>
        public static string NeedExcelToSend {
            get {
                return ResourceManager.GetString("NeedExcelToSend", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Se necesita adjuntar el remito en formato pdf antes de ser aprobado.
        /// </summary>
        public static string NeedPdfToApprove {
            get {
                return ResourceManager.GetString("NeedPdfToApprove", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Remito no encontrado.
        /// </summary>
        public static string NotFound {
            get {
                return ResourceManager.GetString("NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a PDF guardo exitosamente.
        /// </summary>
        public static string PdfUpload {
            get {
                return ResourceManager.GetString("PdfUpload", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a La cantidad del detalle es requerida.
        /// </summary>
        public static string QuantityRequired {
            get {
                return ResourceManager.GetString("QuantityRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Remito rechazado exitosamente.
        /// </summary>
        public static string Reject {
            get {
                return ResourceManager.GetString("Reject", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Remito enviado correctamente a la DAF.
        /// </summary>
        public static string SentToDaf {
            get {
                return ResourceManager.GetString("SentToDaf", resourceCulture);
            }
        }
    }
}
