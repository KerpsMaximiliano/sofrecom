import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { DataTableService } from 'app/services/common/datatable.service';
import { GenericOptionService } from 'app/services/admin/generic-option.service';
import { MessageService } from 'app/services/common/message.service';
import { GenericOptions } from 'app/models/enums/genericOptions';
import { CustomerService } from 'app/services/billing/customer.service';
import { JobSearchStatus } from 'app/models/enums/jobSearchStatus';
import { JobSearchService } from 'app/services/recruitment/jobsearch.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';

@Component({
  selector: 'app-recruitment-report',
  templateUrl: './recruitment-report.html',
  styleUrls: ['./recruitment-report.scss']
})
export class RecruitmentReportComponent implements OnInit, OnDestroy {

    clientCrmId: number;
    skillId: number;
    statusId: number;

    filterByDates: boolean;
    startDate: Date;
    endDate: Date;

    skillOptions: any[] = new Array();
    customerOptions: any[] = new Array();
    statusOptions: any[] = new Array(); 

    list: any[] = new Array();
    contacts: any[] = new Array();
    selectors: any[] = new Array();
    salaryAverage: any[] = new Array();
    jobSearchsBySelectors: any[] = new Array();

    comments: string;
    rrhhComments: string;
    technicalComments: string;
    clientCommets: string;
    selectorName: string;

    searchSubscrip: Subscription;
    getClientsSubscrip: Subscription;
    getSkillsSubscrip: Subscription;

    data: any;
    applicantsInterviewedData: any;
    applicantsReportPieData: any;
    options: any;

    @ViewChild('commentsModal') commentsModal;

