import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DataTableService } from '../../../../services/common/datatable.service';
import { Router } from '@angular/router';
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
import { Subscription } from 'rxjs';
import { PurchaseOrderApprovalDelegateService } from '../../../../services/billing/purchase-order-approval-delegate.service';

@Component({
    selector: 'app-purchase-order-approval-delegate',
    templateUrl: './purchase-order-approval-delegate.component.html'
  })

export class PurchaseOrderApprovalDelegateComponent implements OnInit, OnDestroy {

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

    constructor(private purchaseOrderDelegateService: PurchaseOrderApprovalDelegateService,
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
            order: [[ 0, 'asc' ]],
            columnDefs: [ {"aTargets": [5], "sType": "date-uk"} ]
        });
    }

    getDelegates() {
        this.subscription = this.purchaseOrderDelegateService.getAll().subscribe(response => {
            this.delegates = response.data;
            this.initTable();
        });
    }

    goToAdd() {
        this.router.navigate(['/billing/purchaseOrders/approval/delegate/edit']);
    }

    showConfirmDelete(item: any) {
        this.delegeteSelected = item;
        this.confirmModal.show();
    }

    processDelete() {
        this.confirmModal.hide();
        this.subscription = this.purchaseOrderDelegateService.delete(this.delegeteSelected.id).subscribe(response => {
            this.getDelegates();
        });
    }
}
