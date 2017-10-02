import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Service } from "app/services/common/service";
import { Menu } from "app/models/admin/menu";
import { DatepickerOptions } from 'ng2-datepicker';

@Injectable()
export class MenuService {
    private baseUrl: string;

    public menu: Menu[];
    public userIsDirector: boolean;
    public currentUser: any;
    public user: any;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
 
        if(!this.menu){
            var menu = JSON.parse(localStorage.getItem('menu'));
            this.menu = menu.menus;
            this.userIsDirector = menu.isDirector;
        }

        if(!this.currentUser){
            this.currentUser = Cookie.get('currentUser')
        }

        var userInfo = Cookie.get("userInfo");

        if(userInfo){
            var jsonParsed = JSON.parse(userInfo);
            this.currentUser = jsonParsed.name;
            this.user = jsonParsed;
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
    
    getDatePickerOptions(){
        var options: DatepickerOptions = {
            minYear: 1970,
            maxYear: 2030,
            displayFormat: 'DD/MM/YYYY',
            barTitleFormat: 'MMMM YYYY',
            firstCalendarDay: 1
          };

        return options;
    }
}