import { UserService } from "app/services/admin/user.service";
import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";
import { DataTableService } from "app/services/common/datatable.service";
import { DelegationType } from "app/models/enums/delegationType";
import { DelegationService } from "app/services/admin/delegation.service";
import { MenuService } from "app/services/admin/menu.service";
import { PurchaseOrderApprovalDelegateService } from "app/services/billing/purchase-order-approval-delegate.service";

@Component({
    selector: 'delegation',
    templateUrl: './delegation.html'
})
export class DelegationComponent implements OnInit, OnDestroy {

    typeId: number;
    grantedUserId: number;
    analyticSourceId: number;
    userSourceId: number[];
    areaSourceId: number;
    sectorSourceId: number;
    sourceType = "2";

    sourceTypeDisabled: boolean;
    analyticDisabled: boolean;
    userSourceDisabled: boolean;

    types: any[] = new Array();
    users: any[] = new Array();
    resources: any[] = new Array();
    analytics: any[] = new Array();
    areas: any[] = new Array<any>(); 
    sectors: any[] = new Array<any>();
    data: any[] = new Array();
    resourcesByManager: any[] = new Array();

    addSubscript: Subscription;
    usersSubscript: Subscription;
    analyticsSubscript: Subscription;
    deleteSubscript: Subscription;
    getSubscript: Subscription;
    getResourcesSubscript: Subscription;
    getAreasSubscription: Subscription;
    getSectorsSubscription: Subscription;
 
    constructor(private messageService: MessageService,
                private i18nService: I18nService,
                private menuService: MenuService,
                private datatableService: DataTableService,
                private purchaseOrderDelegateService: PurchaseOrderApprovalDelegateService,
                private userDelegateService: DelegationService,
                private userService: UserService) { }

    ngOnInit(): void {
        this.usersSubscript = this.userService.getOptions().subscribe(response => {
            this.users = response;
        });

        this.analyticsSubscript = this.userDelegateService.getAnalytics().subscribe(response => {
            this.analytics = response.data;
        });

        if(this.menuService.hasFunctionality("DELEG", "MAN-REPORT-DELEGATE")){
            this.types.push({ id: DelegationType.ManagementReport, text: this.i18nService.translateByKey(DelegationType[DelegationType.ManagementReport]) });
        }

        if(this.menuService.hasFunctionality("DELEG", "ADVANCEMENT-DELEGATE")){
            this.types.push({ id: DelegationType.Advancement, text: "Aprobación Adelantos" });
        }

        if(this.menuService.hasFunctionality("DELEG", "REFUND-DELEGATE")){
            this.types.push({ id: DelegationType.RefundApprovall, text: "Aprobación Reintegros" });
        }

        if(this.menuService.hasFunctionality("DELEG", "ADD-REFUND-DELEGATE")){
            this.types.push({ id: DelegationType.RefundAdd, text: "Generación Reintegros" });
        }

        if(this.menuService.hasFunctionality("DELEG", "LICENSE-DELEGATE")){
            this.types.push({ id: DelegationType.LicenseAuthorizer, text: "Aprobación Licencias" });
        }

        if(this.menuService.hasFunctionality("DELEG", "WORKTIME-DELEGATE")){
            this.types.push({ id: DelegationType.WorkTime, text: "Aprobación Horas" });
        }
 
        if(this.menuService.hasFunctionality("DELEG", "SOLFAC-DELEGATE")){
            this.types.push({ id: DelegationType.Solfac, text: "Generación Solfac" }); 
        }

        if(this.menuService.hasFunctionality("DELEG", "OC-COMPLI-DELEGATE")){
            this.types.push({ id: DelegationType.PurchaseOrderApprovalCompliance, text: "Aprobación OC Compliance" }); 
        }

        if(this.menuService.hasFunctionality("DELEG", "OC-COMERC-DELEGATE")){
            this.types.push({ id: DelegationType.PurchaseOrderApprovalCommercial, text: "Aprobación OC Comercial" }); 
        }

        if(this.menuService.hasFunctionality("DELEG", "OC-DAF-DELEGATE")){
            this.types.push({ id: DelegationType.PurchaseOrderApprovalDaf, text: "Aprobación OC DAF" }); 
        }

        if(this.menuService.hasFunctionality("DELEG", "OC-OPERA-DELEGATE")){
            this.types.push({ id: DelegationType.PurchaseOrderApprovalOperation, text: "Aprobación OC Operativo" }); 
        }
        
        if(this.menuService.hasFunctionality("DELEG", "OC-VIEW-DELEGATE")){
            this.types.push({ id: DelegationType.PurchaseOrderActive, text: "Vista Ordenes de Compra" }); 
        }

        this.get();
        this.getResourcesByManager();
        this.getAreas();
        this.getSectors();
    }

