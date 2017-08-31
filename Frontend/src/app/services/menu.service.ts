import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/service";
import { Menu } from "models/menu";
import { Module } from "models/module";
import { User } from "models/user";
import { Cookie } from 'ng2-cookies/ng2-cookies';

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

  getMenu(menu: string): Menu{
    var m = this.menu;
    var i: number;
    var rpta = null;
    if(m){
      i = m.findIndex(x => x.code == menu);
      rpta = m[i];
    }
    return rpta;
  }

  getModule(menu: string, module: string): Module{
    var m = this.getMenu(menu);
    var i: number;
    var rpta = null;
    if(m && m.modules){
        i = m.modules.findIndex(x => x.code == module);
        if(i > -1){
            rpta = m.modules[i];
        }
    }
    return rpta;
  }

  hasMenu(menu: string){
    var m = this.menu ;
    var rpta: boolean = (m != null && m != undefined) &&
                         m.findIndex(x => x.code == menu) > -1;

    return rpta;
  }

  hasModule(menu: string, module: string){
    var m = this.getMenu(menu);
    var rpta: boolean = (m != null && m != undefined) &&
                         m.modules.findIndex(x => x.code == module) > -1;

    return rpta;
  }

  hasFunctionality(menu: string, module: string, functionality: string){
    var mod = this.getModule(menu, module);
    var rpta: boolean = (mod != null && mod != undefined) &&
                        mod.functionalities.findIndex(x => x.code == functionality) > -1;

    return rpta;
  }

/*
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
    }*/

/*
    hasMenu(menuCode: string){
        var menuItem = this.getMenu(menuCode);
        return (menuItem != null && menuItem != undefined);
    }

    hasModule(menuCode: string, moduleCode: string){
        var menuItem = this.hasModule(menuCode, moduleCode);
        return (menuItem != null && menuItem != undefined);
    }*/


}