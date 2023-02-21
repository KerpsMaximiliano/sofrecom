import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router, UrlSerializer } from "@angular/router";
import { BuyOrderSearchFilters } from "app/models/admin/buyOrder";
import { SearchFilters } from "app/models/admin/requestNote";
import { Observable } from "rxjs-compat";
import { Service } from "../common/service";

declare const InstallTrigger: any;
@Injectable({
    providedIn: 'root'
})

export class PurchaseOrderService {

    private baseUrl: string;
    private mode = "View";
    private currentId: number;

    constructor(
        private http: HttpClient,
        private service: Service,
        private router: Router,
        private serializer: UrlSerializer) {
        this.baseUrl = this.service.UrlApi;
    }

    setMode(mode: string) {
        this.mode = mode;
    }

    getMode() {
        return this.mode;
    }

    public getId() {
        return this.currentId;
    }

    public setId(id: number) {
        this.currentId = id;
    }


    /////
    public saveOC(oc: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/buyOrderRequestNote`, oc);
    }

    public getOCById(id: number): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/buyOrderRequestNote/GetById?id=${id}`);
    }

    public getAll(filters: BuyOrderSearchFilters): Observable<any> {
        let finalFilters = {
            startingSymbol: '',
            id: '',
            requestNoteId: '',
            fromDate: '',
            toDate: '',
            providerId: '',
            statusId: '',
            number: ''
        };
        if(filters.id != undefined || filters.id != null) {
            finalFilters.startingSymbol = '?';
            finalFilters.id = `id=${filters.id}`;
        };
        if(filters.requestNoteId != undefined || filters.requestNoteId != null) {
            if(finalFilters.startingSymbol == '?') {
                finalFilters.requestNoteId = `&requestNoteId=${filters.requestNoteId}`;
            } else {
                finalFilters.startingSymbol = '?';
                finalFilters.requestNoteId = `requestNoteId=${filters.requestNoteId}`;
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
        if(filters.number != undefined || filters.number != null) {
            if(finalFilters.startingSymbol == '?') {
                finalFilters.number = `&number=${filters.number}`;
            } else {
                finalFilters.startingSymbol = '?';
                finalFilters.number = `number=${filters.number}`;
            }
        };
        return this.http.get<any>(`${this.baseUrl}/buyOrderRequestNote/GetAll/${finalFilters.startingSymbol}${finalFilters.id}${finalFilters.requestNoteId}${finalFilters.fromDate}${finalFilters.toDate}${finalFilters.providerId}${finalFilters.statusId}${finalFilters.number}`);
    }

    public getStates(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/buyOrderRequestNote/States`);
    }

}