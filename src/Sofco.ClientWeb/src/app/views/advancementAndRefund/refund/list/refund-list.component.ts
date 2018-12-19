import { Component, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { I18nService } from "app/services/common/i18n.service";
import { UserService } from "app/services/admin/user.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";

declare var $: any;

@Component({
    selector: 'refund-list',
    templateUrl: './refund-list.component.html',
    styleUrls: ['./refund-list.component.scss']
})
export class RefundListComponent implements OnInit {

    public resources: any[] = new Array<any>();
    public states: any[] = new Array<any>();

    public resourceId: number;
    public typeId: number;
    public dateSince: Date;
    public dateTo: Date;

    public tabInProcess = true;

    getResourcesSubscrip: Subscription;
    subscrip: Subscription;

    constructor(private userService: UserService,
        private refundService: RefundService){}

    ngOnInit(): void {
        this.getResources();
        this.getStates();
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

    collapse(){
        if($("#collapseOne").hasClass('in')){
            $("#collapseOne").removeClass('in');
        }
        else{
            $("#collapseOne").addClass('in');
        }

        this.changeIcon();
    }

    changeIcon(){
        if($("#collapseOne").hasClass('in')){
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else{
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }
}