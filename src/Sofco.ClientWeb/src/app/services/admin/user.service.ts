
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Response, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from '../common/service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class UserService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/users`);
    }

    get(id: number) {
       return this.http.get<any>(`${this.baseUrl}/users/${id}`);
    }

    getByEmail() {
       return this.http.get<any>(`${this.baseUrl}/users/email/`).pipe(map((res: Response) => res['data']));
    }

    searchByEmail(mail: string) {
        return this.http.get<any>(`${this.baseUrl}/users/ad/email/${mail}`);
    }

    searchBySurname(surname: string) {
        return this.http.get<any>(`${this.baseUrl}/users/ad/surname/${surname}`);
    }

    save(model){
        return this.http.post<any>(`${this.baseUrl}/users`, model);
    }

    getOptions() {
       return this.http.get<any>(`${this.baseUrl}/users/options`);
    }

    getDetail(id: string) {
       return this.http.get<any>(`${this.baseUrl}/users/${id}/detail`);
    }

    deactivate(id: number) {
        return this.http.put<any>(`${this.baseUrl}/users/${id}/active/false`, {});
    }

    activate(id: number) {
        return this.http.put<any>(`${this.baseUrl}/users/${id}/active/true`, {});
    }

    assignGroups(userId: number, objToSend: any){
        return this.http.post<any>(`${this.baseUrl}/users/${userId}/groups`, objToSend);
    }

    unassignGroup(userId: number, groupId: number) {
        return this.http.delete<any>(`${this.baseUrl}/users/${userId}/group/${groupId}`);
    }

    getCommercialManagers() {
        return this.http.get<any>(`${this.baseUrl}/users/commercialManagers`);
    }
}
