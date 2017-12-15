import { Injectable } from '@angular/core';
import { Response, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class UserService {
    private baseUrl: string;

    constructor(private http: HttpAuth, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/users`).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/users/${id}`).map((res:Response) => res.json());
    }

    getByEmail(mail: string) {
       return this.http.get(`${this.baseUrl}/users/email/${mail}`).map((res:Response) => res.json());
    }

    searchByEmail(mail: string) {
        return this.http.get(`${this.baseUrl}/users/ad/email/${mail}`).map((res:Response) => res.json());
    }

    searchBySurname(surname: string) {
        return this.http.get(`${this.baseUrl}/users/ad/surname/${surname}`).map((res:Response) => res.json());
    }

    save(model){
        return this.http.post(`${this.baseUrl}/users`, model).map((res:Response) => res.json());
    }

    getOptions() {
       return this.http.get(`${this.baseUrl}/users/options`).map((res:Response) => res.json());
    }

    getDetail(id: string) {
       return this.http.get(`${this.baseUrl}/users/${id}/detail`).map((res:Response) => res.json());
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/users/${id}/active/false`, {}).map((res:Response) => res.json());
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/users/${id}/active/true`, {}).map((res:Response) => res.json());
    }

    assignGroups(userId: number, objToSend: any){
        return this.http.post(`${this.baseUrl}/users/${userId}/groups`, objToSend).map((res:Response) => res.json());
    }

    unassignGroup(userId: number, groupId: number) {
        return this.http.delete(`${this.baseUrl}/users/${userId}/group/${groupId}`).map((res:Response) => res.json());
    }
}