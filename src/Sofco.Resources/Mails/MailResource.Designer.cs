﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sofco.Resources.Mails {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///    A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class MailResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal MailResource() {
        }
        
        /// <summary>
        ///    Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sofco.Resources.Mails.MailResource", typeof(MailResource).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///    Overrides the current thread's CurrentUICulture property for all
        ///    resource lookups using this strongly typed resource class.
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
        ///    Looks up a localized string similar to {message}&lt;br&gt;&lt;br&gt;.
        /// </summary>
        public static string Default {
            get {
                return ResourceManager.GetString("Default", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to     RRHH ha confirmado la baja del siguiente recurso: &lt;br/&gt;&lt;br/&gt;
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
        ///    Looks up a localized string similar to 
        ///Se han detectado Hitos sin Solfac que requiere revisión:&lt;br&gt;&lt;br&gt;
        ///&lt;/div&gt;
        ///&lt;div class=&quot;contentResult&quot;&gt;{content}.
        /// </summary>
        public static string HitosWithoutSolfac {
            get {
                return ResourceManager.GetString("HitosWithoutSolfac", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to  &lt;html&gt;&lt;head&gt;&lt;style&gt;
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
        ///	u [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Template {
            get {
                return ResourceManager.GetString("Template", resourceCulture);
            }
        }
    }
}
