import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { DataTableService } from '../../../../services/common/datatable.service';
import { JobSearchService } from 'app/services/recruitment/jobsearch.service';
import { JobSearchStatus } from 'app/models/enums/jobSearchStatus';
import { ApplicantStatus } from 'app/models/enums/applicantStatus';

@Component({
  selector: 'applicants-related',
  templateUrl: './applicants-by-js.html',
})
export class ApplicantsRelatedByJsComponent implements OnDestroy {

    public data: any[] = new Array<any>();
    
    getHistoriesSubscrip: Subscription;

    constructor(private jobSearchService: JobSearchService,
                private datatableService: DataTableService) {
    }

    getApplicants(id){
        this.data = [];

        var options = {
            selector: "#applicantsByJs",
            columnDefs: [ { "aTargets": [3], "sType": "date-uk" }],
        };

        this.datatableService.destroy(options.selector);

        this.getHistoriesSubscrip = this.jobSearchService.getApplicantsRelatedByJobsearch(id).subscribe(response => {
            this.data = response.data;

            this.datatableService.destroy(options.selector);
            this.datatableService.initialize(options);
        });
    }

    ngOnDestroy(){
        if(this.getHistoriesSubscrip) this.getHistoriesSubscrip.unsubscribe();
    }

    getStatusDesc(status){
        switch(status){
            case ApplicantStatus.Valid: return "Vigente";
            case ApplicantStatus.InProgress: return "En Curso";
            case ApplicantStatus.Close: return "Deshabilitado";
            case ApplicantStatus.InCompany: return "Ingresado";
            case ApplicantStatus.Contacted: return "Vigente/Contactado";
            default: return "";
        }
      }
}