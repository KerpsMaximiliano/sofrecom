import { Component, ViewChild } from '@angular/core';
import { BaseChartDirective } from 'ng2-charts/ng2-charts';

@Component({
    selector: 'solfac-chart',
    templateUrl: './solfac-chart.component.html'
})
export class SolfacChartComponent {
    @ViewChild(BaseChartDirective) chart: BaseChartDirective;

    public barChartOptions:any = {
        scaleShowVerticalLines: false,
        responsive: true,
        scales: {
            yAxes: [{
                ticks: { min: 0 }
            }]
        }
    };

    public barChartLabels:string[] = [];
    public barChartType:string = 'bar';
    public chartColors: Array<any> = [
        {
            backgroundColor: 'rgba(63,191,63, 0.8)',
            borderColor: 'rgba(192, 240, 192, 0.8)',
            borderWidth: 1,
            hoverBackgroundColor: 'rgba(64, 247, 140, 0.8)',
            hoverBorderColor: 'rgba(33, 145, 33, 0.5)',
            hoverBorderWidth: 1
        },
        {
            backgroundColor: 'rgba(63, 80, 191, 0.8)',
            borderColor: 'rgba(192, 240, 192, 0.8)',
            borderWidth: 1,
            hoverBackgroundColor: 'rgba(64, 247, 140, 0.8)',
            hoverBorderColor: 'rgba(33, 145, 33, 0.5)',
            hoverBorderWidth: 1
        },
        {
            backgroundColor: 'rgba(226, 33, 237, 0.8)',
            borderColor: 'rgba(192, 240, 192, 0.8)',
            borderWidth: 1,
            hoverBackgroundColor: 'rgba(64, 247, 140, 0.8)',
            hoverBorderColor: 'rgba(33, 145, 33, 0.5)',
            hoverBorderWidth: 1
        },
        {
            backgroundColor: 'rgba(237, 118, 33, 0.8)',
            borderColor: 'rgba(192, 240, 192, 0.8)',
            borderWidth: 1,
            hoverBackgroundColor: 'rgba(64, 247, 140, 0.8)',
            hoverBorderColor: 'rgba(33, 145, 33, 0.5)',
            hoverBorderWidth: 1
        },
        {
            backgroundColor: 'rgba(35, 206, 209, 0.8)',
            borderColor: 'rgba(192, 240, 192, 0.8)',
            borderWidth: 1,
            hoverBackgroundColor: 'rgba(64, 247, 140, 0.8)',
            hoverBorderColor: 'rgba(33, 145, 33, 0.5)',
            hoverBorderWidth: 1
        },
        {
            backgroundColor: 'rgba(109, 26, 204, 0.8)',
            borderColor: 'rgba(192, 240, 192, 0.8)',
            borderWidth: 1,
            hoverBackgroundColor: 'rgba(64, 247, 140, 0.8)',
            hoverBorderColor: 'rgba(33, 145, 33, 0.5)',
            hoverBorderWidth: 1
        },
        {
            backgroundColor: 'rgba(220, 34, 34, 0.8)',
            borderColor: 'rgba(192, 240, 192, 0.8)',
            borderWidth: 1,
            hoverBackgroundColor: 'rgba(64, 247, 140, 0.8)',
            hoverBorderColor: 'rgba(33, 145, 33, 0.5)',
            hoverBorderWidth: 1
        },
        {
            backgroundColor: 'rgba(8, 18, 200, 0.8)',
            borderColor: 'rgba(192, 240, 192, 0.8)',
            borderWidth: 1,
            hoverBackgroundColor: 'rgba(64, 247, 140, 0.8)',
            hoverBorderColor: 'rgba(33, 145, 33, 0.5)',
            hoverBorderWidth: 1
        },
        {
            backgroundColor: 'rgba(109, 200, 18, 0.8)',
            borderColor: 'rgba(192, 240, 192, 0.8)',
            borderWidth: 1,
            hoverBackgroundColor: 'rgba(64, 247, 140, 0.8)',
            hoverBorderColor: 'rgba(33, 145, 33, 0.5)',
            hoverBorderWidth: 1
        },
      ];

    public barChartData:any[] = [{data: [0], label: ''}];

    public setData(labels:string[], data:any){
        this.barChartLabels = labels;
        this.barChartData = data;
        
        this.applyDatasetColor(data);
        this.chart.chart.config.data.labels = this.barChartLabels;
        this.chart.chart.config.data.datasets = data;
        this.chart.chart.update();
    }

    private applyDatasetColor(data:Array<any>):Array<any>
    {
        for(let i=0; i<data.length;i++){
            var cfgIndex = i>=this.chartColors.length?0:i;

            var chartColors = this.chartColors[cfgIndex];
            for(let key in chartColors){
                data[i][key] = chartColors[key];
            }
        }
        return data;
    }
}