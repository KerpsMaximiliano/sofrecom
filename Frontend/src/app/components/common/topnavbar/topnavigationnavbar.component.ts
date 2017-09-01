import { ModuleService } from 'app/services/module.service';
import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router';
import { smoothlyMenu } from '../../../app.helpers';
import { MenuService } from "app/services/admin/menu.service";
declare var jQuery: any;

@Component({
  selector: 'topnavigationnavbar',
  templateUrl: 'topnavigationnavbar.template.html'
})
export class TopNavigationNavbarComponent implements OnInit{

  private currentUser: string;

  constructor(private menuService: MenuService){}

  ngOnInit(){
    /*if(this.menuService){
      this.menuService.menu = JSON.parse(localStorage.getItem('menu'));
      this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    }*/
  }

  toggleNavigation(): void {
    jQuery("body").toggleClass("mini-navbar");
    smoothlyMenu();
  }

}
