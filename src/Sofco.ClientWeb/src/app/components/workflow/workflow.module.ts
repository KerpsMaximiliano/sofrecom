import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { TranslateModule } from "@ngx-translate/core";
import { ButtonsModule } from "../buttons/buttons.module";
import { WorkflowComponent } from "./workflow.component";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { MessageService } from "app/services/common/message.service";

@NgModule({
    declarations: [ WorkflowComponent ],

    imports     : [ CommonModule, TranslateModule, ButtonsModule ],

    providers   : [ WorkflowService, MessageService ],

    exports     : [ WorkflowComponent]
})

export class WorkflowModule {} 