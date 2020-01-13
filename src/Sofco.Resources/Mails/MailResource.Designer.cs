﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sofco.Resources.Mails {
    using System;
    
    
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
    public class MailResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MailResource() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sofco.Resources.Mails.MailResource", typeof(MailResource).Assembly);
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
        ///   Busca una cadena traducida similar a {message}&lt;br&gt;&lt;br&gt;.
        /// </summary>
        public static string Default {
            get {
                return ResourceManager.GetString("Default", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a     RRHH ha confirmado la baja del siguiente recurso: &lt;br/&gt;&lt;br/&gt;
        ///    Legajo: {employeeNumber}&lt;br/&gt;
        ///    Nombre: {name}&lt;br/&gt;
        ///    Fecha de baja: {endDate} &lt;br/&gt;&lt;br/&gt;.
        /// </summary>
        public static string EmployeeEndConfirmation {
            get {
                return ResourceManager.GetString("EmployeeEndConfirmation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Se han detectado Hitos sin Solfac que requiere revisión:&lt;br&gt;&lt;br&gt;
        ///&lt;/div&gt;
        ///&lt;div class=&quot;contentResult&quot;&gt;{content}.
        /// </summary>
        public static string HitosWithoutSolfac {
            get {
                return ResourceManager.GetString("HitosWithoutSolfac", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a &lt;html&gt;&lt;head&gt;&lt;style&gt;
        ///	body { font-family:Sans-serif;font-size:10pt; }
        ///	.panel {
        ///		border-top:solid 2px #edf0f3;
        ///		border-left:solid 2px #e7ebef;
        ///		border-right:solid 1px #919498;
        ///		border-bottom:solid 1px #8A8A8A;
        ///		padding:10px;
        ///	}
        ///	.title {
        ///		font-size:12pt; font-weight:bold; color:#242F89;
        ///		margin-bottom:10px
        ///	}
        ///	.content { margin-bottom:10px }
        ///	.contentResult { margin-left:10px; margin-bottom:10px }
        ///	.footer { margin-top:10px;height:40px;color:#818080;font-size:9pt;text-align:center }
        ///	u [resto de la cadena truncado]&quot;;.
        /// </summary>
        public static string Template {
            get {
                return ResourceManager.GetString("Template", resourceCulture);
            }
        }
    }
}
