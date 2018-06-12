import { Component, Input, Output, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { I18nService } from 'app/services/common/i18n.service';
import { DataTableService } from 'app/services/common/datatable.service';
import { SolfacDelegateService } from 'app/services/billing/solfac-delegate.service';
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
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

    private delegeteSelected: any;

    public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        'ACTIONS.confirmTitle',
        'confirmModal',
        true,
        true,
        'ACTIONS.DELETE',
        'ACTIONS.cancel');

    @ViewChild('confirmModal') confirmModal;

    constructor(private solfacDelegateService: SolfacDelegateService,
        private errorHandlerService: ErrorHandlerService,
        private dataTableService: DataTableService,
        private messageService: MessageService,
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
        this.dataTableService.initialize({
            selector: '#delegateTable',
            order: [[ 0, 'asc' ]]
        });
    }

    getDelegates() {
        this.messageService.showLoading();
        this.subscription = this.solfacDelegateService.getAll().subscribe(response => {
            this.messageService.closeLoading();
            this.delegates = response.data;
            this.initTable();
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err);
        });
    }

    goToAdd() {
        this.router.navigate(['/billing/solfac/delegate/edit']);
    }

    showConfirmDelete(item: any) {
        this.delegeteSelected = item;
        this.confirmModal.show();
    }

    processDelete() {
        this.confirmModal.hide();
        this.subscription = this.solfacDelegateService.delete(this.delegeteSelected.id).subscribe(response => {
            this.getDelegates();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }
}
