import { Component, OnInit, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { I18nService } from "app/services/common/i18n.service";
import { UserService } from "app/services/admin/user.service";

declare var $: any;

@Component({
    selector: 'advancement-search',
    templateUrl: './advancement-search.component.html',
    styleUrls: ['./advancement-search.component.scss']
})
export class AdvancementSearchComponent implements OnInit {

    public resources: any[] = new Array<any>();
    public types: any[] = new Array<any>();

    public resourceId: number;
    public typeId: number;
    public dateSince: Date;
    public dateTo: Date;

    @ViewChild('inProcess') inProcess;
    @ViewChild('finalized') finalized;

    public currentTab: number = 1;

    getResourcesSubscrip: Subscription;

    constructor(private userService: UserService, 
        private i18nService: I18nService){}

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
        var model = {
            resourceId: this.resourceId,
            typeId: this.typeId,
            dateSince: this.dateSince,
            dateTo: this.dateTo
        }

        if(this.currentTab == 1){
            this.inProcess.search(model);
        }

        if(this.currentTab == 3){
            this.finalized.search(model);
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