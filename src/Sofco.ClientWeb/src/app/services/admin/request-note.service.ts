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

    public RequestNoteNew = {
        //Pantalla 1
        approve(id: number): Observable<any> {
            return this.http.post(`${this.baseUrl}/RequestNoteAprobada/AprobarRequestNote`, id);
        },
        save(requestNote: any): Observable<any> {
            return this.http.post(`${this.baseUrl}/RequestNoteAprobada/GuardarBorrador`, requestNote);
        }
    }

    public RequestNotePendingSupplyRevision = {
        //Pantalla 2
        reject(id: number): Observable<any> {
            return this.http.post(`${this.baseUrl}/RequestNoteAprobada/RechazarRequestNote`, id);
        }
    }
}