import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { NgModule } from "@angular/core";
import { TranslateModule } from "@ngx-translate/core";
import { ButtonConfirmInlineComponent } from "app/components/buttons/button-confirm-inline/btn-confirm-inline";
import { LaddaModule } from "angular2-ladda";

@NgModule({
    imports: [
      CommonModule, FormsModule, TranslateModule, LaddaModule
    ],
    declarations: [ 
      ButtonConfirmInlineComponent
    ],
    providers: [
    ],
    exports: [
      ButtonConfirmInlineComponent
    ]
  })
  export class ButtonsModule { }
  