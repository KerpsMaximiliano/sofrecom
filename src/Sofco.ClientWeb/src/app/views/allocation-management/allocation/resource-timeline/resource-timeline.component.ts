import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";

declare var google: any;
declare var self: any;

@Component({
    selector: 'resource-timeline',
    templateUrl: './resource-timeline.component.html'
})
export class ResourceTimelineComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    getAllSubscrip: Subscription;
    showTimeLine: boolean = false;

    constructor(private analyticService: AnalyticService,
                private dataTableService: DataTableService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void { 
        self.this = this;
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    getAllocations(analyticId){
        this.showTimeLine = false;

        this.getAllSubscrip = this.analyticService.getResources(analyticId).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            if(data.data && data.data.length > 0){
                this.model = data.data;
                this.configChart();
            }
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    configChart(){
        this.showTimeLine = true;

        setTimeout(() => {
            google.charts.load("current", {packages:["timeline"]});
            google.charts.setOnLoadCallback(this.drawChart);
        }, 500);
    }

    drawChart() {
        var container = document.getElementById('example');
        var chart = new google.visualization.Timeline(container);
        var dataTable = new google.visualization.DataTable();
        dataTable.addColumn({ type: 'string', id: 'title'});
        dataTable.addColumn({ type: 'string', id: 'name', p: { html: true } });
        dataTable.addColumn({ type: 'date', id: 'Start' });
        dataTable.addColumn({ type: 'date', id: 'End' });

        var rows = [];
        var resourcesName = [];

        self.this.model.forEach(function(item, index){
            var startDate = new Date(item.startDate);
            var endDate = new Date(item.endDate);

            var row = [ item.resource, `${item.percentage}%`, startDate, endDate ] 
            rows.push(row);

            if(!resourcesName.includes(item.resource)){
                resourcesName.push(item.resource);
            }
        });

        dataTable.addRows(rows);
    
        var height = 350;

        if(resourcesName.length <= 5){
            height = 100 + (35 * (resourcesName.length-1));
        }

        var options = {
            timeline: { colorByRowLabel: true },
            enableInteractivity: false,
            height: height
        };
    
        chart.draw(dataTable, options);
    }
}

