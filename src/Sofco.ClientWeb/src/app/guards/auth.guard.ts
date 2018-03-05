import { Subscription } from 'rxjs/Subscription';
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { MenuService } from "app/services/admin/menu.service";

@Injectable()
export class AuthGuard implements CanActivate {

    private menuSubscrip: Subscription;

    constructor(private router: Router,
                private menuService: MenuService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (Cookie.get('access_token') && Cookie.get('currentUser')) {

            if(route.data && JSON.stringify(route.data) !== JSON.stringify({})){
                var data = <any>route.data;

                if(!data.module && !data.functionality) return true;

                if(this.menuService.hasFunctionality(data.module, data.functionality)){
                    return true;
                }
                else{
                    this.router.navigate(['/403']);
                    return false;
                }
            }
            else{
                return true;
            }

        }
        
        Cookie.deleteAll();
        sessionStorage.clear();
        localStorage.removeItem('menu');

        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url }});
        return false;
    }
}