import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { Subscription } from "rxjs";
import { UserService } from "app/services/admin/user.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { UtilsService } from "app/services/common/utils.service";

@Component({
    selector: 'refund-list-filter',
    templateUrl: './refund-list-filter.component.html'
})
export class RefundListFilterComponent implements OnInit {
    @Input()
    public controlId = 'grid1';
    @Input()
    public model: any;
    @Input()
    public inWorkflowProcess = true;
    private collapseSelector = '#grid1';
    public resources: any[] = new Array<any>();
    public resourceId: number;
    @Output() valueChange = new EventEmitter<any>();
    public dateSince: Date;
    public dateTo: Date;
    public states: any[] = new Array<any>();
    public banks: any[] = new Array<any>();
    public stateId: number;
    public bankId: number;
    getResourcesSubscrip: Subscription;
    subscrip: Subscription;

    constructor(private userService: UserService,
        private utilsService: UtilsService,
        private refundService: RefundService) { }

    ngOnInit(): void {
        this.collapseSelector = "#collapse" + this.controlId;

        this.getResources();
        //  if(this.inWorkflowProcess){
        this.getStates();
        //}
        this.getBanks();
    }

    collapse() {
        if ($(this.collapseSelector).hasClass('in')) {
            $(this.collapseSelector).removeClass('in');
        }
        else {
            $(this.collapseSelector).addClass('in');
        }

        this.changeIcon();
    }

    changeIcon() {
        if ($(this.collapseSelector).hasClass('in')) {
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else {
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }

    getResources() {
        this.getResourcesSubscrip = this.userService.getOptions().subscribe(data => {
            this.resources = data;
        });
    }

    getStates() {
        this.subscrip = this.refundService.getStates().subscribe(res => {
            this.states = res.data
        });
    }

    getBanks() {
        this.getResourcesSubscrip = this.utilsService.getBanks().subscribe(response => {
            this.banks = response;
        });
    }

    clean() {
        this.resourceId = null;
        this.stateId = null;
        this.dateSince = null;
        this.dateTo = null;
        this.inWorkflowProcess = this.inWorkflowProcess;
        this.bankId = null;
        this.search();
    }

    search() {
        this.model = {
            userApplicantId: this.resourceId,
            stateId: this.stateId,
            dateSince: this.dateSince,
            dateTo: this.dateTo,
            inWorkflowProcess: this.inWorkflowProcess,
            bank: this.bankId
        }
        this.valueChange.emit(this.model);

        //   this.collapse();
    }
}
