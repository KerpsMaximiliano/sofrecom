import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";

declare var google: any;
declare var self;

@Component({
    selector: 'allocation-list',
    templateUrl: './allocation-list.component.html'
})
export class AllocationListComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    getAllSubscrip: Subscription;

    constructor(private allocationService: AllocationService,
                private dataTableService: DataTableService,
                private messageService: MessageService,
                private i18nService: I18nService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void { 
        self = this;
        this.model = [ 
            { analyticTitle: "Titulo 1", percentage: "100", analyticName: "Analitica 1", startDate: new Date(2017, 10, 1), endDate: new Date(2017, 10, 30) },
            { analyticTitle: "Titulo 2", percentage: "100", analyticName: "Analitica 2", startDate: new Date(2017, 8, 1), endDate: new Date(2017, 10, 30) },
            { analyticTitle: "Titulo 1", percentage: "100", analyticName: "Analitica 1", startDate: new Date(2017, 8, 1), endDate: new Date(2017, 9, 30) },
            { analyticTitle: "Titulo 3", percentage: "100", analyticName: "Analitica 3", startDate: new Date(2017, 5, 1), endDate: new Date(2017, 11, 31) }
        ]

        google.charts.load("current", {packages:["timeline"]});
        google.charts.setOnLoadCallback(this.drawChart);
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    getAllocations(employeeId, startDate, endDate){
        //this.dataTableService.destroy('#allocationsTable');

        this.getAllSubscrip = this.allocationService.getAll(employeeId, startDate, endDate).subscribe(data => {

            if(data && data.length > 0){
                this.model = data;
                //this.dataTableService.init('#allocationsTable', false);

                google.charts.load("current", {packages:["timeline"]});
                google.charts.setOnLoadCallback(this.drawChart);
            }
            else{
                this.messageService.succes(this.i18nService.translate('allocationManagement.allocation.emptyMessage'));
            }
        },
        error => this.errorHandlerService.handleErrors(error));
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
            var row = [ item.analyticTitle, `${item.percentage}%`, getTooltipHtml(item.analyticName, item.percentage, item.startDate, item.endDate), item.startDate, item.endDate ] 
            rows.push(row);
        });

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

