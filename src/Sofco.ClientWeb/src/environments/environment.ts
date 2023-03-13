export const environment = {
  production: false,
  urlApi: 'http://localhost:9696/api',
  crmCloseStatusCode: '717620004',
  currencyPesosId: 1,
  draftWorkflowStateId: 8,
  gafWorkflowStateId: 10,
  dafWorkflowStateId: 4,
  rrhhWorkflowStateId: 6,
  rejectedWorkflowStateId: 2,
  pendingDirectorWorkflowStateId: 12,
  pendingManagerWorkflowStateId: 9,
  pendingComplianceWorkflowStateId: 5,
  pendingComplianceDWorkflowStateId: 22,
  pendingComplianceDGWorkflowStateId: 23,
  redCategoryId: 22,
  infrastructureCategoryId: 14,
  PENDIENTE_APROBACION_GERENTE_ANALITICA_STATE_ID: 29,
  REQUEST_NOTE_STATES: [
    { id: 0, text: "Todos" },
    { id: 8, text: "Borrador" },
    { id: 29, text: "Pendiente Aprobación Gerentes Analítica" },
    { id: 31, text: "Pendiente Aprobación Compras" },
    { id: 4, text: "Pendiente Aprobación DAF" },
    { id: 33, text: "Pendiente Generación SAP" },
    { id: 34, text: "Pendiente Recepción Mercadería" },
    { id: 35, text: "Recepción Parcial" },
    { id: 36, text: "Cerrada" },
    { id: 30, text: "Rechazada" }
  ],
  NP_BORRADOR: 8,
  NP_PEND_APRO_GER_AN: 29,
  NP_PEND_APRO_COMP: 31,
  NP_PEND_APRO_DAF: 4,
  //NP_PEND_GEN_SAP: 34, //prod
  NP_PEND_GEN_SAP: 33, //dev
  NP_PEND_RECEP_MERC: 34,
  NP_RECEP_PARCIAL: 35,
  NP_CERRADA: 36, //prod
  NP_RECHAZADA: 30,
  OC_PEND_APRO_DAF: 4, //dev
  OC_PEND_RECEP_MERC: 34, //dev
  OC_PEND_RECEP_FAC: 37, //dev
  OC_FINAL: 11 //dev
}; 