import { OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { FormControl, Validators } from "@angular/forms";
import { GenericOptions } from "app/models/enums/genericOptions";

declare var moment: any;

export class GenericOptionComponent implements OnDestroy {
 
    getSubscrip: Subscription;
    updateSubscrip: Subscription;
    addSubscrip: Subscription;
    activateSubscrip: Subscription;

    public list: any[] = new Array();
    public listFiltered: any[] = new Array();
    public actives: boolean = true;

    private entity: string;

    isReasonCause: boolean;
    public title: string;
    public id: number = 0;
    public text = new FormControl('', [Validators.required, Validators.maxLength(75)]);

    columns: any[] = new Array();

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(public dataTableService: DataTableService,
        public genericOptionService: GenericOptionService,
        public messageService: MessageService) {
    }

    ngOnDestroy(): void {
        if (this.getSubscrip) this.getSubscrip.unsubscribe();
        if (this.updateSubscrip) this.updateSubscrip.unsubscribe();
        if (this.addSubscrip) this.addSubscrip.unsubscribe();
        if (this.activateSubscrip) this.activateSubscrip.unsubscribe();
    }

    setEntity(controller){
        this.genericOptionService.controller = controller;
        this.entity = controller;
        this.getAll();

        if(this.entity == GenericOptions.ReasonCause){
            this.isReasonCause = true;
        }
    }

    getAll() {
        this.messageService.showLoading();

        this.getSubscrip = this.genericOptionService.getAll().subscribe(response => {
            this.messageService.closeLoading();
            this.list = response.data;
            this.filterActives(true);
        },
        () => this.messageService.closeLoading());
    }

    habInhabClick(item) {
        var newState = !item.active;
        
        this.messageService.showConfirm(() => {

            this.activateSubscrip = this.genericOptionService.active(item.id, newState).subscribe(() => {
                this.messageService.closeLoading();
                item.active = newState;
                this.filterActives(this.actives);
            },
            () => {
                this.messageService.closeLoading();
            });
 
        });
    }

    filterActives(active){
        this.actives = active;

        if(active){
            this.listFiltered = this.list.filter((element) => {
                return element.active == true;
            });
        }
        else{
            this.listFiltered = this.list.filter((element) => {
                return element;
            });
        }

        this.initGrid();
    }

    initGrid() {
        var title = `${this.entity}-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#table',
            columns: this.columns,
            title: title,
            withExport: true,
        }

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }

    add(){
        this.confirmModal.show();
    }

    update(item){
        this.id = item.id;
        this.text.setValue(item.text);

        this.confirmModal.show();
    }

    save(json, callback = null){
        if(json.id > 0){
            this.updateSubscrip = this.genericOptionService.edit(json).subscribe(response => {
                this.confirmModal.hide();

                var item = this.list.find(x => x.id == json.id);

                if(item) {
                    item.text = json.text;
                    
                    if(callback) callback(this, item);
                };

                this.id = 0;
                this.text.setValue("");
            },
            () => this.confirmModal.hide());
        }
        else{
            this.addSubscrip = this.genericOptionService.add(json).subscribe(response => {
                this.confirmModal.hide();
                
                if(callback) {
                    callback(this, response.data);
                }
                else{
                    this.list.push({ id: response.data, text: json.text, active: true });
                }

                this.filterActives(this.actives);

                this.text.setValue("");
            },
            () => this.confirmModal.hide());
        }
    }
}