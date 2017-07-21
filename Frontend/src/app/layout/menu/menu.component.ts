import { Component, OnInit, ChangeDetectorRef, ViewChild, ElementRef, Renderer } from '@angular/core';
import {TranslateService} from '@ngx-translate/core';
import * as _ from 'lodash';
import { ConfigService } from "app/singleton/config.service";

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {

  //@ViewChild('t') t: ElementRef;

  constructor(private renderer: Renderer, private trans: TranslateService, public s: ConfigService) { 
      let browserLang = trans.getBrowserLang();
      s.setCurrLang(browserLang);
  }

  ngOnInit() {
  }

  selectLanguage(lang){
    this.s.setCurrLang(lang);
  }

  /*translate(str: string){
    this.renderer.invokeElementMethod(this.t, 'translate', [str]);
  }*/

  capitalize(str): string{
    return _.capitalize(str);
  }

}
