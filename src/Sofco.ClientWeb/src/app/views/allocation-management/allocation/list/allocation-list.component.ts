import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";

declare var google: any;
declare var self: any;

@Component({
    selector: 'allocation-list',
    templateUrl: './allocation-list.component.html'
})
export class AllocationListComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    getAllSubscrip: Subscription;
    analyticStartDate: Date;
    analyticEndDate: Date;
    showTimeLine: boolean = false;

    constructor(private allocationService: AllocationService,
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

    getAllocations(employeeId, startDate, endDate){
        this.analyticStartDate = startDate;
        this.analyticEndDate = endDate;
        this.showTimeLine = false;

        this.getAllSubscrip = this.allocationService.getAll(employeeId, startDate, endDate).subscribe(data => {

            if(data && data.length > 0){
                this.model = data;

                this.configChart();
            }
            else{
                this.configChart();
                this.messageService.succes(this.i18nService.translate('allocationManagement.allocation.emptyMessage'));
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

            var row = [ item.analyticTitle, `${item.percentage}%`, getTooltipHtml(item.analyticName, item.percentage, startDate, endDate), startDate, endDate ] 
            rows.push(row);
        });

        rows.push([ self.i18nService.translate("allocationManagement.allocation.analyticLife"), self.i18nService.translate("allocationManagement.allocation.analyticCicleLife"), 
                    getTooltipHtml(self.i18nService.translate("allocationManagement.allocation.analyticLife"), self.i18nService.translate("allocationManagement.allocation.analyticCicleLife"), 
                    new Date(self.analyticStartDate), new Date(self.analyticEndDate)), 
                    new Date(self.analyticStartDate), new Date(self.analyticEndDate)])

        dataTable.addRows(rows);
    
        var options = {
            timeline: { colorByRowLabel: true }
        };
    
        chart.draw(dataTable, options);
        
        function getTooltipHtml(analytic, percentage, startDate, endDate){
            return `
                <div class="google-visualization-tooltip" style="width: 240px; height: 157px">
                    <ul class="google-visualization-tooltip-item-list" style="">
                        <li class="google-visualization-tooltip-item" style="">
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none; font-weight: bold;">${analytic}</span>
                        </li>
                    </ul>
                    <div class="google-visualization-tooltip-separator" style=""></div>
                    <ul class="google-visualization-tooltip-action-list" style="">
                        <li class="google-visualization-tooltip-action" style="">
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none; font-weight: bold;">Fecha Inicio:</span>
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none;">${startDate.toLocaleDateString()}</span>
                        </li>
                        <li class="google-visualization-tooltip-action" style="">
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none; font-weight: bold;">Fecha Inicio:</span>
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none;">${endDate.toLocaleDateString()}</span>
                        </li>
                        <li class="google-visualization-tooltip-action" style="">
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none; font-weight: bold;">Porcentaje:</span>
                            <span style="font-family: Arial; font-size: 12px; color: rgb(0, 0, 0); opacity: 1; margin: 0px; text-decoration: none;">${percentage}%</span>
                        </li>
                    </ul>
            </div>`;
        }
    }
}

