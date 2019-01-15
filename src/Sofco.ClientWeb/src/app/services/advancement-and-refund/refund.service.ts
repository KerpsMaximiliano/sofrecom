import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import {map} from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class RefundService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    add(model){
        return this.http.post<any>(`${this.baseUrl}/refund`, model);
    }

    edit(model){
        return this.http.put<any>(`${this.baseUrl}/refund`, model);
    }

    get(id){
        return this.http.get<any>(`${this.baseUrl}/refund/${id}`);
    }

    getHistories(id){
        return this.http.get<any>(`${this.baseUrl}/refund/${id}/histories`);
    }

    getAllInProcess(){
        return this.http.get<any>(`${this.baseUrl}/refund/inProcess`);
    }

    getAllFinalized(model){
        return this.http.post<any>(`${this.baseUrl}/refund/finalized`, model);
    }

    canLoad(){
        return this.http.get<any>(`${this.baseUrl}/refund/canLoad`);
    }

    getStates(){
        return this.http.get<any>(`${this.baseUrl}/refund/states`);
    }

    getUrlForImportExcel(id){
        return `${this.baseUrl}/refund/${id}/file`;
    }

    deleteFile(id, fileId){
        return this.http.delete<any>(`${this.baseUrl}/refund/${id}/file/${fileId}`);
    }

    getAll(model){
        return this.http.post<any>(`${this.baseUrl}/refund/search`, model);
    }

    exportFile(id){
        return this.http.get(`${this.baseUrl}/refund/export/${id}`, {
            responseType: 'arraybuffer',
            observe: 'response'
        }).pipe(map((res: any) => {
            return new Blob([res.body], { type: 'application/octet-stream' });
        }));
    } 
}
