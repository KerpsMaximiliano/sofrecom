import { Component, OnDestroy, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import { FormControl, Validators } from "@angular/forms";
import { months } from "moment";
import { DebugContext } from "@angular/core/src/view";

@Component({
    selector: 'tracing-staff',
    templateUrl: './tracing-staff.component.html',
    styleUrls: ['./tracing-staff.component.scss']
})

export class TracingStaffComponent implements OnInit, OnDestroy {
   
    months: any[] = new Array()
    accumulatedMonths: any[] = new Array()

   
    ngOnInit(): void {
       
        this.months = [
            {
                display: "May. 2019",
                monthYear: "2019-05-01",
                totalBudget: 0,
                totalPfa1: 0,
                totalPfa2: 0,
                totalReal: 0
            },
            {
                display: "Jun. 2019",
                monthYear: "2019-06-01",
                totalBudget: 0,
                totalPfa1: 0,
                totalPfa2: 0,
                totalReal: 0
            },
            {
                display: "Jul. 2019",
                monthYear: "2019-07-01",
                totalBudget: 0,
                totalPfa1: 0,
                totalPfa2: 0,
                totalReal: 0
            },
            {
                display: "Ago. 2019",
                monthYear: "2019-08-01",
                totalBudget: 0,
                totalPfa1: 0,
                totalPfa2: 0,
                totalReal: 0
            },
            {
                display: "Sep. 2019",
                monthYear: "2019-09-01",
                totalBudget: 0,
                totalPfa1: 0,
                totalPfa2: 0,
                totalReal: 0
            },
            {
                display: "Oct. 2019",
                monthYear: "2019-10-01",
                totalBudget: 0,
                totalPfa1: 0,
                totalPfa2: 0,
                totalReal: 0
            },
            {
                display: "Nov. 2019",
                monthYear: "2019-11-01",
                totalBudget: 0,
                totalPfa1: 0,
                totalPfa2: 0,
                totalReal: 0
            },
        ]

        this.calculateAccumulated(this.months)
    }

    ngOnDestroy(): void {
    }
   

    calculateAccumulated(months){
        this.accumulatedMonths = new Array()

        months.forEach((month, index) => {

            let acomulatedBudget = 0
            let acomulatedPfa1 = 0
            let acomulatedPfa2 = 0
            let acomulatedReal = 0

            for (let i = 0; i <= index; i++) {

                acomulatedBudget += months[i].totalBudget;
                acomulatedPfa1 += months[i].totalPfa1;
                acomulatedPfa2 += months[i].totalPfa2;
                acomulatedReal += months[i].totalReal;
            }

            var acomulatedMont = {
                display: month.display,
                monthYear: month.monthYear,
                totalBudget: month.totalBudget,
                totalPfa1: month.totalPfa1,
                totalPfa2: month.totalPfa2,
                totalReal: month.totalReal,
                acomulatedBudget: acomulatedBudget,
                acomulatedPfa1: acomulatedPfa1,
                acomulatedPfa2: acomulatedPfa2,
                acomulatedReal: acomulatedReal,
            }

            this.accumulatedMonths.push(acomulatedMont)
        })
    }

}