import { MessageService } from './../../../services/message.service';
import { Subscription } from 'rxjs/Subscription';
import { Menu } from 'models/menu';
import { MenuService } from './../../../services/menu.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { smoothlyMenu } from '../../../app.helpers';
import 'jquery-slimscroll';

declare var jQuery:any;

@Component({
  selector: 'navigation',
  templateUrl: 'navigation.template.html'
})

export class NavigationComponent implements OnInit, OnDestroy{

  public menu: Menu[];
  public menuSubscrip: Subscription;

  constructor(
      private router: Router,
      public menuService: MenuService,
      private route: ActivatedRoute,
      public messageService: MessageService) {}


  ngOnInit(){

    /*if(!this.menuService.menu){
      this.menuService.menu = JSON.parse(localStorage.getItem('menu'));
    }*/
     /* var userName = localStorage.getItem('currentUser').replace(/\"/g, '');
      var returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

      if(!this.menuService.menu){
        this.menuSubscrip = this.menuService.get(userName).subscribe(
            data => {
                this.menuService.menu = data;
            },
            error => {
                if(error.messages) this.messageService.showMessages(error.messages);
            }
        );
      }*/
  }

  ngOnDestroy(){

  }

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

/*
  getMenu(menu: string): Menu{
    var m = this.menuService.menu;
    var i: number;
    var rpta = null;
    if(m){
      i = m.findIndex(x => x.code == menu);
      rpta = m[i];
    }
    return rpta;
  }

  hasMenu(menu: string){
    var m = this.menuService.menu ;
    var rpta: boolean = (m != null && m != undefined) &&
                         m.findIndex(x => x.code == menu) > -1;

    return rpta;
  }

  hasModule(menu: string, module: string){
    var m = this.getMenu(menu);
    var rpta: boolean = (m != null && m != undefined) &&
                         m.modules.findIndex(x => x.code == module) > -1;

    return rpta;
  }*/

}
