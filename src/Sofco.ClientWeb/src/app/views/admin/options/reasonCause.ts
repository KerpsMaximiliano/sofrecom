import { Component, OnInit } from "@angular/core";
import { GenericOptionComponent } from "./genereic-options";
import { GenericOptions } from "app/models/enums/genericOptions";
import { DataTableService } from "app/services/common/datatable.service";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { MessageService } from "app/services/common/message.service";
import { Validators, FormControl } from "@angular/forms";

@Component({
    selector: 'reason-cause-admin',
    templateUrl: './generic-options.html'
})
export class ReasonCauseComponent extends GenericOptionComponent implements OnInit {

    type = new FormControl('1', [Validators.required]);

    constructor(public dataTableService: DataTableService,
        public genericOptionService: GenericOptionService,
        public messageService: MessageService) {
            super(dataTableService, genericOptionService, messageService)
    }

    ngOnInit(): void {
        this.setEntity(GenericOptions.ReasonCause);
        this.title = "Motivos / Causas";
        this.columns = [0, 1, 2, 3];
    }

    edit(item){
        this.type.setValue(item.type.toString());
        this.update(item);
    }

    create() {
        var json = {
            id: this.id,
            text: this.text.value,
            parameters: {
                'type': this.type.value
            }
        };

        if(this.id == 0){
            this.save(json, this.saveCallback);
        }
        else{
            this.save(json, this.updateCallback);
        }
    }

    saveCallback(self, id){
        self.list.push({ id: id, text: self.text.value, active: true, type: self.type.value });
    }

    updateCallback(self, item){
        item.type = self.type.value;
    }
}