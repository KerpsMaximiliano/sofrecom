import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Service } from "../common/service";

declare const InstallTrigger: any;
@Injectable({
    providedIn: 'root'
})

export class PurchaseOrderService {

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

}