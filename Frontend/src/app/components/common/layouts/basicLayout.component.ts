import { MenuService } from 'app/services/menu.service';
import { Component } from '@angular/core';
import { detectBody } from '../../../app.helpers';
import { Cookie } from 'ng2-cookies/ng2-cookies';

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
      this.menuService.currentUser = Cookie.get('currentUser');
    }
    
    detectBody();
  }

  public onResize(){
    detectBody();
  }
}
