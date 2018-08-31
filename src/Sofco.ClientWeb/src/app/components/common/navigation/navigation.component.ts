import { MessageService } from '../../../services/common/message.service';
import { Subscription } from 'rxjs';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { smoothlyMenu } from '../../../app.helpers';
import 'jquery-slimscroll';
import { MenuService } from "../../../services/admin/menu.service";
import { Menu } from "../../../models/admin/menu";

declare var jQuery:any;

@Component({
  selector: 'navigation',
  templateUrl: 'navigation.template.html'
})

export class NavigationComponent implements OnInit {

  public menu: Menu[];
  public menuSubscrip: Subscription;
  public isExternal: boolean = false;

  constructor(
      private router: Router,
      public menuService: MenuService,
      public messageService: MessageService) {}

  ngOnInit(){
    var user = this.menuService.user;

    if(user){
      this.isExternal = user.isExternal;
    }
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
}
