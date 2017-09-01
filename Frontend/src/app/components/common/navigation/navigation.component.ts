import { MessageService } from 'app/services/common/message.service';
import { Subscription } from 'rxjs/Subscription';
import { Menu } from 'models/menu';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { smoothlyMenu } from '../../../app.helpers';
import 'jquery-slimscroll';
import { MenuService } from "app/services/admin/menu.service";

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
}
