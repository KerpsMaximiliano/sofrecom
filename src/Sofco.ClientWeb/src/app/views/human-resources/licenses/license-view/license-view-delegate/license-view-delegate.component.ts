import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { DataTableService } from 'app/services/common/datatable.service';
import { Router } from '@angular/router';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Subscription } from 'rxjs';
import { LicenseViewDelegateService } from 'app/services/human-resources/license-view-delegate.service';

@Component({
    selector: 'app-license-view-delegate',
    templateUrl: './license-view-delegate.component.html'
  })

export class LicenseViewDelegateComponent implements OnInit, OnDestroy {

    private subscription: Subscription;

    public delegates: any[] = new Array<any>();

    private delegeteSelected: any;

    public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        'ACTIONS.confirmTitle',
        'confirmModal',
        true,
        true,
        'ACTIONS.DELETE',
        'ACTIONS.cancel');

    @ViewChild('confirmModal') confirmModal;

    constructor(private licenseViewDelegateService: LicenseViewDelegateService,
        private errorHandlerService: ErrorHandlerService,
        private dataTableService: DataTableService,
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
        this.dataTableService.initialize({
            selector: '#delegateTable',
            order: [[ 0, 'asc' ]]
        });
    }

    getDelegates() {
        this.subscription = this.licenseViewDelegateService.getAll().subscribe(response => {
            this.delegates = response.data;
            this.initTable();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    goToAdd() {
        this.router.navigate(['/rrhh/licenses/views/delegates/edit']);
    }

    showConfirmDelete(item: any) {
        this.delegeteSelected = item;
        this.confirmModal.show();
    }

    processDelete() {
        this.confirmModal.hide();
        this.subscription = this.licenseViewDelegateService.delete(this.delegeteSelected.id).subscribe(response => {
            this.getDelegates();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }
}
