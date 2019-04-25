import { Component, OnDestroy, OnInit } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { WorkflowStateType } from "app/models/enums/workflowStateType";
import { Router } from "@angular/router";

declare var moment: any;

@Component({
    selector: 'list-in-process',
    templateUrl: './list-in-process.component.html',
    styleUrls: ['./list-in-process.component.scss']
})
export class AdvancementListInProcessComponent implements OnInit, OnDestroy {

    getSubscrip: Subscription;

    public model: any[] = new Array();
    public modelFiltered: any[] = new Array();
   
    constructor(private advancementService: AdvancementService,
        private router: Router,
        private datatableService: DataTableService,
        private messageService: MessageService) { }

    ngOnInit(): void {
        this.messageService.showLoading();

        this.getSubscrip = this.advancementService.getAllInProcess().subscribe(response => {
            this.messageService.closeLoading();

            const data = JSON.parse(sessionStorage.getItem('lastAdvancementQuery'));

            this.model = response.data;
     
            if(data){
                this.search(data);
            }
            else{
                this.modelFiltered = response.data;
                this.initGrid();
            }
        },
        () => this.messageService.closeLoading());
    }

    ngOnDestroy(): void {
        if (this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    initGrid() {
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        var title = `Adelantos-en-proceso`;

        var params = {
            selector: '#advancements',
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [{ "aTargets": [4], "sType": "date-uk" }],
            currencyColumns: [6]
        }

        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);

        setTimeout(() => {
            $("#advancements_wrapper").css("float", "left");
            $("#advancements_wrapper").css("padding-bottom", "50px");
            $("#advancements_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#advancements_paginate").addClass('table-pagination');
            $("#advancements_length").css("margin-right", "10px");
            $("#advancements_info").css("padding-top", "4px");
        }, 500);
    }

    goToDetail(item) {
        this.router.navigate(['/advancementAndRefund/advancement/' + item.id]);
    }

    getStatusClass(type) {
        switch (type) {
            case WorkflowStateType.Info: return "label-success";
            case WorkflowStateType.Warning: return "label-warning";
            case WorkflowStateType.Success: return "label-primary";
            case WorkflowStateType.Danger: return "label-danger";
        }
    }

    search(parameters) {
        this.modelFiltered = [];
        sessionStorage.setItem('lastAdvancementQuery', JSON.stringify(parameters));

        if (!parameters.resourceId && !parameters.typeId && !parameters.dateSince && !parameters.dateTo && !parameters.stateId && !parameters.bank) {
            this.modelFiltered = this.model;
        }
        else {
            for (var i = 0; i < this.model.length; i++) {
                var addItem = true;
                var item = this.model[i];

                if (parameters.resourceId && parameters.resourceId > 0) {
                    if (parameters.resourceId != item.userApplicantId) {
                        addItem = false;
                    }
                }

                if (parameters.typeId && parameters.typeId > 0) {
                    if (parameters.typeId != item.type) {
                        addItem = false;
                    }
                }

                if (parameters.stateId && parameters.stateId > 0) {
                    if (parameters.stateId != item.statusId) {
                        addItem = false;
                    }
                }

                if (parameters.dateSince) {
                    parameters.dateSince.setHours(0, 0, 0, 0);

                    if (moment(item.creationDate).toDate() < parameters.dateSince) {
                        addItem = false;
                    }
                }

                if (parameters.dateTo) {
                    parameters.dateTo.setHours(0, 0, 0, 0);

                    if (parameters.dateSince) {
                        if (moment(item.creationDate).toDate() < parameters.dateSince || moment(item.creationDate).toDate() > parameters.dateTo) {
                            addItem = false;
                        }
                    }
                    else {
                        if (moment(item.creationDate).toDate() > parameters.dateTo) {
                            addItem = false;
                        }
                    }
                }

                if (parameters.bank) {
                    if (parameters.bank != item.bank) {
                        addItem = false;
                    }
                }

                if (addItem) {
                    this.modelFiltered.push(item);
                }
            }
        }

        if (this.modelFiltered.length == 0) {
            this.messageService.showWarningByFolder('common', 'searchNotFound');
        }

        this.initGrid();
    }
   
}


