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
    ///    Clase de recurso fuertemente tipada para buscar cadenas localizadas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class MailResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal MailResource() {
        }
        
        /// <summary>
        ///    Devuelve la instancia de ResourceManager en caché que usa la clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sofco.Resources.MailResource", typeof(MailResource).GetTypeInfo().Assembly);
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
        ///    Busca una cadena localizada similar a  &lt;html&gt;&lt;head&gt;&lt;style&gt;
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
        ///		margin-bottom:5px
        ///	}
        ///	.content { margin-bottom:10px }
        ///	.contentResult { margin-left:10px; margin-bottom:10px }
        ///	.footer { margin-top:10px;height:40px;color:#818080;font-size:9pt;text-align:center }
        ///	ul [el resto de la cadena se truncó]&quot;;.
        /// </summary>
        public static string HitosWithoutSolfac {
            get {
                return ResourceManager.GetString("HitosWithoutSolfac", resourceCulture);
            }
        }
    }
}