    public commentsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "comments",
        "commentsModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close"
    );

    constructor(private dataTableService: DataTableService,
        private genericOptionsService: GenericOptionService,
        private customerService: CustomerService,
        private jobSearchService: JobSearchService,
        private messageService: MessageService) { }

    ngOnInit() {
        this.getCustomers();
        this.getSkills();

        this.statusOptions.push({ id: JobSearchStatus.Open, text: "Abierta"});
        this.statusOptions.push({ id: JobSearchStatus.Reopen, text: "Re-Abierta"});
        this.statusOptions.push({ id: JobSearchStatus.Suspended, text: "Suspendida"});
        this.statusOptions.push({ id: JobSearchStatus.Close, text: "Cerrada"});
    }

    ngOnDestroy(): void {
        if (this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if (this.getClientsSubscrip) this.getClientsSubscrip.unsubscribe();
        if (this.getSkillsSubscrip) this.getSkillsSubscrip.unsubscribe();
    }

    getCustomers() {
        this.getClientsSubscrip = this.customerService.getAllOptions().subscribe(d => {
            this.customerOptions = d.data;
        });
    }

    getSkills(){
        this.genericOptionsService.controller = GenericOptions.Skill;
        this.getSkillsSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            this.skillOptions = response.data;
        });
    }

    search() {
        if(!this.filterByDates){
            this.startDate = null;
            this.endDate = null;
        }
        else{
            if(this.startDate != null && this.endDate != null && this.endDate < this.startDate){
                this.messageService.showError("dateToLessThanSince");
                return;
            }
        }

        var json = {
            skills: this.skillId,
            clientId: this.clientCrmId,
            status: this.statusId,
            startDate: this.startDate,
            endDate: this.endDate
        };

        this.list = [];

        this.messageService.showLoading();

        this.searchSubscrip = this.jobSearchService.searchReport(json).subscribe(response => {
            this.messageService.closeLoading();

            if(response && response.data && response.data.length > 0){
                this.list = response.data.map(x => {
                    x.selected = false;
                    return x;
                });

                this.collapse();
                this.initGrid();
            }
            else{
                this.messageService.showWarning("searchEmpty");
            }
        }, 
        error => this.messageService.closeLoading())
    }

    clean(){
        this.skillId = null;
        this.clientCrmId = null;
        this.statusId = null;
        this.startDate = null;
        this.endDate = null;
        this.filterByDates = false;
    }

    initGrid() {
        var columns = [0, 1, 2, 3 ,4, 5, 6, 7, 8, 9, 10, 11];

        var params = {
            selector: '#searchTable',
            columns: columns,
            columnDefs: [ { "aTargets": [2], "sType": "date-uk" }],
            title: 'Reporte reclutamiento',
            withExport: true,
        }

        setTimeout(() => {
            $("#searchTable_wrapper").css("float","left");
            $("#searchTable_wrapper").css("padding-bottom","50px");
            $("#searchTable_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#searchTable_paginate").addClass('table-pagination');
            $("#searchTable_length").css("margin-right","10px");
            $("#searchTable_info").css("padding-top","4px");
        }, 500);

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }

    initContactGrid() {
        var columns = [0, 1, 2, 3 ,4, 5, 6, 7];

        var params = {
            selector: '#contactTable',
            columns: columns,
            columnDefs: [ { "aTargets": [1], "sType": "date-uk" }],
            title: 'Reporte reclutamiento - contactos',
            withExport: true,
        }

        setTimeout(() => {
            $("#contactTable_wrapper").css("float","left");
            $("#contactTable_wrapper").css("padding-bottom","50px");
            $("#contactTable_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#contactTable_paginate").addClass('table-pagination');
            $("#contactTable_length").css("margin-right","10px");
            $("#contactTable_info").css("padding-top","4px");
        }, 500);

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
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

    getStatusDesc(id){
        switch(id){
            case JobSearchStatus.Open: return "Abierta";
            case JobSearchStatus.Reopen: return "Re-Abierta";
            case JobSearchStatus.Suspended: return "Suspendida";
            case JobSearchStatus.Close: return "Cerrada";
        }
    }

    anyJobsearchSelected(){
        return this.list.filter(x => x.selected).length > 0;
    }

    filterContacts(){
        this.contacts = [];

        var jobsearchSelected = this.list.filter(x => x.selected);

        if(jobsearchSelected && jobsearchSelected.length > 0){

            jobsearchSelected.forEach(jobSearch => {
                
                if(jobSearch.contacts && jobSearch.contacts.length > 0){

                    jobSearch.contacts.forEach(contact => {
                        contact.jobsearchId = jobSearch.code;
                        this.contacts.push(contact);
                    });
                }
            });

            this.initContactGrid();

            this.buildFirstReport();
            this.buildSecondReport();
            this.buildThirdReport();
        }
    } 

    chartClick(event){
        if(event.element && event.element.length > 0){
            var index = event.element[0]._index;
            let selector = this.selectors[index];

            this.jobSearchsBySelectors = [];
            this.selectorName = null;

            if(selector && selector.jobSearchs && selector.jobSearchs.length > 0){
                this.jobSearchsBySelectors = selector.jobSearchs;
                this.selectorName = selector.name;
            }
        }
    }

    buildThirdReport(){
        let interviewedCount = 0;
        let notInterviewedCount = 0;
        let interviewedPercentage = 0;
        let notInterviewedPercentage = 0;
        this.applicantsReportPieData = [];

        this.contacts.forEach(contact => {
            if(contact.hasRrhhInterview || contact.hasTechnicalInterview || contact.hasClientInterview){
                interviewedCount += 1;
            }
            else{
                notInterviewedCount += 1;
            }
        });

        let total = interviewedCount + notInterviewedCount;
        interviewedPercentage =  (interviewedCount / total) * 100;
        notInterviewedPercentage =  (notInterviewedCount / total) * 100;

        this.applicantsReportPieData.push({ name: "Entrevistados", count: interviewedCount, percentage: interviewedPercentage });
        this.applicantsReportPieData.push({ name: "Sin Entrevistar", count: notInterviewedCount, percentage: notInterviewedPercentage });

        this.applicantsInterviewedData = {
            labels: ["Entrevistados", "Sin Entrevistar"],
            datasets: [
              {
                data: [interviewedCount, notInterviewedCount],
                backgroundColor: this.getBackgroudColors(2),
                fill: false,
                borderWidth: 1,
              },
            ],
        };
    }

    buildSecondReport(){
        this.selectors = [];

        this.contacts.forEach(contact => {

            if(!contact.recruiterId || contact.recruiterId <= 0) return;
            
            let selector = this.selectors.find(x => x.id == contact.recruiterId);

            if(selector != null){
                selector.count += 1;

                let jobsearch = selector.jobSearchs.find(x => x.id == contact.jobsearchId);

                if(jobsearch != null){
                    jobsearch.count += 1;
                }
                else{
                    selector.jobSearchs.push({
                        id: contact.jobsearchId,
                        count: 1
                    });
                }
            }
            else{
                let selectorToAdd = {
                    id: contact.recruiterId,
                    name: contact.recruiterText,
                    jobSearchs: [{
                        id: contact.jobsearchId,
                        count: 1
                    }],
                    count: 1
                };

                this.selectors.push(selectorToAdd);
            }
        });

        let labels = this.selectors.map(x => x.name);
        let values = this.selectors.map(x => x.count);

        this.data = {
            labels: labels,
            datasets: [
              {
                label: "Selectores",
                data: values,
                fill: false,
                backgroundColor: this.getBackgroudColors(this.selectors.length),
                borderWidth: 1,
              },
            ],
        };

        this.options = {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    }

    buildFirstReport(){
        this.salaryAverage = [];

        this.contacts.forEach(contact => {

            if(!contact.salary || contact.salary <= 0) return;

            var itemAverage = this.salaryAverage.find(x => x.jobsearchId == contact.jobsearchId);

            if(itemAverage){
                itemAverage.salaryValue += contact.salary;
                itemAverage.count += 1;

                itemAverage.salaryValueAvg = itemAverage.salaryValue / itemAverage.count;
                itemAverage.salaryPercentageAvg = ((itemAverage.salaryValueAvg - itemAverage.salary) / itemAverage.salaryValueAvg) * 100;
            }
            else{
                let itemToAdd = {
                    jobsearchId: contact.jobsearchId,
                    count: 1,
                    salaryValue: contact.salary,
                    salary: null,
                    salaryValueAvg: 0,
                    salaryPercentageAvg: 0
                }

                var jobSearch = this.list.find(x => x.code == contact.jobsearchId);

                if(jobSearch){
                    itemToAdd.salary = jobSearch.maximumSalary;
                }

                itemToAdd.salaryValueAvg = itemToAdd.salaryValue / itemToAdd.count;
                itemToAdd.salaryPercentageAvg = ((itemToAdd.salaryValueAvg - itemToAdd.salary) / itemToAdd.salaryValueAvg) * 100;

                this.salaryAverage.push(itemToAdd);
            }
        });
    }

    showComments(item){
        this.comments = item.comments;
        this.rrhhComments = item.rrhhInterviewComments;
        this.technicalComments = item.technicalInterviewComments;
        this.clientCommets = item.clientInterviewComments;

        this.commentsModal.show();
    }

    getBackgroudColors(count){
        var graphColors = [];

        var  i = 0;
        while (i <= count) {
            var randomR = Math.floor((Math.random() * 130) + 100);
            var randomG = Math.floor((Math.random() * 130) + 100);
            var randomB = Math.floor((Math.random() * 130) + 100);
        
            var graphBackground = "rgb(" 
                    + randomR + ", " 
                    + randomG + ", " 
                    + randomB + ")";
            graphColors.push(graphBackground);
            
            i++;
        }

        return graphColors;
    }
}
