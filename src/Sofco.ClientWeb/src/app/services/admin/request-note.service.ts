import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { Service } from "../common/service";
import * as FileSaver from "file-saver";
import { SearchFilters } from "app/models/admin/requestNote";

declare const InstallTrigger: any;
@Injectable({
    providedIn: 'root'
})

export class RequestNoteService {

    private baseUrl: string;
    private mode = "Edit";

    constructor(
        private http: HttpClient,
        private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    setMode(mode: string) {
        this.mode = mode;
    }

    getMode() {
        return this.mode;
    }

    

    //FINALES
    public getById(id: number): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteBorrador/GetById/?id=${id}`);
    }

    public getAll(filters: SearchFilters): Observable<any> {
        let finalFilters = {
            startingSymbol: '',
            id: '',
            creationUserId: '',
            fromDate: '',
            toDate: '',
            providerId: '',
            statusId: '',
            analyticId: ''
        };
        if(filters.id != undefined || filters.id != null) {
            finalFilters.startingSymbol = '?';
            finalFilters.id = `id=${filters.id}`;
        };
        if(filters.creationUserId != undefined || filters.creationUserId != null) {
            if(finalFilters.startingSymbol == '?') {
                finalFilters.creationUserId = `&creationUserId=${filters.creationUserId}`;
            } else {
                finalFilters.startingSymbol = '?';
                finalFilters.creationUserId = `creationUserId=${filters.creationUserId}`;
            }
        };
        if(filters.fromDate != undefined || filters.fromDate != null) {
            if(finalFilters.startingSymbol == '?') {
                finalFilters.fromDate = `&fromDate=${filters.fromDate}`;
            } else {
                finalFilters.startingSymbol = '?';
                finalFilters.fromDate = `fromDate=${filters.fromDate}`;
            }
        };
        if(filters.toDate != undefined || filters.toDate != null) {
            if(finalFilters.startingSymbol == '?') {
                finalFilters.toDate = `&toDate=${filters.toDate}`;
            } else {
                finalFilters.startingSymbol = '?';
                finalFilters.toDate = `toDate=${filters.toDate}`;
            }
        };
        if(filters.providerId != undefined || filters.providerId != null) {
            if(finalFilters.startingSymbol == '?') {
                finalFilters.providerId = `&providerId=${filters.providerId}`;
            } else {
                finalFilters.startingSymbol = '?';
                finalFilters.providerId = `providerId=${filters.providerId}`;
            }
        };
        if(filters.statusId != undefined || filters.statusId != null) {
            if(finalFilters.startingSymbol == '?') {
                finalFilters.statusId = `&statusId=${filters.statusId}`;
            } else {
                finalFilters.startingSymbol = '?';
                finalFilters.statusId = `statusId=${filters.statusId}`;
            }
        };
        if(filters.analyticId != undefined || filters.analyticId != null) {
            if(finalFilters.startingSymbol == '?') {
                finalFilters.analyticId = `&analyticID=${filters.analyticId}`;
            } else {
                finalFilters.startingSymbol = '?';
                finalFilters.analyticId = `analyticID=${filters.analyticId}`;
            }
        };
        return this.http.get<any>(`${this.baseUrl}/RequestNoteBorrador/GetAll/${finalFilters.startingSymbol}${finalFilters.id}${finalFilters.creationUserId}${finalFilters.fromDate}${finalFilters.toDate}${finalFilters.providerId}${finalFilters.statusId}${finalFilters.analyticId}`);
    }

    public getHistories(id: number) {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteHistories/?id=${id}`);
    }

    private _getFile(id: number, type: number) {
        return this.http.get<any>(`${this.baseUrl}/file/${id}/${type}`);
    }

    public downloadFile(id: number, type: number, fileDescription: string) {
        let isFirefox = typeof InstallTrigger !== 'undefined';
        if (isFirefox) {
            this._getFile(id, type).subscribe(response => {
                let fileData = this._base64ToArrayBuffer(response.data);
                const blob = new Blob([fileData], { type: 'application' });
                FileSaver.saveAs(blob, fileDescription)
            });
        } else {
            this._getFile(id, type).subscribe(response => {
                let fileData = this._base64ToArrayBuffer(response.data);
                const blob = new Blob([fileData], { type: 'application/pdf' });
                FileSaver.saveAs(blob, fileDescription)
                //const url = URL.createObjectURL(blob);
                //window.open(url, '_blank');
            });
        }
    }

    public getProviders(requestNoteId: number): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteBorrador/GetProviders?requestNoteId=${requestNoteId}`);
    }

    private _base64ToArrayBuffer(base64Data) {
        const binary_string = window.atob(base64Data);
        const len = binary_string.length;
        const bytes = new Uint8Array(len);
        for (let i = 0; i < len; i++) {
          bytes[i] = binary_string.charCodeAt(i);
        }
        return bytes.buffer;
    }

    public 
    //Borrador
    public approveDraft(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteBorrador/AprobarBorrador?id=${id}`, null);
    };

    public saveDraft(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteBorrador/GuardarBorrador`, requestNote);
    }

    public uploadDraftFiles() {
        return `${this.baseUrl}/RequestNoteBorrador/UploadFiles`
    }
    //Pendiente Revisión Abastecimiento
    public sendPendingSupplyRevision(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/AprobarPendienteRevisionAbastecimiento`, requestNote)
    }

    public rejectPendingSupplyRevision(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/RechazarPendienteRevisionAbastecimiento`, requestNote)
    }
    //Pendiente Aprobación Gerentes Analítica
    public approvePendingApprovalManagementAnalytic(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/AprobarPendienteAprobacionGerentesAnalitica`, requestNote)
    }

    public rejectPendingApprovalManagementAnalytic(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/RechazarPendienteAprobacionGerentesAnalitica`, requestNote)
    }
    //Pendiente Aprobación Abastecimiento
    approvePendingSupplyApproval(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/AprobarPendienteAprobacionAbastecimiento`, requestNote);
    }

    rejectPendingSupplyApproval(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/RechazarPendienteAprobacionAbastecimiento`, requestNote);
    }
    //Pendiente Aprobación DAF
    approvePendingDAFApproval(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/AprobarPendienteAprobacionDAF`, requestNote);
    }

    rejectPendingDAFApproval(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/RechazarPendienteAprobacionDAF`, requestNote);
    }

    //Aprobada
    applyApproved(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/AprobarAprobada`, requestNote);
    }

    //Request Notes Requested Provider - Solicitada a Proveedor
    approveRequestedProvider(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/AprobarSolicitadaProveedor`, requestNote);
    }

    closeRequestedProvider(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/CerrarSolicitadaProveedor`, requestNote);
    }

    //Recibido Conforme
    approveReceivedConformable(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/AprobarRecibidoConforme`, requestNote);
    }

    //Factura Pendiente Aprobación Gerente
    approvePendingManagementBillApproval(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/AprobarFacturaPendienteAprobacion`, requestNote);
    }

    //Pendiente Procesar GAF
    approvePendingGAFProcessing(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteCambiosEstado/AprobarPendienteProcesarGAF`, requestNote);
    }

    //COMMENTS

    getComments(requestNoteId: number): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteComment/GetByNoteRequest?id=${requestNoteId}`);
    }

    deleteComment(commentId: number): Observable<any> {
        return this.http.delete<any>(`${this.baseUrl}/RequestNoteComment?id=${commentId}`);
    }

    postComment(comment: {requestNoteId: number, comment: string}): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteComment`, comment);
    }
}