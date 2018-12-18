import { Component, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { I18nService } from "app/services/common/i18n.service";
import { UserService } from "app/services/admin/user.service";

declare var $: any;

@Component({
    selector: 'refund-list',
    templateUrl: './refund-list.component.html',
    styleUrls: ['./refund-list.component.scss']
})
export class RefundListComponent implements OnInit {

    public resources: any[] = new Array<any>();
    public types: any[] = new Array<any>();

    public resourceId: number;
    public typeId: number;
    public dateSince: Date;
    public dateTo: Date;

    public tabInProcess = true;

    getResourcesSubscrip: Subscription;

    constructor(private userService: UserService, private i18nService: I18nService){}

    ngOnInit(): void {
        this.getResources();
        this.getTypes();
    }

    getResources(){
        this.getResourcesSubscrip = this.userService.getOptions().subscribe(data => {
            this.resources = data;
        });
    }

    getTypes(){
        this.types.push({ id: 1, text: this.i18nService.translateByKey('advancement.salary') });
        this.types.push({ id: 2, text: this.i18nService.translateByKey('advancement.viaticum') });
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