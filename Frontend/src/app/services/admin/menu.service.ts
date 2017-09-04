import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Menu } from "models/menu";
import { Module } from "models/module";
import { User } from "models/user";
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Service } from "app/services/common/service";

@Injectable()
export class MenuService {
    private baseUrl: string;
    private headers: Headers;

    public menu: Menu[];
    public currentUser: any;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getHeaders();

        var userInfo = Cookie.get("userInfo");
        if(userInfo){
            this.currentUser = JSON.parse(userInfo).name;
        }
    }

    reloadHeaders(){
        this.headers = this.service.getHeaders();
    }

    get(userName: string) {
       return this.http.get(`${this.baseUrl}/menu/${userName}`, { headers: this.headers})
                       .map((res:Response) => {
                           var rpta = res.json();
                           return rpta;
                        });
    }

    hasModule(module: string){
        return this.menu.findIndex(x => x.module == module) > -1;
    }

    hasFunctionality(module: string, functionality: string){
        return this.menu.findIndex(x => x.module == module && x.functionality == functionality) > -1;
    }

    hasMenu(modules: string[]){
        var result = false;

        modules.forEach(item => {
            if(this.hasModule(item)){
                result = true;
            }
        });

        return result;
    }
}