﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sofco.Resources {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Common {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Common() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sofco.Resources.Common", typeof(Common).GetTypeInfo().Assembly);
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
        ///   Busca una cadena traducida similar a common.crmGeneralError.
        /// </summary>
        public static string CrmGeneralError {
            get {
                return ResourceManager.GetString("CrmGeneralError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a common.currencyRequired.
        /// </summary>
        public static string CurrencyRequired {
            get {
                return ResourceManager.GetString("CurrencyRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a common.errorSave.
        /// </summary>
        public static string ErrorSave {
            get {
                return ResourceManager.GetString("ErrorSave", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a common.errorSendMail.
        /// </summary>
        public static string ErrorSendMail {
            get {
                return ResourceManager.GetString("ErrorSendMail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a common.exportFileError.
        /// </summary>
        public static string ExportFileError {
            get {
                return ResourceManager.GetString("ExportFileError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a common.fileDeleted.
        /// </summary>
        public static string FileDeleted {
            get {
                return ResourceManager.GetString("FileDeleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a common.fileNotFound.
        /// </summary>
        public static string FileNotFound {
            get {
                return ResourceManager.GetString("FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a common.generalError.
        /// </summary>
        public static string GeneralError {
            get {
                return ResourceManager.GetString("GeneralError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a common.mailSent.
        /// </summary>
        public static string MailSent {
            get {
                return ResourceManager.GetString("MailSent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a common.saveFileError.
        /// </summary>
        public static string SaveFileError {
            get {
                return ResourceManager.GetString("SaveFileError", resourceCulture);
            }
        }
    }
}
