import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/service";
import { Menu } from "models/menu";

@Injectable()
export class MenuService {
    private baseUrl: string;
    private headers: Headers;

    public menu: Menu[];

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getHeaders();
    }

    get(userName: string) {
       return this.http.get(`${this.baseUrl}/menu/${userName}`, { headers: this.headers}).map((res:Response) => res.json());
    }

    private getMenu(menuCode: string){
        var menuItems = null;
        var menuItem = null;
        if (this.menu){
            menuItems = this.menu.filter(x => x.code == menuCode);
        }
        if (menuItems && menuItems.length > 0){
            menuItem = menuItems[0];
        }
        return menuItem;
    }

    private getModule(menuCode: string, moduleCode: string){
        var menuItem = this.getMenu(menuCode);
        var moduleItems = null;
        var moduleItem = null;
        if (menuItem){
            moduleItems = menuItem.modules.filter(x => x.code == menuCode);
        }
        if (moduleItems && moduleItems.length > 0){
            moduleItem = moduleItems[0];
        }
        return moduleItem;
    }

    private getFunctionality(menuCode: string, moduleCode: string, funcCode: string){
        var moduleItem = this.getModule(menuCode, moduleCode);
        var funcItems = null;
        var funcItem = null;
        if (moduleItem){
            funcItems = moduleItem.functionalities.filter(x => x.code == funcCode);
        }
        if (funcItems && funcItems.length > 0){
            funcItem = funcItems[0];
        }
        return funcItem;
    }

    hasMenu(menuCode: string){
        var menuItem = this.getMenu(menuCode);
        return menuItem;
    }

    hasModule(menuCode: string, moduleCode: string){
        var menuItem = this.hasModule(menuCode, moduleCode);
        return menuItem;
    }

    hasFunctionality(moduleCode: string, funcCode: string){
        var menuItem
        if(this.menu){
            menuItem = this.menu.filter(x => x.modules.filter(y => y.code == moduleCode && y.functionalities.filter(z => z.code == funcCode)));
        }
        
        /*if(menuItem){
            var moduleItem = menuItem[0].modules.filter(x => x.code == moduleCode);
            if (moduleItem){
                moduleItem[0].functionalities
            }
        }*/
        return (menuItem);
    }
}