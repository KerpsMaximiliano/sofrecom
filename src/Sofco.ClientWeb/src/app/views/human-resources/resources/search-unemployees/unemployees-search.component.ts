import { Component, ViewChild, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { DataTableService } from "app/services/common/datatable.service";
import { ErrorHandlerService } from "../../../../services/common/errorHandler.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

declare var $: any;

@Component({
    selector: 'unemployees-search',
    templateUrl: './unemployees-search.component.html',
    styleUrls: ['./unemployees-search.component.scss']
})
export class UnemployeesSearchComponent implements OnInit, OnDestroy {

    @ViewChild('accordion') accordion;

    public resources: any[] = new Array<any>();
    public comments: string;
    public gridIsVisible: boolean = false;

    searchSubscrip: Subscription;

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
        startDate: null,
        endDate: null
    };

    constructor(private router: Router,
        private messageService: MessageService,
        private employeeService: EmployeeService,
        private dataTableService: DataTableService,
        private errorHandlerService: ErrorHandlerService){}

    ngOnInit(): void {
        var data = JSON.parse(sessionStorage.getItem('lastUnemployeeQuery'));

        if(data){
            this.searchModel = data;
            this.search();
        }
    }

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
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
            this.errorHandlerService.handleErrors(error)
        });
    }

    initGrid(){
        var options = { selector: "#resourcesTable", columnDefs: [ {'aTargets': [1, 2], "sType": "date-uk"} ] };
        this.dataTableService.destroy(options.selector); 
        this.dataTableService.init2(options);
        this.gridIsVisible = true;
    }

    clean(){
        this.searchModel.name = "";
        this.searchModel.startDate = "";
        this.searchModel.endDate = "";
        $('.datepicker').val('');
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
            $("#search-icon").toggleClass('fa-minus').toggleClass('fa-plus');
        }
        else{
            $("#search-icon").toggleClass('fa-plus').toggleClass('fa-minus');
        } 
    }
}