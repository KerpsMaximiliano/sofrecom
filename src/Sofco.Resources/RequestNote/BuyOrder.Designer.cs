﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sofco.Resources.RequestNote {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class BuyOrder {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal BuyOrder() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sofco.Resources.RequestNote.BuyOrder", typeof(BuyOrder).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
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
        ///   Busca una cadena traducida similar a requestNote/buyOrder.addSuccess.
        /// </summary>
        public static string AddSuccess {
            get {
                return ResourceManager.GetString("AddSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a requestNote/buyOrder.fileUpload.
        /// </summary>
        public static string FileUpload {
            get {
                return ResourceManager.GetString("FileUpload", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a requestNote/buyOrder.invoiceRequired.
        /// </summary>
        public static string InvoiceRequired {
            get {
                return ResourceManager.GetString("InvoiceRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a requestNote/buyOrder.notAllowed.
        /// </summary>
        public static string NotAllowed {
            get {
                return ResourceManager.GetString("NotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a requestNote/buyOrder.notFound.
        /// </summary>
        public static string NotFound {
            get {
                return ResourceManager.GetString("NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a requestNote/buyOrder.updateSuccess.
        /// </summary>
        public static string UpdateSuccess {
            get {
                return ResourceManager.GetString("UpdateSuccess", resourceCulture);
            }
        }
    }
}
