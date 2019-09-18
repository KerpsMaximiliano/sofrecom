import { Component, OnInit } from "@angular/core";
import { GenericOptionComponent } from "./genereic-options";
import { GenericOptions } from "app/models/enums/genericOptions";
import { DataTableService } from "app/services/common/datatable.service";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'time-hiring-admin',
    templateUrl: './generic-options.html'
})
export class TimeHiringComponent extends GenericOptionComponent implements OnInit {

    constructor(public dataTableService: DataTableService,
        public genericOptionService: GenericOptionService,
        public messageService: MessageService) {
            super(dataTableService, genericOptionService, messageService)
    }

    ngOnInit(): void {
        this.setEntity(GenericOptions.TimeHiring);
        this.title = "Tiempo de contratación";
        this.columns = [0, 1, 2];
    }

    edit(item){
        this.update(item);
    }

    create() {
        var json = {
            id: this.id,
            text: this.text.value
        };

        this.save(json);
    }
}