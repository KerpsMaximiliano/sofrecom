import { Component, ViewChild, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "../../../../services/common/message.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { Router } from "@angular/router";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { UserService } from "app/services/admin/user.service";

declare var $: any;

@Component({
    selector: 'unemployees-search',
    templateUrl: './unemployees-search.component.html',
    styleUrls: ['./unemployees-search.component.scss']
})
export class UnemployeesSearchComponent implements OnInit, OnDestroy {

    @ViewChild('accordion') accordion;

    public resources: any[] = new Array<any>();
    public users: any[] = new Array<any>();
    public analytics: any[] = new Array<any>();

    public comments: string;
    public gridIsVisible: boolean = false;

    searchSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getUsersSubscrip: Subscription;

    @ViewChild('commentsModal') commentsModal;

    public commentsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "comments",
        "commentsModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close"
    );

    public searchModel = {
        name: "",
        seniority: "",
        profile: "",
        technology: "",
        analyticId: null,
        superiorId: null,
        managerId: null,
        employeeNumber: "",
        startDate: null,
        endDate: null
    };

    constructor(
        private messageService: MessageService,
        private employeeService: EmployeeService,
        private router: Router,
        private analyticService: AnalyticService,
        private usersService: UserService,
        private dataTableService: DataTableService){}

    ngOnInit(): void {
        this.getAnalytics();
        this.getUsers();

        var data = JSON.parse(sessionStorage.getItem('lastUnemployeeQuery'));

        if(data){
            this.searchModel = data;
            this.search();
        }
    }

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getUsersSubscrip) this.getUsersSubscrip.unsubscribe();
    }

    getAnalytics() {
        this.getAnalyticSubscrip = this.analyticService.getOptions().subscribe(
            data => {
                this.analytics = data;
            });
    }

    getUsers(){
        this.getUsersSubscrip = this.usersService.getOptions().subscribe(data => {
            this.users = data;
        });
    }

    search(){
        this.messageService.showLoading();

        this.searchSubscrip = this.employeeService.searchUnemployees(this.searchModel).subscribe(response => {
            this.resources = response;

            this.initGrid();
            this.messageService.closeLoading();
            this.collapse();

            sessionStorage.setItem('lastUnemployeeQuery', JSON.stringify(this.searchModel));
        },
        error => {
            this.messageService.closeLoading();
        });
    }

    initGrid(){
        var columns = [0, 1, 2, 3];
        var title = `Recursos inactivos`;

        var options = { 
            selector: "#resourcesTable", 
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [ {'aTargets': [1, 2], "sType": "date-uk"} ] 
        };

        this.dataTableService.destroy(options.selector); 
        this.dataTableService.initialize(options);
        this.gridIsVisible = true;
    }

    clean(){
        this.searchModel.name = "";
        this.searchModel.startDate = "";
        this.searchModel.endDate = "";
        $('.datepicker').val('');
      
        this.searchModel.profile = "";
        this.searchModel.seniority = "";
        this.searchModel.technology = "";
        this.searchModel.employeeNumber = "";
        this.searchModel.analyticId = null;
        this.searchModel.superiorId = null;
        this.searchModel.managerId = null;

        this.resources = [];
        sessionStorage.removeItem('lastUnemployeeQuery');

        this.initGrid();
    }

    showComments(resource){
        this.comments = resource.endReasonComments;
        this.commentsModal.show();
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

    goToProfile(resource){
        this.router.navigate([`/allocationManagement/resources/${resource.id}`]);
    }
}