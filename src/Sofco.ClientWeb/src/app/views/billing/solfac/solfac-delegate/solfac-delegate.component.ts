import { Component, Input, Output, OnInit, OnDestroy } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { I18nService } from 'app/services/common/i18n.service';
import { DataTableService } from 'app/services/common/datatable.service';
import { SolfacDelegateService } from 'app/services/billing/solfac-delegate.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
declare var $: any;

@Component({
    selector: 'app-solfac-delegate',
    templateUrl: './solfac-delegate.component.html'
  })

export class SolfacDelegateComponent implements OnInit, OnDestroy {

    private nullId = '';

    private subscription: Subscription;

    public delegates: any[] = new Array<any>();

    constructor(private solfacDelegateService: SolfacDelegateService,
        private errorHandlerService: ErrorHandlerService,
        private dataTableService: DataTableService,
        private i18nService: I18nService,
        private router: Router) {

    }

    ngOnInit() {
        this.getDelegates();
        this.initTable();
    }

    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    initTable() {
        this.dataTableService.destroy('#delegateTable');
        this.dataTableService.init('#delegateTable', false);
    }

    getDelegates() {
        this.subscription = this.solfacDelegateService.getAll().subscribe(response => {
            this.delegates = this.sortDelegate(response.data);
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    sortDelegate(data: Array<any>) {
        return data.sort(function (a, b) {
            return a.serviceName.localeCompare(b.serviceName);
          });
    }

    goToAdd() {
        this.router.navigate(['/billing/solfac/delegate/edit']);
    }
}
