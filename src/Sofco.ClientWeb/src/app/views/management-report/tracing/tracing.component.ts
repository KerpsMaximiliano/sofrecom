import { Component, OnInit, OnDestroy } from "@angular/core";

@Component({
    selector: 'management-report-tracing',
    templateUrl: './tracing.component.html',
    styleUrls:['./tracing.module.scss']
})
export class TracingComponent implements OnInit, OnDestroy {
    
    AllMarginTracking: any[] = new Array()

    
    constructor(){

    }
    
    ngOnInit(): void {
       
    }
    ngOnDestroy(): void {
        
    }

    open(marginTracking){

        this.AllMarginTracking = marginTracking
        
        this.AllMarginTracking.forEach(margin => {
            margin.display = this.getDateShortDescrip(margin.Month, margin.Year)
        })
    }

    getDateShortDescrip(month, year){
 
        switch (month)
        {
            case 1: return `Ene. ${year}`;
            case 2: return `Feb. ${year}`;
            case 3: return `Mar. ${year}`;
            case 4: return `Abr. ${year}`;;
            case 5: return `May. ${year}`;
            case 6: return `Jun.${year}`;
            case 7: return `Jul. ${year}`;
            case 8: return `Ago. ${year}`;
            case 9: return `Sep. ${year}`;
            case 10: return `Oct. ${year}`;
            case 11: return `Nov. ${year}`;
            case 12: return `Dic. ${year}`;
            default: return '';
        }
    }
}