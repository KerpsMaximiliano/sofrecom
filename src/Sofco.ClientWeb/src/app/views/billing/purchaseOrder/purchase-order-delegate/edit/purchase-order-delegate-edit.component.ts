import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { MessageService } from 'app/services/common/message.service';
import { I18nService } from 'app/services/common/i18n.service';
import { UserService } from 'app/services/admin/user.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { PurchaseOrderDelegateService } from 'app/services/billing/purchase-order-delegate.service';
import { UtilsService } from 'app/services/common/utils.service';
import { PurchaseOrderDelegateModel } from 'app/models/billing/purchase-order/purchase-order-delegate-model';
declare var $: any;

@Component({
    selector: 'app-purchase-delegate-edit',
    templateUrl: './purchase-order-delegate-edit.component.html'
  })

export class PurchaseOrderDelegateEditComponent implements OnInit, OnDestroy {

    private nullId = '';

    subscription: Subscription;

    @Input()
    public model: PurchaseOrderDelegateModel;

    private idKey = 'id';
    private textKey = 'text';

    private PurchaseOrderCompliance = "1";
    private PurchaseOrderCommercialId = "2";
    private PurchaseOrderOperationId = "3";
    private PurchaseOrderDaf = "4";

    private typeData: any[] = new Array<any>();
    private types: any[] = new Array<any>();
    public typeId: string = null;

    private areas: any[] = new Array<any>();
    public areaId: string = null;

    private sectors: any[] = new Array<any>();
    public sectorId: string = null;

    public users: any[] = new Array<any>();
    public userId: string = null;

    public responsable: any = null;

    constructor(private utilsService: UtilsService,
        private usersService: UserService,
        private purchaseOrderDelegateService: PurchaseOrderDelegateService,
        private errorHandlerService: ErrorHandlerService,
        private messageService: MessageService,
        private i18nService: I18nService,
        private router: Router) {
    }

    ngOnInit() {
        this.getTypes();
        this.getAreas();
        this.getSectors();
        this.getUsers();
    }

    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    getTypes() {
        this.subscription = this.utilsService.getUserDelegateTypes().subscribe(data => {
            this.typeData = data;
            this.types = this.cleanTypeDelgate(data);
            const self = this;
            this.initSelect2Control(this.types, '#typeControl', 'typeId', function() { self.clearControls(); }, function() { self.clearControls(); });
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    clearControls() {
        this.responsable = null;
        this.areaId = null;
        this.sectorId = null;
        $('#areaControl').val([]).trigger('change');
        $('#sectorControl').val([]).trigger('change');
    }

    getAreas() {
        this.subscription = this.purchaseOrderDelegateService.getAreas().subscribe(data => {
            this.areas = data;
            const self = this;
            this.initSelect2Control(this.areas, '#areaControl', 'areaId', function(){
                self.responsable = null;
                const item = self.areas.find(x => x.id == self.areaId);
                if(item.responsableUser == null) return;
                self.responsable = item.responsableUser;
            }, function() { self.responsable = null; });
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    getSectors() {
        this.subscription = this.purchaseOrderDelegateService.getSectors().subscribe(data => {
            this.sectors = data;
            const self = this;
            this.initSelect2Control(this.sectors, '#sectorControl', 'sectorId', function(){
                self.responsable = null;
                const item = self.sectors.find(x => x.id == self.sectorId);
                if(item.responsableUser == null) return;
                self.responsable = item.responsableUser;
            }, function() { self.responsable = null; });
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    cleanTypeDelgate(data: any[]) {
        const result = data.filter(s => s.id !== 0);

        result.forEach(x => x.text = this.i18nService.translateByKey('billing.purchaseOrder.delegate.' + x.text));

        return result;
    }

    initSelect2Control(dataSource, controlSelector, model, selectCallback = null, unselectCallback = null) {
        const data = this.mapToSelect(dataSource);
        const self = this;

        $(controlSelector).select2({
            data: data,
            allowClear: true,
            placeholder: this.i18nService.translateByKey('selectAnOption'),
            width: '100%'
        });
        $(controlSelector).on('select2:select', function(evt){
            const item = evt.params.data;
            self[model] = item.id === this.nullId ? null : item.id;
            if(selectCallback != null) { selectCallback(); }
        });
        $(controlSelector).on('select2:unselecting', function(){
            self[model] = null;
            if(unselectCallback != null) { unselectCallback(); }
        });
    }

    mapToSelect(data: Array<any>, selectedOption: string = ""): Array<any> {
        const result = new Array<any>();
        result.push({id: this.nullId, text: ''});
        data.forEach(s => {
            const text = s[this.textKey];
            result.push({
                id: s[this.idKey],
                text: text,
                selected: false
            });
        });
        return result;
    }

    getUsers(): void {
        this.subscription = this.usersService.getOptions().subscribe(res => {
            this.users = res;
            this.initSelect2Control(this.users, '#userControl', 'userId');
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    showArea(): boolean {
        return this.typeId === this.PurchaseOrderCommercialId;
    }

    showSector(): boolean {
        return this.typeId === this.PurchaseOrderOperationId;
    }

    showUser(): boolean {
        return this.areaId !== null || this.sectorId !== null;
    }

    showSave(): boolean {
        return this.userId !== null;
    }

    showResponsable() {
        return this.responsable != null;
    }

    save(): void {
        const model = new PurchaseOrderDelegateModel();
        model.responsableId = this.responsable.id;
        model.sourceId = this.getSourceId();
        model.userId = parseInt(this.userId);
        model.type = this.typeId;

        this.subscription = this.purchaseOrderDelegateService.save(model).subscribe(users => {
            this.messageService.succes('billing.solfac.delegate.saveSuccess');
            this.router.navigate(['/billing/purchaseOrders/delegate']);
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    getSourceId(): number {
        if(this.areaId != null) return parseInt(this.areaId);

        if(this.sectorId != null) return parseInt(this.sectorId);

        return 0;
    }
}
