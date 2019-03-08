import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { TranslateModule } from "@ngx-translate/core";
import { ButtonsModule } from "../buttons/buttons.module";
import { WorkflowComponent } from "./workflow.component";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { MessageService } from "app/services/common/message.service";
import { WfRejectComponent } from "./templates/reject/wf-reject.component";
import { Ng2ModalModule } from "../modal/ng2modal.module";
import { FormsModule } from "@angular/forms";
import { WfCurrencyExchangeComponent } from "./templates/currency-exchange/currency-exchange";

@NgModule({
    declarations: [ WorkflowComponent, WfRejectComponent, WfCurrencyExchangeComponent ],

    imports     : [ CommonModule, TranslateModule, ButtonsModule, Ng2ModalModule, FormsModule ],

    providers   : [ WorkflowService, MessageService ],

    exports     : [ WorkflowComponent],

    entryComponents : [ WfRejectComponent, WfCurrencyExchangeComponent ] 
})

export class WorkflowModule {} 