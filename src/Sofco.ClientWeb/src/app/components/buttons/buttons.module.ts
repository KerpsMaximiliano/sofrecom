import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { NgModule } from "@angular/core";
import { TranslateModule } from "@ngx-translate/core";
import { ButtonConfirmInlineComponent } from "./button-confirm-inline/btn-confirm-inline";
import { LaddaModule } from "angular2-ladda";
import { I18nService } from "../../services/common/i18n.service";
import { ButtonConfirmComponent } from "./button-confirm/btn-confirm";
import { MessageService } from "../../services/common/message.service";

@NgModule({
    imports: [
      CommonModule, FormsModule, TranslateModule, LaddaModule
    ],
    declarations: [ 
      ButtonConfirmInlineComponent, ButtonConfirmComponent
    ],
    providers: [
      I18nService, MessageService
    ],
    exports: [
      ButtonConfirmInlineComponent, ButtonConfirmComponent
    ]
  })
  export class ButtonsModule { }
  