import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable()
export class PaymentPendingService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    get(){
        return this.http.get<any>(`${this.baseUrl}/paymentPending`);
    }

    exportFile(){
        return this.http.get(`${this.baseUrl}/paymentPending/excel`, {
            responseType: 'arraybuffer',
            observe: 'response'
        }).pipe(map((res: any) => {
            return new Blob([res.body], { type: 'application/octet-stream' });
        }));
    } 
} 