﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sofco.Resources.es {
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
    public class Functionality {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal Functionality() {
        }
        
        /// <summary>
        ///    Devuelve la instancia de ResourceManager en caché que usa la clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sofco.Resources.es.Functionality", typeof(Functionality).GetTypeInfo().Assembly);
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
        ///    Busca una cadena localizada similar a Funcionalidad creado correctamente.
        /// </summary>
        public static string Created {
            get {
                return ResourceManager.GetString("Created", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Funcionalidad inhabilitada correctamente.
        /// </summary>
        public static string Disabled {
            get {
                return ResourceManager.GetString("Disabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Funcionalidad habilitada correctamente.
        /// </summary>
        public static string Enabled {
            get {
                return ResourceManager.GetString("Enabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Funcionalidad ya asociada al modulo.
        /// </summary>
        public static string ModuleFunctionalityAlreadyCreated {
            get {
                return ResourceManager.GetString("ModuleFunctionalityAlreadyCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Funcionalidad no asociada al modulo.
        /// </summary>
        public static string ModuleFunctionalityAlreadyRemoved {
            get {
                return ResourceManager.GetString("ModuleFunctionalityAlreadyRemoved", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a Funcionalidad no encontrada.
        /// </summary>
        public static string NotFound {
            get {
                return ResourceManager.GetString("NotFound", resourceCulture);
            }
        }
    }
}
