﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sofco.Resources.es.AllocationManagement {
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
    public class Allocation {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal Allocation() {
        }
        
        /// <summary>
        ///    Devuelve la instancia de ResourceManager en caché que usa la clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sofco.Resources.es.AllocationManagement.Allocation", typeof(Allocation).GetTypeInfo().Assembly);
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
        ///    Busca una cadena localizada similar a allocationManagement/allocation.added.
        /// </summary>
        public static string Added {
            get {
                return ResourceManager.GetString("Added", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a allocationManagement/allocation.billingPercentageExceeded.
        /// </summary>
        public static string BillingPercentageExceeded {
            get {
                return ResourceManager.GetString("BillingPercentageExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a allocationManagement/allocation.cannotBeAssign.
        /// </summary>
        public static string CannotBeAssign {
            get {
                return ResourceManager.GetString("CannotBeAssign", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a allocationManagement/allocation.dateSinceOutOfRange.
        /// </summary>
        public static string DateSinceOutOfRange {
            get {
                return ResourceManager.GetString("DateSinceOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a allocationManagement/allocation.dateSinceRequired.
        /// </summary>
        public static string DateSinceRequired {
            get {
                return ResourceManager.GetString("DateSinceRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a allocationManagement/allocation.dateToLessThanDateSince.
        /// </summary>
        public static string DateToLessThanDateSince {
            get {
                return ResourceManager.GetString("DateToLessThanDateSince", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a allocationManagement/allocation.dateToOutOfRange.
        /// </summary>
        public static string DateToOutOfRange {
            get {
                return ResourceManager.GetString("DateToOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a allocationManagement/allocation.dateToRequired.
        /// </summary>
        public static string DateToRequired {
            get {
                return ResourceManager.GetString("DateToRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a allocationManagement/allocation.releaseDateIsRequired.
        /// </summary>
        public static string ReleaseDateIsRequired {
            get {
                return ResourceManager.GetString("ReleaseDateIsRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Busca una cadena localizada similar a allocationManagement/allocation.wrongPercentage.
        /// </summary>
        public static string WrongPercentage {
            get {
                return ResourceManager.GetString("WrongPercentage", resourceCulture);
            }
        }
    }
}
