import { Component, OnInit } from "@angular/core";
import { GenericOptionComponent } from "./genereic-options";
import { GenericOptions } from "app/models/enums/genericOptions";
import { DataTableService } from "app/services/common/datatable.service";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { MessageService } from "app/services/common/message.service";
import { Validators, FormControl } from "@angular/forms";

@Component({
    selector: 'cost-type-admin',
    templateUrl: './generic-options.html'
})
export class CostTypeComponent extends GenericOptionComponent implements OnInit {

    category = new FormControl(null, [Validators.required]);

    constructor(public dataTableService: DataTableService,
        public genericOptionService: GenericOptionService,
        public messageService: MessageService) {
            super(dataTableService, genericOptionService, messageService)
    }

    ngOnInit(): void {
        this.setEntity(GenericOptions.CostType);
        this.title = "Tipos de costos";
        this.columns = [0, 1, 2, 3];
    }

    edit(item){
        this.category.setValue(item.category.toString());
        this.update(item);
    }

    create() {
        var json = {
            id: this.id,
            text: this.text.value,
            parameters: {
                'category': this.category.value
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
        self.list.push({ id: id, text: self.text.value, active: true, category: self.category.value });
    }

    updateCallback(self, item){
        item.category = self.category.value;
    }
}