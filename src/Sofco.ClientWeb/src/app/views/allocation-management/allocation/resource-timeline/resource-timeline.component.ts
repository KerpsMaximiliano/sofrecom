import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";

declare var google: any;
declare var self: any;
declare var moment: any;

@Component({
    selector: 'resource-timeline',
    templateUrl: './resource-timeline.component.html'
})
export class ResourceTimelineComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    getAllSubscrip: Subscription;
    showTimeLine = false;

    constructor(private analyticService: AnalyticService,
                private dataTableService: DataTableService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        self.this = this;
    }

    ngOnDestroy(): void {
        if (this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }
 
    getAllocations(analyticId, dateSince, months){
        this.model = [];
        this.showTimeLine = false;

        this.getAllSubscrip = this.analyticService.getTimelineResources(analyticId, moment(dateSince).format('YYYY-MM-DD'), months).subscribe(data => {
            if (data.messages) this.messageService.showMessages(data.messages);

            if (data.data && data.data.length > 0){
                this.model = data.data;
                this.configChart();
            }
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    configChart() {
        this.showTimeLine = true;

        setTimeout(() => {
            google.charts.load("current", {packages:["timeline"]});
            google.charts.setOnLoadCallback(this.drawChart);
        }, 500);
    }

    drawChart() {
        const container = document.getElementById('grid');
        const chart = new google.visualization.Timeline(container);
        const dataTable = new google.visualization.DataTable();
        dataTable.addColumn({ type: 'string', id: 'title', p: { html: true }});
        dataTable.addColumn({ type: 'string', id: 'name', p: { html: true } });
        dataTable.addColumn({ type: 'date', id: 'Start' });
        dataTable.addColumn({ type: 'date', id: 'End' });

        const rows = [];
        const resourcesName = [];
        const releaseDates = [];
        const releaseData = [];

        self.this.model.forEach(function(item, index){
            const startDate = new Date(item.startDate);
            const endDate = new Date(item.endDate);

            rows.push([ item.resource, `${item.percentage}%`, startDate, endDate ]);

            if (!resourcesName.includes(item.resource)) {
                resourcesName.push(item.resource);
                releaseData[item.employeeId] = item.releaseDate;
            } else {
                releaseData[item.employeeId] = item.releaseDate;
            }
        });
        for (const key in releaseData) {
            releaseDates.push(releaseData[key]);
        }

        dataTable.addRows(rows);

        let height = 350;

        if (resourcesName.length <= 5) {
            height = 110 + (35 * (resourcesName.length - 1));
        }

        const options = {
            timeline: {
                colorByRowLabel: true,
                rowLabelStyle: {fontName: 'inherit' },
                barLabelStyle: { fontName: 'inherit' }
            },
            enableInteractivity: false,
            showRowLabels: false,
            height: height
        };

        google.visualization.events.addOneTimeListener(chart, 'ready', function () {
            const rectangles = container.getElementsByTagName('rect');
            const adjustY = 37;
            const adjustX = 30;

            const labels = rectangles[0].parentElement.getElementsByTagName('text');

            for (let i = 0; i < resourcesName.length; i++) {
                const itemLabel: any = labels[i];
                const referenceElement: any = rectangles[i];
                const adjustedLabel = parseInt(itemLabel.getAttribute('y')) - 7;
                itemLabel.setAttribute('y', adjustedLabel.toString());
                itemLabel.setAttribute('x', '10');
                itemLabel.setAttribute('text-anchor', 'start');

                const text: any = document.createElementNS('http://www.w3.org/2000/svg', 'text');
                text.textContent = 'Fecha de liberación: ' + moment(releaseDates[i]).format("DD/MM/YYYY");
                text.setAttribute('x', (parseInt(referenceElement.getAttribute('x')) + adjustX));
                text.setAttribute('y', (parseInt(referenceElement.getAttribute('y')) + adjustY));
                text.setAttribute('width', parseInt(referenceElement.getAttribute('width')));
                text.setAttribute('height', 35);
                text.setAttribute('font-size', '11px');
                text.setAttribute('fill', '#7E7E7E');
                referenceElement.parentElement.appendChild(text);
            }
        });

        chart.draw(dataTable, options);
    }
}

