import { Menu } from 'models/menu';
import { MenuService } from './../../../services/menu.service';
import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router';
import { smoothlyMenu } from '../../../app.helpers';
import 'jquery-slimscroll';

declare var jQuery:any;

@Component({
  selector: 'navigation',
  templateUrl: 'navigation.template.html'
})

export class NavigationComponent {

  public menu: Menu[];

  constructor(
      private router: Router,
      private menuService: MenuService) {}


  ngAfterViewInit() {
    jQuery('#side-menu').metisMenu();

    if (jQuery("body").hasClass('fixed-sidebar')) {
      jQuery('.sidebar-collapse').slimscroll({
        height: '100%'
      })
    }


  }

  toggleNavigation(): void {
    jQuery("body").toggleClass("mini-navbar");
    smoothlyMenu();
  }


  activeRoute(routename: string): boolean{
    return this.router.url.indexOf(routename) > -1;
  }


}
