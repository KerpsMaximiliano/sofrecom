import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DataTableService } from '../../../../services/common/datatable.service';
import { SolfacDelegateService } from '../../../../services/billing/solfac-delegate.service';
import { MessageService } from '../../../../services/common/message.service';
import { Router } from '@angular/router';
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-solfac-delegate',
    templateUrl: './solfac-delegate.component.html'
  })

export class SolfacDelegateComponent implements OnInit, OnDestroy {


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
        private dataTableService: DataTableService,
        private messageService: MessageService,
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
        () => {
                this.messageService.closeLoading();
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
        this.subscription = this.solfacDelegateService.delete(this.delegeteSelected.id).subscribe(() => {
            this.getDelegates();
        });
    }
}
