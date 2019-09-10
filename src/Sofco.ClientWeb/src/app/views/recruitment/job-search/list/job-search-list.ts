import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { JobSearchService } from "app/services/recruitment/jobsearch.service";
import { DataTableService } from "app/services/common/datatable.service";
import { CustomerService } from "app/services/billing/customer.service";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { Subscription } from "rxjs";
import { JobSearchStatus } from "app/models/enums/jobSearchStatus";

declare var moment: any;

@Component({
    selector: 'job-search-list',
    templateUrl: './job-search-list.html'
})
export class JobSearchListComponent implements OnInit, OnDestroy {

    searchSubscrip: Subscription;
    getUsersSubscrip: Subscription;
    getReasonsSubscrip: Subscription;
    getClientsSubscrip: Subscription;
    getProfilesSubscrip: Subscription;
    getSkilsSubscrip: Subscription;
    getRecruiterSubscrip: Subscription;

    profileOptions: any[] = new Array();
    skillOptions: any[] = new Array();
    reasonOptions: any[] = new Array();
    customerOptions: any[] = new Array();
    applicantOptions: any[] = new Array();
    recruitersOptions: any[] = new Array();
    statusOptions: any[] = new Array();

    data: any[] = new Array();

    id: number;
    userId: number;
    reasonCauseId: number;
    recruiterId: number;
    statusId: number;
    clientId: string;
    skills: any;
    profiles: any;

    constructor(private router: Router,
        private messageService: MessageService,
        private jobSearchService: JobSearchService,
        private dataTableService: DataTableService,
        private customerService: CustomerService,
        private genericOptionsService: GenericOptionService,
        private menuService: MenuService){
    }

    ngOnInit(): void {
        this.messageService.showLoading();

        var promises = new Array();

        var promise1 = new Promise((resolve, reject) => this.getProfiles(resolve));
        var promise3 = new Promise((resolve, reject) => this.getSkills(resolve));
        var promise4 = new Promise((resolve, reject) => this.getReasons(resolve));
        var promise5 = new Promise((resolve, reject) => this.getCustomers(resolve));
        var promise6 = new Promise((resolve, reject) => this.getRecruiters(resolve));
        var promise7 = new Promise((resolve, reject) => this.getApplicants(resolve));

        promises.push(promise1);
        promises.push(promise3);
        promises.push(promise4);
        promises.push(promise5);
        promises.push(promise6);
        promises.push(promise7);

        Promise.all(promises).then(data => { 
            this.messageService.closeLoading();
        });

        this.statusOptions.push({ id: JobSearchStatus.Open, text: "Abierta"});
        this.statusOptions.push({ id: JobSearchStatus.Reopen, text: "Re-Abierta"});
        this.statusOptions.push({ id: JobSearchStatus.Suspended, text: "Suspendida"});
        this.statusOptions.push({ id: JobSearchStatus.Close, text: "Cerrada"});
    }    
    
    ngOnDestroy(): void {
        if (this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if (this.getUsersSubscrip) this.getUsersSubscrip.unsubscribe();
        if (this.getReasonsSubscrip) this.getReasonsSubscrip.unsubscribe();
        if (this.getClientsSubscrip) this.getClientsSubscrip.unsubscribe();
        if (this.getProfilesSubscrip) this.getProfilesSubscrip.unsubscribe();
        if (this.getSkilsSubscrip) this.getSkilsSubscrip.unsubscribe();
        if (this.getRecruiterSubscrip) this.getRecruiterSubscrip.unsubscribe();
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

    getApplicants(resolve){
        this.getUsersSubscrip = this.jobSearchService.getApplicants().subscribe(response => {
            resolve();
            this.applicantOptions = response.data;
        },
        () => resolve());
    }

    getRecruiters(resolve){
        this.getRecruiterSubscrip = this.jobSearchService.getRecruiters().subscribe(response => {
            resolve();
            this.recruitersOptions = response.data;
        },
        () => resolve());
    }

    getProfiles(resolve){
        this.genericOptionsService.controller = "profile";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.profileOptions = response.data;
        },
        () => resolve());
    }

    getSkills(resolve){
        this.genericOptionsService.controller = "skill";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.skillOptions = response.data;
        },
        () => resolve());
    }

    getReasons(resolve){
        this.genericOptionsService.controller = "reasonCause";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.reasonOptions = response.data;
        },
        () => resolve());
    }

    getCustomers(resolve) {
        this.getClientsSubscrip = this.customerService.getOptions().subscribe(d => {
            resolve();
            this.customerOptions = d.data;
        },
        () => resolve());
    }

    clean(){
        this.id = null;
        this.clientId = null;
        this.skills = null;
        this.profiles = null;
        this.userId = null;
        this.statusId = null;
        this.reasonCauseId = null;
        this.recruiterId = null;
    }

    search(){
        var json = {
            id: this.id,
            clientId: this.clientId,
            skills: this.skills,
            profiles: this.profiles,
            userId: this.userId,
            status: this.statusId,
            reasonId: this.reasonCauseId,
            recruiterId: this.recruiterId
        };

        this.data = [];

        this.messageService.showLoading();

        this.searchSubscrip = this.jobSearchService.search(json).subscribe(response => {
            this.messageService.closeLoading();

            if(response && response.data && response.data.length > 0){
                this.data = response.data;
                this.initGrid();
            }
            else{
                this.messageService.showWarning("searchEmpty");
            }
        }, 
        error => this.messageService.closeLoading())
    }

    initGrid() {
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
        var title = `Busquedas-${moment(new Date()).format("YYYYMMDD")}`;

        var options = {
            selector: "#jobSearchTable",
            columns: columns,
            title: title,
            columnDefs: [{ 'aTargets': [1, 12, 13, 14], "sType": "date-uk" }],
            withExport: true,
        };

        setTimeout(() => {
            $("#jobSearchTable_wrapper").css("float","left");
            $("#jobSearchTable_wrapper").css("padding-bottom","50px");
            $("#jobSearchTable_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#jobSearchTable_paginate").addClass('table-pagination');
            $("#jobSearchTable_length").css("margin-right","10px");
            $("#jobSearchTable_info").css("padding-top","4px");
        }, 500);

        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }

    getStatusDesc(id){
        switch(id){
            case JobSearchStatus.Open: return "Abierta";
            case JobSearchStatus.Reopen: return "Re-Abierta";
            case JobSearchStatus.Suspended: return "Suspendida";
            case JobSearchStatus.Close: return "Cerrada";
        }
    }

    canCreate(){
        return this.menuService.hasFunctionality('RECRU', 'ADD-JOBSEARCH');
    }

    goToNew(){
        this.router.navigate(['recruitment/jobSearch/add']);
    }
}