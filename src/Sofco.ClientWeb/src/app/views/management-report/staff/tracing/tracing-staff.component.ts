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
      
    }

    ngOnDestroy(): void {
    }
   

    calculateAccumulated(months){
        this.months = months
        this.accumulatedMonths = new Array()

        months.forEach((month, index) => {
            let acomulatedBudget = 0
            let acomulatedPfa1 = 0
            let acomulatedPfa2 = 0
            let acomulatedReal = 0

            for (let i = 0; i <= index; i++) {

                acomulatedBudget += months[i].budget.totalCost
                acomulatedPfa1 += months[i].pfa1.totalCost
                acomulatedPfa2 += months[i].pfa2.totalCost
                acomulatedReal += months[i].real.totalCost
            }

            var acomulatedMont = {
                display: month.display,
                monthYear: month.monthYear,
                totalBudget: month.budget.totalCost,
                totalPfa1: month.pfa1.totalCost,
                totalPfa2: month.pfa2.totalCost,
                totalReal: month.real.totalCost,
                acomulatedBudget: acomulatedBudget,
                acomulatedPfa1: acomulatedPfa1,
                acomulatedPfa2: acomulatedPfa2,
                acomulatedReal: acomulatedReal,
            }

            this.accumulatedMonths.push(acomulatedMont)
        })
    }

}