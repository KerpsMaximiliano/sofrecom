import { Injectable } from '@angular/core';
import { Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { Group } from "app/models/admin/group";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class GroupService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/groups`);
    }

    get(id: number) {
       return this.http.get<any>(`${this.baseUrl}/groups/${id}`);
    }

    add(model: Group) {
        return this.http.post<any>(`${this.baseUrl}/groups`, model);
    }

    edit(model) {
        return this.http.put<any>(`${this.baseUrl}/groups`, model);
    }

    delete(id: number) {
        return this.http.delete<any>(`${this.baseUrl}/groups/${id}`);
    }

    deactivate(id: number) {
        return this.http.put<any>(`${this.baseUrl}/groups/${id}/active/false`, {});
    }

    activate(id: number) {
        return this.http.put<any>(`${this.baseUrl}/groups/${id}/active/true`, {});
    }

    getOptions() {
        return this.http.get<any>(`${this.baseUrl}/groups/options`);
    }
}