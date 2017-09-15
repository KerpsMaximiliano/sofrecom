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

  constructor(){
  }

  public ngOnInit():any {
    detectBody();
  }

  public onResize(){
    detectBody();
  }
}
