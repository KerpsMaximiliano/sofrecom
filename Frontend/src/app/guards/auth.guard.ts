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
            return true;            
        }

        Cookie.delete('access_token');
        Cookie.delete('userInfo');
        Cookie.delete('currentUser');
        Cookie.delete('currentUserMail');
        localStorage.removeItem('menu');

        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url }});
        return false;
    }
}