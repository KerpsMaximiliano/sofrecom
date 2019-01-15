import { Component, ViewChild } from '@angular/core';
import { detectBody } from '../../../app.helpers';
// import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { AuthenticationService } from '../../../services/common/authentication.service';
import { Router } from '@angular/router';

declare var jQuery:any;

@Component({
  selector: 'basic',
  templateUrl: 'basicLayout.template.html',
  host: {
    '(window:resize)': 'onResize()'
  }
})
export class BasicLayoutComponent {

  // @ViewChild('inactivityModal') inactivityModal;
  // public inactivityModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
  //     "ACTIONS.sessionExpiredTitle",
  //     "inactivityModal",
  //     true,
  //     false,
  //     "HOME.logout",
  //     "ACTIONS.cancel",
  //     false
  // );

  constructor(private authService: AuthenticationService,
              private router: Router){
  }

  public ngOnInit():any {
    detectBody();

      jQuery.extend(jQuery.fn.dataTableExt.oSort, {
        "date-uk-pre": function (a) {
          if (a === null || a === "") return false;

          var ukDatea = a.split('/');
          return (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
        },

        "date-uk-asc": function (a, b) {
          if (a === false && b === false) {
            return 0;
          } else if (a === false) {
            return 1;
          } else if (b === false) {
            return -1;
          } else{
            return ((a < b) ? -1 : ((a > b) ? 1 : 0));
          }
        },

        "date-uk-desc": function (a, b) {
          if (a === false && b === false) {
            return 0;
          } else if (a === false) {
            return 1;
          } else if (b === false) {
            return -1;
          } else{
            return ((a < b) ? 1 : ((a > b) ? -1 : 0));
          }
        },

        "non-empty-string-asc": function (str1, str2) {
          if(str1 == "")
              return 1;
          if(str2 == "")
              return -1;
          return ((str1 < str2) ? -1 : ((str1 > str2) ? 1 : 0));
      },
   
      "non-empty-string-desc": function (str1, str2) {
          if(str1 == "")
              return 1;
          if(str2 == "")
              return -1;
          return ((str1 < str2) ? 1 : ((str1 > str2) ? -1 : 0));
      }
    });
  }

  public onResize(){
    detectBody();
  }

  // handleInactivityCallback(){
  //   localStorage.setItem('mustLogout', "true");
  //   this.inactivityModal.show();
  // }

  // accept(){
  //   this.inactivityModal.hide();

  //   setTimeout(() => {
  //     window.location.reload();
  //   }, 500);

  //   this.authService.logout();
  //   this.router.navigate(['/login']);
  // }
}
