import { Component, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ApplicantService } from "app/services/recruitment/applicant.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import * as FileSaver from "file-saver";

@Component({
    selector: 'contact-files',
    templateUrl: './contact-files.html',
})
export class ContactFileComponent implements OnDestroy {   
    files: any[] = new Array();

    getFileSubscrip: Subscription;

    constructor(private applicantService: ApplicantService, 
        private messageService: MessageService,
        private dataTableService: DataTableService){
    }
    
    ngOnDestroy(): void {
        if (this.getFileSubscrip) this.getFileSubscrip.unsubscribe();
    }

    getFiles(applicantId){
        this.files = [];

        this.getFileSubscrip = this.applicantService.getFiles(applicantId).subscribe(response => {
          this.files = response.data;
    
          this.initGrid();
        });
    }

    initGrid() {
        var columns = [0, 1, 2, 3];

        var options = {
            selector: "#filesTable",
            columns: columns,
            columnDefs: [ { "aTargets": [2], "sType": "date-uk" }],
        };

        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }

    export(item){
        this.messageService.showLoading();

        this.applicantService.getFile(item.id).subscribe(response => {
            this.messageService.closeLoading();

            FileSaver.saveAs(response, item.name);
        },
        () => this.messageService.closeLoading());
    }
} 