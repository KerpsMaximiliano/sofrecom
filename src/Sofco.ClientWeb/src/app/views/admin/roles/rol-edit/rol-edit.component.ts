import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Option } from 'app/models/option';
import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/common/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { FunctionalityService } from "app/services/admin/functionality.service";
import { ModuleService } from "app/services/admin/module.service";
import { RoleService } from "app/services/admin/role.service";
import { MenuService } from "app/services/admin/menu.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Role } from "app/models/admin/role";
declare var $: any;

@Component({
  selector: 'app-rol-edit',
  templateUrl: './rol-edit.component.html',
  styleUrls: ['./rol-edit.component.css']
})
export class RolEditComponent implements OnInit, OnDestroy {
    public functId: number = 0;
    public moduleId: number = 0;

    public role: Role = <Role>{};

    private id: number;
    public moduleSelected: number = 0;
    
    private paramsSubscrip: Subscription;
    private getSubscrip: Subscription;
    private editSubscrip: Subscription;
    private modulesSubscrip: Subscription;
    private functionalitiesToAddSubscrip: Subscription;

    //EntitiesService
    public isLoading: boolean = false;
    public editMode: boolean = false;
    public checkAtLeft:boolean = true;
    public functionalitiesToAdd: any[] = new Array<any>();
    public allModules: any[] = new Array<any>();
    public allFunctionalities: any[] = new Array<any>();

    @ViewChild('moduleModal') moduleModal;
    @ViewChild('functModal') functModal;
    @ViewChild('confirmModal') confirmModal;
    @ViewChild('functConfirmModal') functConfirmModal;

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmDelete",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public confirmFunctModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmDelete",
        "functConfirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public moduleModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ADMIN.ROLES.addModule", //title
        "moduleModal", //id
        true,          //Accept Button
        true,          //Cancel Button
        "ACTIONS.ACCEPT",     //Accept Button Text
        "ACTIONS.cancel");   //Cancel Button Text

    public functModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ADMIN.ROLES.addFunctionality", //title
        "functModal", //id
        true,          //Accept Button
        true,          //Cancel Button
        "ACTIONS.ACCEPT",     //Accept Button Text
        "ACTIONS.cancel");   //Cancel Button Text

    constructor(
        private service: RoleService, 
        private moduleService: ModuleService,
        private activatedRoute: ActivatedRoute, 
        private router: Router,
        public menuService: MenuService,
        private messageService: MessageService,
        private functionalityService: FunctionalityService,
        private errorHandlerService: ErrorHandlerService) { 
    }

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getDetails();
            this.getAllModules();
        });
    }

    ngOnDestroy(){
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.editSubscrip) this.editSubscrip.unsubscribe();
        if(this.modulesSubscrip) this.modulesSubscrip.unsubscribe();
        if(this.functionalitiesToAddSubscrip) this.functionalitiesToAddSubscrip.unsubscribe();
    }

    getDetails(){
        this.getSubscrip = this.service.getDetail(this.id).subscribe(
            data => {
                this.role = data;
            },
            err => this.errorHandlerService.handleErrors(err));
    }
 
    onSubmit(form){
        this.messageService.showLoading();

        var model = {
            id: this.role.id,
            description: this.role.description,
            active: this.role.active
        }

        this.editSubscrip = this.service.edit(model).subscribe(
            data => {
                this.messageService.closeLoading();

                if(data.messages) this.messageService.showMessages(data.messages);
                this.router.navigate(["/admin/roles"])
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    getAllModules(){
        this.modulesSubscrip = this.moduleService.getOptionsWithFunctionalities().subscribe(
            data => {
                this.allModules = data;
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    addFunctionality(){
        this.functModal.show();
        this.moduleSelected = this.allModules[0].id;
        this.loadFunctionalities(this.allModules[0].id);
    }

    loadFunctionalities(moduleId){
        this.moduleId = moduleId;

        this.functionalitiesToAdd = new Array<any>();
        var functionalities = new Array<any>();
        var moduleIndex = this.role.modules.findIndex(m => m.id == moduleId);
        var index = 0;
        var allModulesIndex = this.allModules.findIndex(x => x.id == moduleId);

        if(moduleIndex > -1){
            for(var i: number = 0; i<this.allModules[allModulesIndex].functionalities.length; i++){

                if(!this.isOptionInArray(this.role.modules[moduleIndex].functionalities, this.allModules[allModulesIndex].functionalities[i]) ){
                    this.allModules[allModulesIndex].functionalities[i].included = false;
                    this.allModules[allModulesIndex].functionalities[i].index = index;
                    functionalities.push(this.allModules[allModulesIndex].functionalities[i]);
                    index++;
                }
            }
        }
        else{
            for(var i: number = 0; i<this.allModules[allModulesIndex].functionalities.length; i++){
                this.allModules[allModulesIndex].functionalities[i].included = false;
                this.allModules[allModulesIndex].functionalities[i].index = index;
                functionalities.push(this.allModules[allModulesIndex].functionalities[i]);
                index++;
            }
        }
        
        this.functionalitiesToAdd = functionalities;
    }

    hasFunctionalitySelected(){
        return this.functionalitiesToAdd.findIndex(x => x.included) > -1;
    }

    private isOptionInArray(arr: any[], option: any): boolean{
        var esta: boolean = false;

        for(var i: number = 0; i<arr.length; i++){
            if(arr[i]["id"].toString() == option.id ){
                esta = true;
                break;
            }
        }

        return esta;
    }

    openConfirmModel(moduleId){
        this.moduleId = moduleId;
        this.confirmModal.show();
    }

    openConfirmFunctModel(functId){
        this.functId = functId;
        this.functConfirmModal.show();
    }

    unAssignFunct(){
        this.isLoading = true;

        this.service.unAssignFunctionality(this.role.id, this.functId).subscribe(
            data => {
                this.isLoading = false;
                this.functConfirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
            },
            err => {
                this.isLoading = false;
                this.functConfirmModal.hide();
                this.errorHandlerService.handleErrors(err);
            }
        );
    }

    unAssignModule(moduleId: number){
        var moduleToRemove = <any>this.role.modules.filter(x => x.id == moduleId);

        var arrFunctsToAdd = moduleToRemove[0].functionalities.map((item) => {
            return item.id;
        });

        this.isLoading = true;

        this.service.unAssignFunctionalities(this.role.id , arrFunctsToAdd).subscribe(
            data => {
                this.isLoading = false;
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
                this.confirmModal.hide();
            },
            err => {
                this.isLoading = false;
                this.confirmModal.hide();
                this.errorHandlerService.handleErrors(err);
            });
    }

    assignFunctionalities(){
        if(!this.hasFunctionalitySelected()){
            this.functModal.hide();
        }
        else{
            var arrFunctsToAdd = this.functionalitiesToAdd.filter((el)=> el.included).map((item) => {
                return item.id;
            });

            this.isLoading = true;

            this.service.assignFunctionalities(this.role.id , arrFunctsToAdd).subscribe(
                data => {
                    this.isLoading = false;
                    if(data.messages) this.messageService.showMessages(data.messages);
                    this.getDetails();
                    this.functModal.hide();
                },
                err => {
                    this.isLoading = false;
                    this.functModal.hide();
                    this.errorHandlerService.handleErrors(err);
                });
        }
    }
}
