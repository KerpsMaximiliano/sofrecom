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
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class MailMessageResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MailMessageResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sofco.Resources.Mails.MailMessageResource", typeof(MailMessageResource).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to Estimados, &lt;/br&gt;&lt;/br&gt; Se dio de alta la analítica &lt;strong&gt;{0}&lt;/strong&gt;, puede acceder a la misma desde el siguiente &lt;a href=&apos;{1}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;&lt;/br&gt;&lt;/br&gt; Saludos.
        /// </summary>
        public static string AddAnalytic {
            get {
                return ResourceManager.GetString("AddAnalytic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimados, &lt;/br&gt;&lt;/br&gt;Se ha dado el cierre de la analítica &lt;strong&gt;{0}&lt;/strong&gt;, del servicio &lt;strong&gt;{1}&lt;/strong&gt;, puede acceder a la misma desde el siguiente &lt;a href=&apos;{2}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt; &lt;/br&gt;&lt;/br&gt;
        ///Saludos.
        /// </summary>
        public static string CloseAnalytic {
            get {
                return ResourceManager.GetString("CloseAnalytic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimados, &lt;/br&gt;&lt;/br&gt; Se procedió al Cierre para Costos de la analítica &lt;strong&gt;{0}&lt;/strong&gt;, del servicio &lt;strong&gt;{1}&lt;/strong&gt;, puede acceder a la misma desde el siguiente &lt;a href=&apos;{2}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt; &lt;/br&gt;&lt;/br&gt;
        ///Saludos.
        /// </summary>
        public static string CloseForExpensesAnalytic {
            get {
                return ResourceManager.GetString("CloseForExpensesAnalytic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to El recurso &lt;strong&gt;{0}&lt;/strong&gt; ha informado a &lt;strong&gt;{1}&lt;/strong&gt; la solicitud de desvinculación de la empresa, a partir del dia {2} &lt;br/&gt;&lt;br/&gt;.
        /// </summary>
        public static string EmployeeEndNotification {
            get {
                return ResourceManager.GetString("EmployeeEndNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Existen novedades que necesitan su confirmacion, por favor acceder a la lista a traves del siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;&lt;br/&gt;&lt;br/&gt;.
        /// </summary>
        public static string EmployeeNews {
            get {
                return ResourceManager.GetString("EmployeeNews", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Se solicita la anulación de los siguientes remitos: &lt;br/&gt;
        ///{0}
        /// &lt;br/&gt; &lt;br/&gt;
        ///Motivo:  &lt;br/&gt;
        ///{1}.
        /// </summary>
        public static string InvoiceRequestAnnulment {
            get {
                return ResourceManager.GetString("InvoiceRequestAnnulment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimado, &lt;br/&gt;&lt;br/&gt;
        ///El REMITO del asunto se encuentra GENERADO. Para acceder, por favor ingresar al siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. &lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias.
        /// </summary>
        public static string InvoiceStatusApproveMessage {
            get {
                return ResourceManager.GetString("InvoiceStatusApproveMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimado, &lt;br/&gt;&lt;br/&gt;
        ///El REMITO del asunto ha sido RECHAZADO por la DAF, por el siguiente motivo: &lt;br/&gt;
        ///*
        ///&lt;br/&gt;
        ///Por favor ingresar en el siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt; para modificar el formulario y enviar nuevamente. &lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string InvoiceStatusRejectMessage {
            get {
                return ResourceManager.GetString("InvoiceStatusRejectMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimados, &lt;br/&gt;&lt;br/&gt;
        ///Se ha cargado un REMITO que requiere revisión y generación (pdf). &lt;br/&gt;
        ///*
        ///Para imprimirlo, utilice el documento anexado al registro. &lt;br/&gt;
        ///Una vez generado el pdf, por favor importarlo en el siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. &lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string InvoiceStatusSentMessage {
            get {
                return ResourceManager.GetString("InvoiceStatusSentMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimado, &lt;br/&gt;&lt;br/&gt;
        ///Se ha iniciado el proceso de generación del remito del asunto. Para acceder al mismo, por favor ingresar al siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. &lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string InvoiceStatusSentMessageToUser {
            get {
                return ResourceManager.GetString("InvoiceStatusSentMessageToUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tipo Licencia: {1}&lt;br/&gt;&lt;br/&gt;
        ///RRHH, Autorizador, Recurso: &lt;br/&gt;
        ///Se ha &lt;strong&gt; Aprobado &lt;/strong&gt; la siguiente solicitud de licencia &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;..
        /// </summary>
        public static string LicenseApproveMessage {
            get {
                return ResourceManager.GetString("LicenseApproveMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tipo Licencia: {1}&lt;/br&gt;&lt;/br&gt;
        ///RRHH: &lt;/br&gt;
        ///Se ha &lt;strong&gt; Aprobado &lt;/strong&gt; la siguiente solicitud de licencia &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. Corroborar la documentación necesaria para aprobar la misma.
        ///&lt;/br&gt;&lt;/br&gt;
        ///Recurso: &lt;/br&gt;
        ///La licencia solicitada ha sido &lt;strong&gt; Aprobada &lt;/strong&gt; por su superior, falta aún la aprobación de RRHH, si necesita adjuntar documentación a la misma, realícelo desde el siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;, recuerde presentar el original a RRHH..
        /// </summary>
        public static string LicenseApprovePendingMessage {
            get {
                return ResourceManager.GetString("LicenseApprovePendingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tipo Licencia: {2}&lt;/br&gt;&lt;/br&gt;
        ///RRHH: &lt;/br&gt;
        ///Se ha iniciado el proceso de aprobación de la siguiente solicitud de licencia &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;.&lt;/br&gt;&lt;/br&gt;
        ///Autorizador: &lt;/br&gt;
        ///El recurso: {1}, ha iniciado un proceso de autorización de licencia. Para autorizarla ingrese al siguiente  &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;..
        /// </summary>
        public static string LicenseAuthPendingMessage {
            get {
                return ResourceManager.GetString("LicenseAuthPendingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tipo Licencia: {2}&lt;/br&gt;&lt;/br&gt;
        ///RRHH: &lt;/br&gt;
        ///Se ha &lt;strong&gt; Cancelado&lt;/strong&gt; la siguiente solicitud de licencia &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;.
        ///Motivo: {1}
        ///&lt;/br&gt;&lt;/br&gt;
        ///Recurso: &lt;/br&gt;
        ///La licencia solicitada ha sido &lt;strong&gt; Cancelada&lt;/strong&gt;. Acceso a la licencia desde este &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;.
        ///Motivo: {1}.
        /// </summary>
        public static string LicenseCancelledMessage {
            get {
                return ResourceManager.GetString("LicenseCancelledMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tipo Licencia: {1}&lt;/br&gt;&lt;/br&gt;
        ///RRHH: &lt;/br&gt;
        ///Se ha &lt;strong&gt; Autorizado&lt;/strong&gt; la siguiente solicitud de licencia &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. Corroborar la documentación necesaria para aprobar la misma.
        ///&lt;/br&gt;&lt;/br&gt;
        ///Recurso: &lt;/br&gt;
        ///La licencia solicitada ha sido &lt;strong&gt; Autorizada&lt;/strong&gt; por su superior, falta aún la aprobación de RRHH, si necesita adjuntar documentación a la misma, realícelo desde el siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;..
        /// </summary>
        public static string LicensePendingMessage {
            get {
                return ResourceManager.GetString("LicensePendingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tipo Licencia: {2}&lt;br/&gt;&lt;br/&gt;
        ///RRHH: &lt;br/&gt;
        ///Se ha &lt;strong&gt; Rechazado&lt;/strong&gt; la siguiente solicitud de licencia &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;.
        ///Motivo: {1}
        ///&lt;br/&gt;&lt;br/&gt;
        ///Recurso: &lt;br/&gt;
        ///La licencia solicitada ha sido &lt;strong&gt; Rechazada&lt;/strong&gt;. Acceso a la licencia desde este &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;.
        ///Motivo: {1}.
        /// </summary>
        public static string LicenseRejectMessage {
            get {
                return ResourceManager.GetString("LicenseRejectMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Se informa que la orden de compra {0} ha sido aprobada por el area de &lt;strong&gt;Compliance&lt;/strong&gt; y su nuevo estado es &lt;strong&gt;Pendiente Aprobación Operativa&lt;/strong&gt;
        ///&lt;br/&gt;&lt;br/&gt; 
        ///Analiticas relacionadas:
        ///&lt;br/&gt;&lt;br/&gt; 
        ///{2}
        ///&lt;br/&gt; 
        ///Para acceder a la misma, por favor ingresar al siguiente &lt;a href=&apos;{1}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. 
        ///&lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string OcComercialMessage {
            get {
                return ResourceManager.GetString("OcComercialMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Se informa que la orden de compra {0} ha sido aprobada por el area de &lt;strong&gt;Compliance&lt;/strong&gt; y su nuevo estado es &lt;strong&gt;Pendiente Aprobación Comercial&lt;/strong&gt;
        ///&lt;br/&gt;&lt;br/&gt; 
        ///Analiticas relacionadas:
        ///&lt;br/&gt;&lt;br/&gt; 
        ///{2}
        ///&lt;br/&gt; 
        ///Para acceder a la misma, por favor ingresar al siguiente &lt;a href=&apos;{1}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. 
        ///&lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string OcComplianceMessage {
            get {
                return ResourceManager.GetString("OcComplianceMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Se informa que la orden de compra {0} ha terminado el proceso de aprobación. Ya se encuentra en estado &lt;strong&gt;Vigente&lt;/strong&gt; para su uso.
        ///&lt;br/&gt;&lt;br/&gt; 
        ///Analiticas relacionadas:
        ///&lt;br/&gt;&lt;br/&gt; 
        ///{2}
        ///&lt;br/&gt; 
        ///Para acceder a la misma, por favor ingresar al siguiente &lt;a href=&apos;{1}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. 
        ///&lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string OcDafMessage {
            get {
                return ResourceManager.GetString("OcDafMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Se informa que la orden de compra {0} ha sido enviada al area de &lt;strong&gt;Compliance&lt;/strong&gt;. 
        ///&lt;br/&gt;&lt;br/&gt; 
        ///Analiticas relacionadas:
        ///&lt;br/&gt;&lt;br/&gt; 
        ///{2}
        ///&lt;br/&gt; 
        ///Para acceder a la misma, por favor ingresar al siguiente &lt;a href=&apos;{1}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. 
        ///&lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string OcDraftMessage {
            get {
                return ResourceManager.GetString("OcDraftMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Se informa que la orden de compra {0} ha sido aprobada por el area &lt;strong&gt;Operativa&lt;/strong&gt; y su nuevo estado es &lt;strong&gt;Pendiente Aprobación DAF&lt;/strong&gt;
        ///&lt;br/&gt;&lt;br/&gt; 
        ///Analiticas relacionadas: 
        ///&lt;br/&gt;&lt;br/&gt; 
        ///{2}
        ///&lt;br/&gt; 
        ///Para acceder a la misma, por favor ingresar al siguiente &lt;a href=&apos;{1}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. 
        ///&lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string OcOperativeMessage {
            get {
                return ResourceManager.GetString("OcOperativeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Se informa que la orden de compra {0} ha sido &lt;strong&gt;RECHAZADA&lt;/strong&gt; por el area de {1} por el motivo: 
        ///&lt;br/&gt;&lt;br/&gt;
        ///{2}
        ///&lt;br/&gt;&lt;br/&gt; 
        ///Analiticas relacionadas 
        ///&lt;br/&gt;&lt;br/&gt; 
        ///{4}
        ///&lt;br/&gt; 
        ///Para acceder a la misma, por favor ingresar al siguiente &lt;a href=&apos;{3}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. 
        ///&lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string OcRejectMessage {
            get {
                return ResourceManager.GetString("OcRejectMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ORDEN DE COMPRA: {0}  &lt;br/&gt;&lt;br/&gt;
        ///Estimado,  &lt;br/&gt;
        ///La orden de compra de referencia se ha consumido. &lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string PurchaseOrderConsumed {
            get {
                return ResourceManager.GetString("PurchaseOrderConsumed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimados, &lt;/br&gt;&lt;/br&gt;
        ///La SOLFAC del asunto se encuentra COBRADA. &lt;/br&gt;
        ///Para acceder, por favor ingresar al siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. &lt;/br&gt;&lt;/br&gt;
        ///Muchas gracias..
        /// </summary>
        public static string SolfacStatusAmountCashedMessage {
            get {
                return ResourceManager.GetString("SolfacStatusAmountCashedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimados, &lt;/br&gt;&lt;/br&gt;
        ///La SOLFAC del asunto se encuentra FACTURADA. &lt;/br&gt;
        ///Para acceder, por favor ingresar al siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. &lt;/br&gt;&lt;/br&gt;
        ///Muchas gracias..
        /// </summary>
        public static string SolfacStatusInvoicedMessage {
            get {
                return ResourceManager.GetString("SolfacStatusInvoicedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimados, &lt;/br&gt;&lt;/br&gt;
        ///Control de Gestión ha aprobado la SOLFAC del asunto, que requiere su facturación. &lt;/br&gt;
        ///Para acceder, por favor ingresar al siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. &lt;/br&gt;&lt;/br&gt;
        ///Muchas gracias..
        /// </summary>
        public static string SolfacStatusInvoicePendingMessage {
            get {
                return ResourceManager.GetString("SolfacStatusInvoicePendingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimados, &lt;/br&gt;&lt;/br&gt;
        ///La SOLFAC del asunto ha sido RECHAZADA por Control de Gestión, por el siguiente motivo: &lt;/br&gt;
        /// *
        ///&lt;/br&gt;
        ///Por favor, ingresar al siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt; para modificar el formulario y enviar nuevamente &lt;/br&gt;&lt;/br&gt;
        ///Muchas gracias..
        /// </summary>
        public static string SolfacStatusManagementControlRejectedMessage {
            get {
                return ResourceManager.GetString("SolfacStatusManagementControlRejectedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimados, &lt;/br&gt;&lt;/br&gt;
        ///Se ha cargado una solfac que requiere revisión y aprobación &lt;/br&gt;
        ///Para acceder, por favor ingresar al siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. &lt;/br&gt;&lt;/br&gt;
        ///Muchas gracias..
        /// </summary>
        public static string SolfacStatusPendingByManagementControlMessage {
            get {
                return ResourceManager.GetString("SolfacStatusPendingByManagementControlMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimado, &lt;br/&gt;&lt;br/&gt;
        ///Se ha iniciado el proceso de facturación de la solicitud del asunto. Para acceder al misma, por favor ingresar al siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt;. &lt;br/&gt;&lt;br/&gt;
        ///Muchas gracias..
        /// </summary>
        public static string SolfacStatusPendingByManagementControlMessageToUser {
            get {
                return ResourceManager.GetString("SolfacStatusPendingByManagementControlMessageToUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimados, &lt;/br&gt;&lt;/br&gt;
        /// La SOLFAC del asunto ha sido RECHAZADA por Dirección de administración y finanzas, por el siguiente motivo: &lt;/br&gt;
        ///*
        ///&lt;/br&gt;
        ///Por favor, ingresar al siguiente &lt;a href=&apos;{0}&apos; target=&apos;_blank&apos;&gt;link&lt;/a&gt; para modificar el formulario y enviar nuevamente &lt;/br&gt;&lt;/br&gt;
        ///Muchas gracias..
        /// </summary>
        public static string SolfacStatusRejectedByDafMessage {
            get {
                return ResourceManager.GetString("SolfacStatusRejectedByDafMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Se ha &lt;strong&gt; Rechazado&lt;/strong&gt; la siguiente hora enviada:
        ///&lt;br/&gt;&lt;br/&gt;
        ///Analítica: {0}
        ///&lt;br/&gt;&lt;br/&gt;
        ///Tarea: {1}
        ///&lt;br/&gt;&lt;br/&gt;
        ///Fecha: {2}
        ///&lt;br/&gt;&lt;br/&gt;
        ///Horas: {3}
        ///&lt;br/&gt;&lt;br/&gt;
        ///Motivo: {4}
        ///&lt;br/&gt;&lt;br/&gt;
        ///Recurso: {5}
        ///&lt;br/&gt;&lt;br/&gt;
        ///Comentario: {6}
        ///&lt;br/&gt;.
        /// </summary>
        public static string WorkTimeRejectHours {
            get {
                return ResourceManager.GetString("WorkTimeRejectHours", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to El siguiente recurso contiene horas para su aprobación:
        ///&lt;br/&gt;&lt;br/&gt;
        ///Recurso: {0}
        ///&lt;br/&gt;&lt;br/&gt;
        ///Fechas:&lt;br/&gt;
        ///{1}
        ///&lt;br/&gt;.
        /// </summary>
        public static string WorkTimeSendHours {
            get {
                return ResourceManager.GetString("WorkTimeSendHours", resourceCulture);
            }
        }
    }
}
