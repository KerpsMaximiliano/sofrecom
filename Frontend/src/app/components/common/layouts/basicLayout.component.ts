import { MenuService } from 'app/services/menu.service';
import { Component } from '@angular/core';
import { detectBody } from '../../../app.helpers';

declare var jQuery:any;

@Component({
  selector: 'basic',
  templateUrl: 'basicLayout.template.html',
  host: {
    '(window:resize)': 'onResize()'
  }
})
export class BasicLayoutComponent {

  constructor(private menuService: MenuService){

  }

  public ngOnInit():any {
    if(!this.menuService.menu){
      this.menuService.menu = JSON.parse(localStorage.getItem('menu'));
    }
    if(!this.menuService.currentUser){
      var currentUser = JSON.parse(localStorage.getItem('currentUser'));
      this.menuService.currentUser = currentUser.userName;
    }
    detectBody();
  }

  public onResize(){
    detectBody();
  }

}
