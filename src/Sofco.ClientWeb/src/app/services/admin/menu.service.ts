import { Injectable } from '@angular/core';
import { Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Service } from "app/services/common/service";
import { Menu } from "app/models/admin/menu";
import { HttpAuth } from "app/services/common/http-auth";
import { DatepickerOptions } from 'ng2-datepicker';

@Injectable()
export class MenuService {
    private baseUrl: string;

    public menu: Menu[];
    public userIsDirector: boolean;
    public userIsDaf: boolean;
    public userIsCdg: boolean;
    public currentUser: any;
    public user: any;

    public dafMail: string;
    public cdgMail: string;
    public pmoMail: string;
    public rrhhMail: string;
    public sellerMail: string;

    constructor(private http: HttpAuth, private service: Service) {
        this.baseUrl = this.service.UrlApi;
        
        if(!this.menu){
            var menu = JSON.parse(localStorage.getItem('menu'));
            if(menu != null)
            {
                this.menu = menu.menus;
                this.userIsDirector = menu.isDirector;
                this.userIsDaf = menu.isDaf;
                this.userIsCdg = menu.isCdg;

                this.dafMail = menu.dafMail;
                this.cdgMail = menu.cdgMail;
                this.pmoMail = menu.pmoMail;
                this.rrhhMail = menu.rrhhMail;
                this.sellerMail = menu.sellerMail;
            }
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
       return this.http.get(`${this.baseUrl}/menu/${userName}`)
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

    hasAllocationManagementMenu(){
        if(this.hasModule("ALLOC")){
            return true;
       }

       return false;
    }

    hasReportMenu(){
        if(this.hasModule("REPOR")){
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