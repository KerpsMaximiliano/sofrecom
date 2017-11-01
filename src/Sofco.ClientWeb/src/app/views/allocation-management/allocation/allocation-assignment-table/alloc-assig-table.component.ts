import { OnDestroy, Component, OnInit, Input, ViewChild } from "@angular/core";
import { AllocationModel, Allocation } from "app/models/allocation-management/allocation";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { I18nService } from "app/services/common/i18n.service";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

@Component({
    selector: 'allocation-assignment-table',
    templateUrl: './alloc-assig-table.component.html',
    styleUrls: ['./alloc-assig-table.component.scss']
})
export class AllocationAssignmentTableComponent implements OnInit, OnDestroy {

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    ); 

    totals: any[] = new Array<any>();
    model: AllocationModel;

    @Input() resourceId: number;
    @Input() analytic: any;

    getAllAllocationsSubscrip: Subscription;
    addSubscrip: Subscription;

    releaseDate: Date = new Date();
    public options;

    public allocationSelected: any;

    constructor(private menuService: MenuService,
        private i18nService: I18nService,
        private allocationsService: AllocationService,
        private messageService: MessageService,
        private employeeService: EmployeeService,
        private errorHandlerService: ErrorHandlerService){

            this.options = this.menuService.getDatePickerOptions();
    }

    ngOnInit(): void {
        if(this.resourceId > 0){
            this.getAllocations(this.resourceId);
        }
    }

    ngOnDestroy(): void {
        if(this.getAllAllocationsSubscrip) this.getAllAllocationsSubscrip.unsubscribe();
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    getAllocations(resourceId){
        var today = new Date();
        today.setDate(1);

        var todayPlus12Months = new Date(today.getFullYear()+1, today.getMonth()-1, 1);

        this.getAllAllocationsSubscrip = this.allocationsService.getAllocations(resourceId, today.toUTCString(), todayPlus12Months.toUTCString()).subscribe(data => {
            this.model = data;

            if(this.model.allocations.length == 0){
                this.messageService.showWarning(this.i18nService.translate("allocationManagement.allocation.emptyMessage"));

                if(this.analytic){
                    this.add(this.analytic);
                }
            }
            
            this.model.monthsHeader.forEach((item, index) => {
                this.totals.push(0);

                this.updateTotal(0, index);
            });
        },
        error => this.errorHandlerService.handleErrors(error));
    }
 
    updateMonth(value, monthIndex, month){
        month.updated = true;
        this.updateTotal(value, monthIndex);
    }

    updateTotal(value, monthIndex){
        var total = 0;

        this.model.allocations.forEach(function(allocation, index){
            total += allocation.months[monthIndex].percentage;
        })

        this.totals[monthIndex] = total;
    }

    add(analytic){
        var analyticExist = this.model.allocations.find(x => x.analyticId == analytic.id);

        if(analyticExist) return;

        var months = [];

        this.model.monthsHeader.forEach((item, index) => {
            months.push({ 
                allocationId: 0,
                date: new Date(item.year, item.month-1, 1),
                percentage: 0
            })
        });

        var row = new Allocation();
        row.analyticId = analytic.id;
        row.analyticTitle = analytic.title;
        row.employeeId = this.resourceId;
        row.months = months;

        this.model.allocations.push(row);
    }

    confirm(allocation){
        this.allocationSelected = allocation;
        this.confirmModal.show();
    }

    save(){
        this.allocationSelected.releaseDate = this.releaseDate;

        this.addSubscrip = this.allocationsService.add(this.allocationSelected).subscribe(data => {
            this.confirmModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);
        },
        error => {
            this.confirmModal.hide();
            this.errorHandlerService.handleErrors(error);
        });
    }
}