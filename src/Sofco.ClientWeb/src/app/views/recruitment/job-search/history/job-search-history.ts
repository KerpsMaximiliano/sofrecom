import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { DataTableService } from '../../../../services/common/datatable.service';
import { JobSearchService } from 'app/services/recruitment/jobsearch.service';
import { JobSearchStatus } from 'app/models/enums/jobSearchStatus';

@Component({
  selector: 'job-search-history',
  templateUrl: './job-search-history.html',
})
export class JobSearchHistoryComponent implements OnDestroy {

    public histories: any[] = new Array<any>();
    
    getHistoriesSubscrip: Subscription;

    constructor(private jobSearchService: JobSearchService,
                private datatableService: DataTableService) {
    }

    getHistories(id){
        this.histories = [];

        this.getHistoriesSubscrip = this.jobSearchService.getHistories(id).subscribe(response => {
            this.histories = response.data;

            var options = {
                selector: "#historyTable",
                columnDefs: [ { "aTargets": [0], "sType": "date-uk" }],
            };

            this.datatableService.destroy(options.selector);
            this.datatableService.initialize(options);
        });
    }

    ngOnDestroy(){
        if(this.getHistoriesSubscrip) this.getHistoriesSubscrip.unsubscribe();
    }

    getStatusDesc(id){
        switch(id){
            case JobSearchStatus.Open: return "Abierta";
            case JobSearchStatus.Reopen: return "Re-Abierta";
            case JobSearchStatus.Suspended: return "Suspendida";
            case JobSearchStatus.Close: return "Cerrada";
        }
    }
}