    ngOnDestroy(): void {
        if(this.addSubscript) this.addSubscript.unsubscribe();          
        if(this.usersSubscript) this.usersSubscript.unsubscribe();          
        if(this.analyticsSubscript) this.analyticsSubscript.unsubscribe();          
        if(this.deleteSubscript) this.deleteSubscript.unsubscribe();          
        if(this.getSubscript) this.getSubscript.unsubscribe();          
        if(this.getResourcesSubscript) this.getResourcesSubscript.unsubscribe();          
    }

    get(){
        this.messageService.showLoading();

        this.getSubscript = this.userDelegateService.get().subscribe(response => {
            this.messageService.closeLoading();
            this.data = response.data;
            this.initGrid();
        },
        error => this.messageService.closeLoading());
    }

    getAreas() {
        this.getAreasSubscription = this.purchaseOrderDelegateService.getAreas().subscribe(res => {
            this.areas = res.data;
        });
    }

    getSectors() {
        this.getSectorsSubscription = this.purchaseOrderDelegateService.getSectors().subscribe(res => {
            this.sectors = res.data;
        });
    }
    
    isAreaType(){
        return this.typeId == DelegationType.PurchaseOrderApprovalCommercial;
    }

    isSectorType(){
        return this.typeId == DelegationType.PurchaseOrderApprovalOperation;
    }

    isPurchaseOrder(){
        return this.typeId == DelegationType.PurchaseOrderApprovalOperation ||
               this.typeId == DelegationType.PurchaseOrderApprovalCommercial ||
               this.typeId == DelegationType.PurchaseOrderApprovalDaf ||
               this.typeId == DelegationType.PurchaseOrderApprovalCompliance;
    }

    getResourcesByManager(){
        this.getSubscript = this.userDelegateService.getResources().subscribe(response => {
            this.resourcesByManager = response.data;
        });
    }

    setEmployees() {
        this.userSourceId = null;
        var analytic = this.analytics.find(x => x.id == this.analyticSourceId);

        if(analytic != null){
            this.resources = analytic.resources.map(user => {
                return { id: user.userId, text: user.text, selected: false };
            });

            this.initResourceGrid();
        }
        else{
            this.resources = [];
        }
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4];
        var title = `usuarios delegados`;

        var params = {
            selector: '#dataTable',
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [ {'aTargets': [4], "sType": "date-uk"} ]
          }

