import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AdvancementRefundSettingService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    update(model){
        return this.http.post<any>(`${this.baseUrl}/advancementRefund/setting`, model);
    }

    get(){
        return this.http.get<any>(`${this.baseUrl}/advancementRefund/setting`);
    }
}