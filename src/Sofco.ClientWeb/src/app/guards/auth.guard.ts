import { Subscription } from 'rxjs';
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { MenuService } from "../services/admin/menu.service";
import { AuthenticationService } from '../services/common/authentication.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private router: Router,
                private authenticationService: AuthenticationService,
                private menuService: MenuService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        var mustlogout = localStorage.getItem('mustLogout');

        if(mustlogout == "true"){
            this.authenticationService.logout();
            this.router.navigate(['/login'], { queryParams: { returnUrl: state.url }});
        }

        if (Cookie.get('access_token') && Cookie.get('currentUser')) {

            if(route.data && JSON.stringify(route.data) !== JSON.stringify({})){
                var data = <any>route.data;

                if(data.group){
                    return this.hasGroup(data.group);
                }
                    
                if(data.module && data.functionality){
                    if(this.menuService.hasFunctionality(data.module, data.functionality)){
                        return true;
                    }
                    else{
                        this.router.navigate(['/403']);
                        return false;
                    }
                }

                return true;
            }
            else{
                return true;
            }

        }
        
        this.authenticationService.logout();
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url }});

        return false;
    }

    hasGroup(code): boolean {
        switch(code){
            case "DAF": return this.menuService.userIsDaf;
            case "CDG": return this.menuService.userIsCdg;
            case "DIRECTOR": return this.menuService.userIsDirector;
            case "MANAGER": return this.menuService.userIsManager;
            case "RRHH": return this.menuService.userIsRrhh;
            default: return false;
        }
    }
}