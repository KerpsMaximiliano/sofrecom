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
        return this.http.get<any>(``)
    }

    public getAll(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/RequestNoteAprobada/GetAll`);
    }

    //New Request Note
    public approve(id: number): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteAprobada/AprobarRequestNote`, id);
    };

    public save(requestNote: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/RequestNoteAprobada/GuardarBorrador`, requestNote);
    }

    public RequestNotePendingSupplyRevision = {
        //Pantalla 2
        reject(id: number): Observable<any> {
            return this.http.post(`${this.baseUrl}/RequestNoteAprobada/RechazarRequestNote`, id);
        }
    }
}