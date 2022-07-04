import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { Service } from "../common/service";
import * as FileSaver from "file-saver";

@Injectable({
    providedIn: 'root'
})

export class RequestNoteService {

    private baseUrl: string;
    private mode = "View";

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

    //Request Notes Pending GAF Processing - Pendiente Procesar GAF
    approvePendingGAFProcessing(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNotePendienteProcesarGAF/AprobarPendienteProcesarGAF`, id);
    }

    //FINALES
    public getById(id: number): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteAprobada/GetById/${id}`);
    }

    public getAll(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteAprobada/GetAll`);
    }

    public getHistories(id: number) {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteHistories/?id=${id}`);
    }

    private _getFile(id: number, type: number) {
        return this.http.get<any>(`${this.baseUrl}/file/${id}/${type}`);
    }

    public downloadProviderFile(id: number, type: number) {
        //return this.http.get<any>(`${this.baseUrl}/file/${id}/${type}`);
        this._getFile(id, type).subscribe(response => {
            let fileData = this._base64ToArrayBuffer(response.data);
            const blob = new Blob([fileData], { type: 'application/pdf' });
            FileSaver.saveAs(blob)
            //const url = URL.createObjectURL(blob);
            //window.open(url, '_blank');
        });
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

    public downloadPurchaseOrder() {

    }

    public downloadProviderDocumentation() {

    }

    public downloadRecievedConformableDocumentation() {

    }

    public downloadBill() {

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
}