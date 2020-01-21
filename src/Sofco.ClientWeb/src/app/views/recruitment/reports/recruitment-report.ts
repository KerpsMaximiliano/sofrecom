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
import { ReasonCauseType } from 'app/models/enums/reasonCauseType';

@Component({
  selector: 'app-recruitment-report',
  templateUrl: './recruitment-report.html',
  styleUrls: ['./recruitment-report.scss']
})
export class RecruitmentReportComponent implements OnInit, OnDestroy {

    clientCrmId: number;
    skillId: number;
    statusId: number;

    groupByRejectedVisible: boolean;
    groupByStatesVisible: boolean;
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
    contactsByState: any[] = new Array();

    comments: string;
    rrhhComments: string;
    technicalComments: string;
    clientCommets: string;
    selectorName: string;
    selectorId: number;

    searchSubscrip: Subscription;
    getClientsSubscrip: Subscription;
    getSkillsSubscrip: Subscription;

    data: any;
    applicantsInterviewedData: any;
    groupByRejected: any;
    groupByStates: any;
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
        this.contacts = [];

        this.messageService.showLoading();

        this.searchSubscrip = this.jobSearchService.searchReport(json).subscribe(response => {
            this.messageService.closeLoading();

            if(response && response.data && response.data.length > 0){
                this.list = response.data.map(x => {
                    x.selected = false;
                    return x;
                });

                this.collapse("collapseOne", "search-icon");
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

    collapse(id, icon) {
        if ($("#" + id).hasClass('in')) {
            $("#" + id).removeClass('in');
        }
        else {
            $("#" + id).addClass('in');
        }

        this.changeIcon(icon);
    }

    changeIcon(id) {
        if ($("#" + id).hasClass('in')) {
            $("#" + id).toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else {
            $("#" + id).toggleClass('fa-caret-up').toggleClass('fa-caret-down');
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

    selectAll(){
        this.list.forEach(x => x.selected = true);
    }

    unselectAll(){
        this.list.forEach(x => x.selected = false);
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

            if(this.contacts.length > 0){
                this.initContactGrid();

                this.buildFirstReport();
                this.buildSecondReport();
            }
            else{
                this.messageService.showWarningByFolder('common', 'searchEmpty');
            }
        }
    } 

    chartClick(event){
        this.jobSearchsBySelectors = [];
        this.selectorName = null;
        this.selectorId = null;

        if(event && event.element && event.element.length > 0){
            var index = event.element[0]._index;
            let selector = this.selectors[index];

            if(selector && selector.jobSearchs && selector.jobSearchs.length > 0){
                this.jobSearchsBySelectors = selector.jobSearchs;
                this.selectorName = selector.name;
                this.selectorId = selector.id;
            }
        }
        else{
            this.selectors.forEach(selector => {
                selector.jobSearchs.forEach(x => {

                    var exist = this.jobSearchsBySelectors.find(s => s.id == x.id);

                    if(exist == null){
                        this.jobSearchsBySelectors.push(Object.assign({}, x));
                    }
                    else{
                        exist.count += x.count;
                    }
                });
            });

            this.selectorName = "Selectores"
        }

        this.buildThirdReport();
    }

    buildFourthReport(interviewdData){
        if(interviewdData){
            if(interviewdData.groupByStates.inCompany == 0 && interviewdData.groupByStates.inProcess == 0 && interviewdData.groupByStates.rejected == 0){
                this.groupByStatesVisible = false;
            }
            else{
                this.groupByStatesVisible = true;

                this.groupByStates = {
                    labels: ["Ingresados", "En Proceso", "Rechazados"],
                    datasets: [
                      {
                        data: [interviewdData.groupByStates.inCompany, interviewdData.groupByStates.inProcess, interviewdData.groupByStates.rejected],
                        backgroundColor: this.getBackgroudColors(3),
                        fill: false,
                        borderWidth: 1,
                      },
                    ],
                };
            }
        }
        else{
            this.groupByStatesVisible = false;
        }
    }

    buildFifthReport(intervieweds, notIntervieweds){
        if(intervieweds.groupByStates.rejected == 0 && notIntervieweds.groupByStates.rejected == 0){
            this.groupByRejectedVisible = false;
        }
        else{
            this.groupByRejectedVisible = true;

            this.groupByRejected = {
                labels: ["Luego de Entrevista", "Antes de Entrevista"],
                datasets: [
                    {
                        data: [intervieweds.groupByStates.rejected, notIntervieweds.groupByStates.rejected],
                        backgroundColor: this.getBackgroudColors(2),
                        fill: false,
                        borderWidth: 1,
                    },
                ],
            };
        }
    }

    buildThirdReport(){
        let intervieweds = {
            count: 0,
            groupByStates: {
                inCompany: 0,
                inProcess: 0,
                rejected: 0
            }
        }

        let notIntervieweds = {
            count: 0,
            groupByStates: {
                inCompany: 0,
                inProcess: 0,
                rejected: 0
            }
        }

        this.contacts.forEach(contact => {
            if(this.selectorId && this.selectorId != contact.recruiterId) return;

            if(contact.reasonCauseType == ReasonCauseType.ApplicantOpen || contact.reasonCauseType == ReasonCauseType.ApplicantContacted) {
                
                let exist = this.contactsByState.find(x => x.id == contact.reasonId);

                if(exist == null){
                    var itemToAdd = {
                        id: contact.reasonId,
                        text: contact.reasonText,
                        count: 1
                    }
    
                    this.contactsByState.push(itemToAdd);
                }
                else{
                    exist += 1;
                }
            }

            if(contact.hasRrhhInterview || contact.hasTechnicalInterview || contact.hasClientInterview){
                intervieweds.count += 1;

                if(contact.reasonCauseType == ReasonCauseType.ApplicantInCompany) intervieweds.groupByStates.inCompany += 1;
                if(contact.reasonCauseType == ReasonCauseType.ApplicantInProgress) intervieweds.groupByStates.inProcess += 1;
                if(contact.reasonCauseType == ReasonCauseType.ApplicantOpen || 
                   contact.reasonCauseType == ReasonCauseType.ApplicantContacted) intervieweds.groupByStates.rejected += 1;
            }
            else{
                notIntervieweds.count += 1;

                if(contact.reasonCauseType == ReasonCauseType.ApplicantInCompany) notIntervieweds.groupByStates.inCompany += 1;
                if(contact.reasonCauseType == ReasonCauseType.ApplicantInProgress) notIntervieweds.groupByStates.inProcess += 1;
                if(contact.reasonCauseType == ReasonCauseType.ApplicantOpen || 
                   contact.reasonCauseType == ReasonCauseType.ApplicantContacted) notIntervieweds.groupByStates.rejected += 1;
            } 
        });

        this.buildFifthReport(intervieweds, notIntervieweds);

        this.applicantsInterviewedData = {
            labels: ["Entrevistados", "Sin Entrevistar"],
            datasets: [
              {
                data: [intervieweds.count, notIntervieweds.count],
                backgroundColor: this.getBackgroudColors(2),
                fill: false,
                borderWidth: 1,
              },
            ],
        };

        this.buildFourthReport(intervieweds);
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

        // this.options = {
        //     scales: {
        //         yAxes: [{
        //             ticks: {
        //                 beginAtZero: true
        //             }
        //         }]
        //     }
        // }

        this.chartClick(null);
    }

    buildFirstReport(){
        this.options = {
            tooltips: {
                callbacks: {
                    label: function(tooltipItem, data) {
                        var info = data.datasets[tooltipItem.datasetIndex].data || null;
                        var label = data.labels[tooltipItem.index] || "";

                        if (info) {
                            const reducer = (accumulator, currentValue) => accumulator + currentValue;

                            var total = info.reduce(reducer);
                            var value = info[tooltipItem.index];
                            var percentage = (value / total) * 100;

                            return `${label} - Cantitdad: ${value} - Porcentaje: ${Math.round(percentage)}%`
                        }

                        return "";
                    }
                }
            }
        }

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
