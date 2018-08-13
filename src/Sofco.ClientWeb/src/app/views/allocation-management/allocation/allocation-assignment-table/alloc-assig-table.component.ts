import { OnDestroy, Component, OnInit, Input, ViewChild, Output, EventEmitter } from "@angular/core";
import { AllocationModel, Allocation } from "../../../../models/allocation-management/allocation";
import { Subscription } from "rxjs";
import { MessageService } from "../../../../services/common/message.service";
import { AllocationService } from "../../../../services/allocation-management/allocation.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";

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
    totalsAux: any[] = new Array<any>();
    model: AllocationModel;

    @Input() resourceId: number;
    @Input() analytic: any;
    @Input() monthQuantity: number;

    @Output() reloadTimeline: EventEmitter<any> = new EventEmitter();

    getAllAllocationsSubscrip: Subscription;
    addSubscrip: Subscription;

    releaseDate: Date;

    public allocationSelected: any;
    public isEditingAnyRow: boolean = false;
    public rowEditing: any[] = new Array<any>();

    dateSince: Date = new Date();
    monthLastAllocation: number;

    constructor(private allocationsService: AllocationService,
        private messageService: MessageService){}

    ngOnInit(): void {
        if(this.resourceId > 0){
            this.getAllocations(this.resourceId, this.dateSince, false);
        }
    }

    ngOnDestroy(): void {
        if(this.getAllAllocationsSubscrip) this.getAllAllocationsSubscrip.unsubscribe();
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    getAllocations(resourceId, dateSince, cleanModel){

        if(cleanModel && this.model){
            this.model.allocations = [];
        }

        this.dateSince = dateSince;

        if(!this.monthQuantity || this.monthQuantity < 1 || this.monthQuantity > 36){
            this.messageService.showError("allocationManagement.allocation.wrongMonthQuantity");
            return;
        }

        dateSince.setDate(1);
        this.totals = [];
        this.isEditingAnyRow = false;

        var todayPlus12Months = new Date(dateSince.getFullYear(), dateSince.getMonth()+this.monthQuantity-1, 1);

        this.messageService.showLoading();

        this.getAllAllocationsSubscrip = this.allocationsService.getAllocations(resourceId, dateSince.toUTCString(), todayPlus12Months.toUTCString()).subscribe(data => {
            let allocationAux = new Array();
            
            if(this.model && this.model.allocations){
                allocationAux = this.model.allocations;
            }

            this.model = data;

            if(this.model.allocations.length == 0){
                this.messageService.showWarning("allocationManagement.allocation.emptyMessage");
            }

            allocationAux.forEach((item, index) => {

                var analyticExist = this.model.allocations.find(x => x.analyticId == item.analyticId);

                if(!analyticExist){
                    
                    if(item.months.length > this.model.monthsHeader.length){
                        item.months = item.months.splice(0, this.model.monthsHeader.length);
                    }
                    else if(item.months.length < this.model.monthsHeader.length){
                        for(var i = item.months.length; i < this.model.monthsHeader.length; i++){
                            item.months.push({ 
                                allocationId: 0,
                                date: new Date(this.model.monthsHeader[i].year, this.model.monthsHeader[i].month-1, 1),
                                percentage: 0
                            })
                        }
                    }

                    this.model.allocations.push(item);
                }
            });

            if(this.analytic){
                this.add(this.analytic);
            }
            
            this.model.monthsHeader.forEach((item, index) => {
                this.totals.push(0);

                this.updateTotal(0, index);
            });

            this.messageService.closeLoading();
        },
        error => {
            this.messageService.closeLoading();
        });
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
        row.edit = false;

        this.model.allocations.push(row);
    }

    confirm(allocation){
        this.allocationSelected = allocation;

        let date;

        allocation.months.forEach(element => {
            if(element.percentage > 0){

                if(typeof element.date == 'string'){
                    element.date = new Date(element.date);
                }

                date = new Date(element.date.getFullYear(), element.date.getMonth()+1, 0);
                this.monthLastAllocation = element.date.getMonth();
            }
        });

        this.releaseDate = date;
        
        this.confirmModal.show();
    }

    save(){
        if(this.releaseDate && this.releaseDate.getMonth() != this.monthLastAllocation){
            this.messageService.showErrorByFolder("allocationManagement/allocation", "monthDifferent");
            return;
        }   

        this.allocationSelected.releaseDate = this.releaseDate;

        this.addSubscrip = this.allocationsService.add(this.allocationSelected).subscribe(data => {
            this.confirmModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);

            this.isEditingAnyRow = false;
            this.allocationSelected.edit = false;

            this.getAllocations(this.resourceId, this.dateSince, false);

            this.rowEditing = [];
            this.totalsAux = [];

            if(this.reloadTimeline.observers.length > 0){
                this.reloadTimeline.emit();
            }
        },
        error => {
            this.allocationSelected.releaseDate = null;
            this.confirmModal.hide();
        });
    }

    edit(allocation){
        this.isEditingAnyRow = true;
        allocation.edit = true;
        
        this.totals.forEach((item, index) => {
            this.totalsAux.push(item);
        }, this);

        allocation.months.forEach((item, index) => {
           this.rowEditing.push(item.percentage);
        });
    }

    cancel(allocation){
        this.isEditingAnyRow = false;
        allocation.edit = false;
        this.totals = [];

        this.rowEditing.forEach((item, index) => {
            allocation.months[index].percentage = item;
        });

        this.totalsAux.forEach((item, index) => {
            this.totals.push(item);
        }, this);

        this.rowEditing = [];
        this.totalsAux = [];
    }
}