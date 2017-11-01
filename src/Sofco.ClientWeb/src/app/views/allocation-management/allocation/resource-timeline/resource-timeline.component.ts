import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";
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
                private i18nService: I18nService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void { 
        self = this;
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
        dataTable.addColumn({ type: 'string', role: 'tooltip', p: { html: true }});
        dataTable.addColumn({ type: 'date', id: 'Start' });
        dataTable.addColumn({ type: 'date', id: 'End' });

        var rows = [];

        self.model.forEach(function(item, index){
            var startDate = new Date(item.startDate);
            var endDate = new Date(item.endDate);
            var releaseDate = new Date(item.releaseDate);

            var row = [ item.resource, `${item.percentage}%`, getTooltipHtml(item.resource, item.percentage, startDate, endDate, releaseDate), startDate, endDate ] 
            rows.push(row);
        });

        dataTable.addRows(rows);
    
        var options = {
            timeline: { colorByRowLabel: true }
        };
    
        chart.draw(dataTable, options);
        
        function getTooltipHtml(resource, percentage, startDate, endDate, releaseDate){
            return `
                <div class="google-visualization-tooltip" style="width: 250px; height: 200px">
                    <ul class="google-visualization-tooltip-item-list" style="">
                        <li class="google-visualization-tooltip-item" style="">
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none; font-weight: bold;">${resource}</span>
                        </li>
                    </ul>
                    <div class="google-visualization-tooltip-separator" style=""></div>
                    <ul class="google-visualization-tooltip-action-list" style="">
                        <li class="google-visualization-tooltip-action" style="">
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none; font-weight: bold;">Fecha Inicio:</span>
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none;">${startDate.toLocaleDateString()}</span>
                        </li>
                        <li class="google-visualization-tooltip-action" style="">
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none; font-weight: bold;">Fecha Fin:</span>
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none;">${endDate.toLocaleDateString()}</span>
                        </li>
                        <li class="google-visualization-tooltip-action" style="">
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none; font-weight: bold;">Porcentaje:</span>
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none;">${percentage}%</span>
                        </li>
                        <li class="google-visualization-tooltip-action" style="">
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none; font-weight: bold;">Fecha de Liberación:</span>
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none;">${releaseDate.toLocaleDateString()}</span>
                        </li>
                    </ul>
            </div>`;
        }
    }
}

