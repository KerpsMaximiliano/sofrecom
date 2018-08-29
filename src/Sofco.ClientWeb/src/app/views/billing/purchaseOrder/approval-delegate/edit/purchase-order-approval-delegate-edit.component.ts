import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { MessageService } from '../../../../../services/common/message.service';
import { I18nService } from '../../../../../services/common/i18n.service';
import { UserService } from '../../../../../services/admin/user.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { PurchaseOrderApprovalDelegateService } from '../../../../../services/billing/purchase-order-approval-delegate.service';
import { UtilsService } from '../../../../../services/common/utils.service';
import { PurchaseOrderDelegateModel } from '../../../../../models/billing/purchase-order/purchase-order-delegate-model';
declare var $: any;

@Component({
    selector: 'app-purchase-approval-delegate-edit',
    templateUrl: './purchase-order-approval-delegate-edit.component.html'
  })

export class PurchaseOrderApprovalDelegateEditComponent implements OnInit, OnDestroy {

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

    private types: any[] = new Array<any>();
    public typeId: string = null;

    public sourceId: string = null;

    private areas: any[] = new Array<any>();
    public areaId: string = null;

    private sectors: any[] = new Array<any>();
    public sectorId: string = null;

    private compliances: any[] = new Array<any>();
    public complianceId: string = null;

    private dafs: any[] = new Array<any>();
    public dafId: string = null;

    public users: any[] = new Array<any>();
    public userId: string = null;

    public responsable: any = null;

    constructor(private utilsService: UtilsService,
        private usersService: UserService,
        private purchaseOrderDelegateService: PurchaseOrderApprovalDelegateService,
        private messageService: MessageService,
        private i18nService: I18nService,
        private router: Router) {
    }

    ngOnInit() {
        this.initSourceControl();
        this.getTypes();
        this.getAreas();
        this.getSectors();
        this.getCompliances();
        this.getDafs();
        this.getUsers();
    }

    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    getTypes() {
        this.subscription = this.utilsService.getUserDelegateTypes().subscribe(data => {
            this.types = this.cleanTypeDelgate(data);
            const self = this;
            this.initSelect2Control(this.types, '#typeControl', 'typeId', function() {
                self.clearControls();
                self.updateSourceControl();
            }, function() { self.clearControls(); });
        });
    }

    initSourceControl() {
        const self = this;
        this.initSelect2Control([], '#sourceControl', 'sourceId', function(){
            self.selectControlCallback();
        }, function() { self.responsable = null; });
    }

    clearControls() {
        this.responsable = null;
        this.areaId = null;
        this.sectorId = null;
        $('#areaControl').val([]).trigger('change');
        $('#sectorControl').val([]).trigger('change');
    }

    getAreas() {
        this.subscription = this.purchaseOrderDelegateService.getAreas().subscribe(res => {
            this.areas = res.data;
            const self = this;
            this.initSelect2Control(res.data, '#areaControl', 'areaId', function(){
                self.selectControlCallback();
            }, function() { self.responsable = null; });
        });
    }

    getSectors() {
        this.subscription = this.purchaseOrderDelegateService.getSectors().subscribe(res => {
            this.sectors = res.data;
            const self = this;
            this.initSelect2Control(res.data, '#sectorControl', 'sectorId', function(){
                self.selectControlCallback();
            }, function() { self.responsable = null; });
        });
    }

    getCompliances() {
        this.subscription = this.purchaseOrderDelegateService.getCompliances().subscribe(res => {
            this.compliances = res.data;
            const self = this;
            this.initSelect2Control(res.data, '#complianceControl', 'complianceId', function(){
                self.selectControlCallback();
            }, function() { self.responsable = null; });
        });
    }

    getDafs() {
        this.subscription = this.purchaseOrderDelegateService.getDafs().subscribe(res => {
            this.dafs = res.data;
            const self = this;
            this.initSelect2Control(res.data, '#dafControl', 'dafId', function(){
                self.selectControlCallback();
            }, function() { self.responsable = null; });
        });
    }

    selectControlCallback() {
        let data: any[];
        if(this.typeId == this.PurchaseOrderCompliance){
            data = this.compliances;
        }
        if(this.typeId == this.PurchaseOrderCommercialId){
            data = this.areas;
        }
        if(this.typeId == this.PurchaseOrderOperationId){
            data = this.sectors;
        }
        if(this.typeId == this.PurchaseOrderDaf){
            data = this.dafs;
        }
        const item = data.find(x => x.id == this.sourceId);

        this.responsable = null;
        if(item.responsableUser == null) return;
        this.responsable = item.responsableUser;
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
        });
    }

    showSource(): boolean {
        return this.typeId != null;
    }

    showUser(): boolean {
        return this.sourceId != null;
    }

    showSave(): boolean {
        return this.userId !== null;
    }

    showResponsable() {
        return this.responsable != null;
    }

    save(): void {
        const model = new PurchaseOrderDelegateModel();
        model.responsableId = this.getResponsableId();
        model.sourceId = this.getResponsableId();
        model.userId = parseInt(this.userId);
        model.type = this.typeId;

        this.subscription = this.purchaseOrderDelegateService.save(model).subscribe(users => {
            this.messageService.succes('billing.solfac.delegate.saveSuccess');
            this.router.navigate(['/billing/purchaseOrders/approval/delegate']);
        });
    }

    getSourceTitle(): string {
        const item = this.types.find(x => x.id == this.typeId);

        if(item == null) return "";

        return item.text;
    }

    updateSourceControl() {
        let data: any[];
        if(this.typeId == this.PurchaseOrderCompliance){
            data = this.compliances;
        }
        if(this.typeId == this.PurchaseOrderCommercialId){
            data = this.areas;
        }
        if(this.typeId == this.PurchaseOrderOperationId){
            data = this.sectors;
        }
        if(this.typeId == this.PurchaseOrderDaf){
            data = this.dafs;
        }
        const options = $('#sourceControl').data('select2').options.options;
        options.data = this.mapToSelect(data);
        $('#sourceControl').empty().select2(options);
    }

    getResponsableId(): number {
        if(this.typeId == this.PurchaseOrderCompliance || this.typeId == this.PurchaseOrderDaf){
            return parseInt(this.sourceId);
        }

        return parseInt(this.responsable.id);
    }
}
