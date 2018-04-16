import { Component, Input, Output, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { ServiceService } from 'app/services/billing/service.service';
import { CustomerService } from 'app/services/billing/customer.service';
import { UserService } from 'app/services/admin/user.service';
import { I18nService } from 'app/services/common/i18n.service';
import { DataTableService } from 'app/services/common/datatable.service';
import { MessageService } from 'app/services/common/message.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
declare var $: any;

@Component({
    selector: 'app-worktime',
    templateUrl: './worktime.component.html'
  })

export class WorkTimeComponent implements OnInit, OnDestroy {

    private subscription: Subscription;

    @Input()
    public model: any;

    private idKey = 'id';
    private textKey = 'text';

    private itemSelected: any;

    public loading = false;

    public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        'ACTIONS.confirmTitle',
        'confirmModal',
        true,
        true,
        'ACTIONS.DELETE',
        'ACTIONS.cancel');

    @ViewChild('confirmModal') confirmModal;

    constructor(private serviceService: ServiceService,
        private customerService: CustomerService,
        private usersService: UserService,
        private errorHandlerService: ErrorHandlerService,
        private dataTableService: DataTableService,
        private messageService: MessageService,
        private i18nService: I18nService,
        private router: Router) {
    }

    ngOnInit() {
    }

    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }
}