          this.datatableService.destroy(params.selector);
          this.datatableService.initialize(params);
    }

    saveEnabled(){
        if(!this.grantedUserId || this.grantedUserId <= 0 ||  !this.typeId || this.typeId <= 0){
            return false;
        }

        if(this.typeId == DelegationType.ManagementReport || this.typeId == DelegationType.Solfac || this.typeId == DelegationType.PurchaseOrderActive){
            if(!this.analyticSourceId || this.analyticSourceId <= 0) return false;

            if(this.sourceType == "3" && !this.anySelected()) return false;
        }

        if(this.typeId == DelegationType.WorkTime){
            if(!this.analyticSourceId || this.analyticSourceId <= 0) return false;

            if(!this.anySelected()) return false;
        }

        if(this.typeId == DelegationType.LicenseAuthorizer){
            if(!this.anySelected()) return false;
        }

        if(this.typeId == DelegationType.PurchaseOrderApprovalCommercial){
            if(!this.areaSourceId || this.areaSourceId <= 0) return false;
        }

        if(this.typeId == DelegationType.PurchaseOrderApprovalOperation){
            if(!this.sectorSourceId || this.sectorSourceId <= 0) return false;
        }

        return true;
    }

    save(){
        if(!this.saveEnabled()) return;

        if(this.sourceType == "2"){
            this.userSourceId = null;
        }

        if(!this.isPurchaseOrder() && this.sourceType == '3' && !this.userSourceDisabled){
            this.userSourceId = this.resources.filter(x => x.selected).map(x => x.id);
        }

        var model = {
            grantedUserId: this.grantedUserId,
            type: this.typeId,
            sourceType: this.sourceType,
            analyticSourceId: this.analyticSourceId,
            userSourceIds: this.userSourceId
        }

        if(this.typeId == DelegationType.PurchaseOrderApprovalCommercial){
            model.analyticSourceId = this.areaSourceId;
        }

        if(this.typeId == DelegationType.PurchaseOrderApprovalOperation){
            model.analyticSourceId = this.sectorSourceId;
        }

        this.messageService.showLoading();

        this.addSubscript = this.userDelegateService.post(model).subscribe(response => {
            this.messageService.closeLoading();
            
            this.get();
        },
        error => this.messageService.closeLoading());
    }

    delete(userDelegate){
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.deleteSubscript = this.userDelegateService.delete(userDelegate.id).subscribe(response => {
                this.messageService.closeLoading();

                var index = this.data.findIndex(x => x.id == userDelegate.id);
                this.data.splice(index, 1);

                this.initGrid();
            },
            error => this.messageService.closeLoading());
        });
    }

    typeChanged(){
        this.analyticSourceId = null;
        this.userSourceId = null;
        this.areaSourceId = null;
        this.sectorSourceId = null;
        this.resources = [];

        if(this.typeId == DelegationType.ManagementReport){
            this.sourceTypeDisabled = false;
            this.analyticDisabled = false;
            this.sourceType = "2";
        }

        if(this.typeId == DelegationType.Advancement){
            this.sourceTypeDisabled = true;
            this.analyticDisabled = true;
            this.userSourceDisabled = false;
            this.sourceType = "3";

            this.resources = [];
            this.analytics.forEach(x => {
                x.resources.forEach(user => {
                    if(this.resources.indexOf(u => u.id == user.userId) == -1){
                        this.resources.push({ id: user.userId, text: user.text, selected: false });
                    }
                });
            });

            this.initResourceGrid();
        }

        if(this.typeId == DelegationType.WorkTime){
            this.sourceTypeDisabled = true;
            this.analyticDisabled = false;
            this.userSourceDisabled = false;
            this.sourceType = "3";
        }

        if(this.typeId == DelegationType.LicenseAuthorizer){
            this.sourceTypeDisabled = true;
            this.analyticDisabled = true;
            this.userSourceDisabled = false;
            this.sourceType = "3";

            this.resources = this.resourcesByManager.map(s => {
                s.selected = false;
                return s;
            });

            this.initResourceGrid();
        }

        if(this.typeId == DelegationType.RefundApprovall || this.typeId == DelegationType.Solfac){
            this.sourceTypeDisabled = true;
            this.analyticDisabled = false;
            this.userSourceDisabled = false;
            this.sourceType = "2";
        }

        if(this.typeId == DelegationType.RefundAdd){
            this.sourceTypeDisabled = true;
            this.analyticDisabled = true;
            this.userSourceDisabled = true;
            this.sourceType = "3";
        }

        if(this.isPurchaseOrder()){
            this.sourceTypeDisabled = true;
            this.sourceType = "2";
        }

        if(this.typeId == DelegationType.PurchaseOrderActive){
            this.sourceTypeDisabled = true;
            this.sourceType = "2";
            this.analyticDisabled = false;
        }
    }

    selectAll(){
        this.resources.forEach((item, index) => {
            item.selected = true;
        });
    }

    unselectAll(){
        this.resources.forEach((item, index) => {
            item.selected = false;
        });
    }

    initResourceGrid() {
        var options = {
            selector: "#resourcesTable",
        };

        this.datatableService.destroy(options.selector);
        this.datatableService.initialize(options);
    }

    anySelected(){
        return this.resources.filter(x => x.selected).length > 0;
    }
}