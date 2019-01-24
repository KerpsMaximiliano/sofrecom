import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { Router } from "@angular/router";
import { WorkflowStateType } from "app/models/enums/workflowStateType";

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
                private datatableService: DataTableService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.messageService.showLoading();

        this.getSubscrip = this.advancementService.getAllInProcess().subscribe(response => {
            this.messageService.closeLoading();

            this.model = response.data;
            this.modelFiltered = response.data;
            this.initGrid();
        }, 
        error => this.messageService.closeLoading());
    }
                
    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
        var title = `Adelantos-en-proceso`;

        var params = {
          selector: '#advancements',
          columns: columns,
          title: title,
          withExport: true,
          columnDefs: [ {"aTargets": [2], "sType": "date-uk"} ]
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);

        setTimeout(() => {
            $("#advancements_wrapper").css("float","left");
            $("#advancements_wrapper").css("padding-bottom","50px");
            $("#advancements_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#advancements_paginate").addClass('table-pagination');
            $("#advancements_length").css("margin-right","10px");
            $("#advancements_info").css("padding-top","4px");
        }, 500);
    }

    goToDetail(item){
        this.router.navigate(['/advancementAndRefund/advancement/' + item.id])
    }

    getStatusClass(type){
        switch(type){
            case WorkflowStateType.Info: return "label-success";
            case WorkflowStateType.Warning: return "label-warning";
            case WorkflowStateType.Success: return "label-primary";
            case WorkflowStateType.Danger: return "label-danger";
        }
    }

    search(parameters){
        this.modelFiltered = [];

        if(!parameters.resourceId && !parameters.typeId && !parameters.dateSince && !parameters.dateTo){
            this.modelFiltered = this.model;
        }
        else{
            for(var i = 0; i < this.model.length; i++){
                var addItem = true;
                var item = this.model[i];

                if(parameters.resourceId && parameters.resourceId > 0){
                    if(parameters.resourceId != item.userApplicantId){
                        addItem = false;
                    }
                }

                if(parameters.typeId && parameters.typeId > 0){
                    if(parameters.typeId != item.type){
                        addItem = false;
                    }
                }

                if(parameters.dateSince){
                    parameters.dateSince.setHours(0,0,0,0);

                    if(moment(item.creationDate).toDate() < parameters.dateSince){
                        addItem = false;
                    }
                }

                if(parameters.dateTo){
                    parameters.dateTo.setHours(0,0,0,0);

                    if(parameters.dateSince){
                        if(moment(item.creationDate).toDate() < parameters.dateSince || moment(item.creationDate).toDate() > parameters.dateTo){
                            addItem = false;
                        }
                    }
                    else{
                        if(moment(item.creationDate).toDate() > parameters.dateTo){
                            addItem = false;
                        }
                    }
                }

                if(addItem){
                    this.modelFiltered.push(item);
                }
            }
        }

        if(this.modelFiltered.length == 0){
            this.messageService.showWarningByFolder('common','searchNotFound');
        }

        this.initGrid();
    }
}