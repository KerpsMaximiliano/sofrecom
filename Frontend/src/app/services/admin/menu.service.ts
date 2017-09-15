import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Service } from "app/services/common/service";
import { Menu } from "app/models/admin/menu";

@Injectable()
export class MenuService {
    private baseUrl: string;

    public menu: Menu[];
    public currentUser: any;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
 
        if(!this.menu){
            this.menu = JSON.parse(localStorage.getItem('menu'));
        }

        if(!this.currentUser){
            this.currentUser = Cookie.get('currentUser')
        }

        var userInfo = Cookie.get("userInfo");
        if(userInfo){
            this.currentUser = JSON.parse(userInfo).name;
        }
    }

    get(userName: string) {
       return this.http.get(`${this.baseUrl}/menu/${userName}`, { headers: this.service.getHeaders()})
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

    hasAdminMenu(){
       if(this.hasModule("USR") || this.hasModule("GRP") || this.hasModule("ROL") || this.hasModule("MOD") || this.hasModule("FUNC")){
            return true;
       }

       return false;
    }

    hasBillingMenu(){
       if(this.hasModule("SOLFA")){
            return true;
       }

       return false;
    }
}