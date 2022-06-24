import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Service } from "../common/service";

@Injectable({
    providedIn: 'root'
})

export class RequestNoteService {

    private baseUrl: string;

    constructor(
        private http: HttpClient,
        private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    public getById(id: number): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteAprobada/GetById/${id}`);
    }

    public getAll(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteAprobada/GetAll`);
    }

    //New Request Note
    public approveDraft(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteBorrador/AprobarRequestNote`, id);
    };

    public saveDraft(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteBorrador/GuardarBorrador`, requestNote);
    }

    //Request Notes Pending Supply Revision - Pendiente Revisión Abastecimiento
    approvePendingSupplyRevision(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteRevisionAbastecimiento/AprobarPendienteRevisionAbastecimiento`, id);
    }

    rejectPendingSupplyRevision(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteRevisionAbastecimiento/RechazarPendienteRevisionAbastecimiento`, id);
    }

    saveNotePendingSupplyRevision(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteRevisionAbastecimiento/GuardarArchivo`, requestNote);
    }

    //Request Notes Pending Approval Management Analytic - Pendiente Aprobación Gerentes Analítica
    approvePendingApprovalManagementAnalytic(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteAprobacionGerentesAnalitica/AprobarPendienteAprobacionGerentesAnalitica`, id);
    }

    rejectPendingApprovalManagementAnalytic(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteAprobacionGerentesAnalitica/RechazarPendienteAprobacionGerentesAnalitica`, id);
    }

    //Request Notes Pending Supply Approval - Pendiente Aprobación Abastecimiento
    approvePendingSupplyApproval(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteAprobacionAbastecimiento/AprobarPendienteAprobacionAbastecimiento`, id);
    }

    rejectPendingSupplyApproval(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteAprobacionAbastecimiento/RechazarPendienteAprobacionAbastecimiento`, id);
    }

    uploadFilePendingSupplyApproval(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteAprobacionAbastecimiento/AdjuntarArchivo`, requestNote);
    }

    //Request Notes Pending DAF Approval - Pendiente Aprobación DAF
    approvePendingDAFApproval(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteAprobacionDAF/AprobarPendienteAprobacionDAF`, id);
    }

    rejectPendingDAFApproval(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteAprobacionDAF/RechazarPendienteAprobacionDAF`, id);
    }

    downloadFilePendingDAFApproval(id: number, type: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteAprobacionDAF/DescargarArchivo`, id, type);
    }

    //Request Notes Approved - Aprobada
    approveApproved(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteAprobada/AprobarAprobada`, id);
    }

    rejectApproved(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteAprobada/RechazarAprobada`, id);
    }

    listFilesApproved(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteAprobada/ListarArchivos`);
    }

    uploadProviderFilesApproved(requestNoteProvider: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteAprobada/CargarArchivosProveedor`, requestNoteProvider);
    }

    //Request Notes Requested Provider - Solicitada a Proveedor
    approveRequestedProvider(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteSolicitadaProveedor/AprobarSolicitadaProveedor`, id);
    }

    closeRequestedProvider(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteSolicitadaProveedor/CerrarSolicitadaProveedor`, id);
    }

    downloadFileRequestedProvicer(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteSolicitadaProveedor/DescargarArchivo`);
    }

    uploadRequestedProviderFilesApproved(requestNoteProviders: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteSolicitadaProveedor/CargarArchivosAdjuntos`, requestNoteProviders);
    }

    //Request Notes Received Conformable - Recibido Conforme
    approveReceivedConformable(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteRecibidoConforme/AprobarRecibidoConforme`, id);
    }

    closeReceivedConformable(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteRecibidoConforme/CerrarSolicitadaProveedor`, id);
    }

    downloadFileReceivedConformable(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteRecibidoConforme/DescargarArchivo`);
    }

    listFilesReceivedConformable(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteRecibidoConforme/ListarArchivos`);
    }

    uploadFilesReceivedConformable(requestNoteProviders: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteRecibidoConforme/CargarArchivosAdjuntos`, requestNoteProviders);
    }

    //Request Notes Pending Management Bill Approval - Factura Pendiente Aprobación Gerente
    approvePendingManagementBillApproval(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteFacturaPendienteAprobacionGerente/AprobarRecibidoConforme`, id);
    }

    listFilesPendingManagementBillApproval(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteFacturaPendienteAprobacionGerente/ListarArchivos`);
    }

    downloadFilePendingManagementBillApproval(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteFacturaPendienteAprobacionGerente/DescargarArchivo`);
    }

    //Request Notes Pending GAF Processing - Pendiente Procesar GAF
    approvePendingGAFProcessing(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteProcesarGAF/AprobarPendienteProcesarGAF`, id);
    }

}