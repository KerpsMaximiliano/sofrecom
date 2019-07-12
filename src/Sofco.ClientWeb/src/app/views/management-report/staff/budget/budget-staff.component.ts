import { Component, OnDestroy, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import { FormControl, Validators } from "@angular/forms";

@Component({
    selector: 'budget-staff',
    templateUrl: './budget-staff.component.html',
    styleUrls: ['./budget-staff.component.scss']
})
export class BudgetStaffComponent implements OnInit, OnDestroy {
   
    months: any[] = new Array()

    
  
    ngOnInit(): void {
       
        this.months = [
            {
                display: "May. 2019", 
                monthYear: "2019-05-01"
            },
            {
                display: "Jun. 2019", 
                monthYear: "2019-06-01"
            },
            {
                display: "Jul. 2019", 
                monthYear: "2019-07-01"
            },
            {
                display: "Ago. 2019", 
                monthYear: "2019-08-01"
            },
            {
                display: "Sep. 2019", 
                monthYear: "2019-09-01"
            },
            {
                display: "Oct. 2019", 
                monthYear: "2019-10-01"
            },
            {
                display: "Nov. 2019", 
                monthYear: "2019-11-01"
            },
        ]

    }

    ngOnDestroy(): void {
       
    }

}