import { Component, OnInit } from "@angular/core";
import { GenericOptionComponent } from "./genereic-options";
import { GenericOptions } from "app/models/enums/genericOptions";
import { DataTableService } from "app/services/common/datatable.service";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'profile-admin',
    templateUrl: './generic-options.html'
})
export class ProfileComponent extends GenericOptionComponent implements OnInit {

    constructor(public dataTableService: DataTableService,
        public genericOptionService: GenericOptionService,
        public messageService: MessageService) {
            super(dataTableService, genericOptionService, messageService)
    }

    ngOnInit(): void {
        this.setEntity(GenericOptions.Profile);
        this.title = "Perfiles";
    }

    create() {
        var json = {
            id: this.id,
            text: this.text.value
        };

        this.save(json);
    }
}