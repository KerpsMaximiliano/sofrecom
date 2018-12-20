import { Component, OnInit, Input } from "@angular/core";
import { Subscription } from "rxjs";
import { UserService } from "app/services/admin/user.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";

@Component({
    selector: 'refund-list-filter',
    templateUrl: './refund-list-filter.component.html'
})
export class RefundListFilterComponent implements OnInit {
    @Input()
    public controlId = 'grid1';
    private collapseSelector = '#grid1';

    public resources: any[] = new Array<any>();
    public resourceId: number;
    public dateSince: Date;
    public dateTo: Date;
    public states: any[] = new Array<any>();
    public typeId: number;
    getResourcesSubscrip: Subscription;
    subscrip: Subscription;

    constructor(private userService: UserService,
        private refundService: RefundService){}

    ngOnInit(): void {
        this.collapseSelector = "#collapse" + this.controlId;
        console.log('--->--> ' + this.collapseSelector);

        this.getResources();
        this.getStates();
    }

    collapse(){
        if($(this.collapseSelector).hasClass('in')){
            $(this.collapseSelector).removeClass('in');
        }
        else{
            $(this.collapseSelector).addClass('in');
        }

        this.changeIcon();
    }

    changeIcon(){
        if($(this.collapseSelector).hasClass('in')){
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else{
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }

    getResources(){
        this.getResourcesSubscrip = this.userService.getOptions().subscribe(data => {
            this.resources = data;

        });
    }

    getStates(){
        this.subscrip = this.refundService.getStates().subscribe(res => {
            this.states = res.data
        });
    }

    clean(){
        this.resourceId = null;
        this.typeId = null;
        this.dateSince = null;
        this.dateTo = null;
    }

    search(){
        const model = {
            resourceId: this.resourceId,
            typeId: this.typeId,
            dateSince: this.dateSince,
            dateTo: this.dateTo
        }
    }
}
