import { Component, OnInit, ViewChild, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { I18nService } from "app/services/common/i18n.service";
import { UserService } from "app/services/admin/user.service";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { UtilsService } from "app/services/common/utils.service";
import { MenuService } from "app/services/admin/menu.service";
import { environment } from "environments/environment";

declare var $: any;

@Component({
    selector: 'advancement-search',
    templateUrl: './advancement-search.component.html',
    styleUrls: ['./advancement-search.component.scss']
})
export class AdvancementSearchComponent implements OnInit, OnDestroy {
    public resources: any[] = new Array<any>();
    public types: any[] = new Array<any>();
    public states: any[] = new Array<any>();
    public banks: any[] = new Array<any>();

    public resourceId: number;
    public typeId: number;
    public stateIds: number[] = [];
    public dateSince: Date;
    public dateTo: Date;
    public bankId: number;

    @ViewChild('finalized') finalized;

    getResourcesSubscrip: Subscription;
    getStatesSubscrip: Subscription;

    constructor(private userService: UserService,
        private advancementService: AdvancementService,
        private menuService: MenuService,
        private utilsService: UtilsService,
        private i18nService: I18nService) { }

    ngOnInit(): void {
        this.getResources();
        this.getTypes();
        this.getStates();
        this.getBanks();

        const data = JSON.parse(sessionStorage.getItem('lastAdvancementQuery'));

        if(data){
            this.resourceId = data.resourceId;
            this.typeId = data.typeId;
            this.stateIds = data.stateIds;
            this.dateSince = data.dateSince;
            this.dateTo = data.dateTo;
            this.bankId = data.bankId;

            this.search();
        }
        else {
            if(this.menuService.userIsDaf) this.stateIds.push(environment.dafWorkflowStateId);
            if(this.menuService.userIsGaf) this.stateIds.push(environment.gafWorkflowStateId);
            if(this.menuService.userIsRrhh) this.stateIds.push(environment.rrhhWorkflowStateId);
            if(this.menuService.userIsDirector) this.stateIds.push(environment.pendingDirectorWorkflowStateId);
            if(this.menuService.userIsManager) this.stateIds.push(environment.pendingManagerWorkflowStateId);

            if(this.stateIds.length > 0){
                this.search();
            }
        }
    }

    ngOnDestroy(): void {
        if (this.getResourcesSubscrip) this.getResourcesSubscrip.unsubscribe();
        if (this.getStatesSubscrip) this.getStatesSubscrip.unsubscribe();
    }

    getResources() {
        this.getResourcesSubscrip = this.userService.getOptions().subscribe(data => {
            this.resources = data;
        });
    }

    getStates() {
        this.getResourcesSubscrip = this.advancementService.getStates().subscribe(response => {
            this.states = response.data;
        });
    }

    getTypes() {
        this.types.push({ id: 1, text: this.i18nService.translateByKey('advancement.salary') });
        this.types.push({ id: 2, text: this.i18nService.translateByKey('advancement.viaticum') });
    }

    getBanks() {
        this.getResourcesSubscrip = this.utilsService.getBanks().subscribe(response => {
            this.banks = response;
        });
    }

    clean() {
        this.resourceId = null;
        this.typeId = null;
        this.stateIds = [];
        this.dateSince = null;
        this.dateTo = null;
        this.bankId = null;
    }

    search() {
        var model = {
            resourceId: this.resourceId,
            typeId: this.typeId,
            stateIds: this.stateIds,
            dateSince: this.dateSince,
            dateTo: this.dateTo,
            bank: this.bankId
        }

        this.finalized.search(model);
    }

    collapse() {
        if ($("#collapseOne").hasClass('in')) {
            $("#collapseOne").removeClass('in');
        }
        else {
            $("#collapseOne").addClass('in');
        }

        this.changeIcon();
    }

    changeIcon() {
        if ($("#collapseOne").hasClass('in')) {
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else {
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }
}