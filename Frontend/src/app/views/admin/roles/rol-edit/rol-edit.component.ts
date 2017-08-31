import { Module } from 'models/module';
import { ModuleService } from 'app/services/module.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Option } from 'models/option';
import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RoleService } from 'app/services/role.service';
import { FunctionalityService } from 'app/services/functionality.service';
import { Role } from 'models/role';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Functionality } from "models/functionality";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
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
    public moduleSelected: string = "";
    
    private paramsSubscrip: Subscription;
    private getSubscrip: Subscription;
    private editSubscrip: Subscription;
    private modulesSubscrip: Subscription;
    private functionalitiesToAddSubscrip: Subscription;

  //EntitiesService
    public editMode: boolean = false;
    public checkAtLeft:boolean = true;
    public modulesToAdd: Option[] = new Array<Option>();
    public functionalitiesToAdd: Option[] = new Array<Option>();
    public allModules: any[] = new Array<any>();
    public allFunctionalities: any[] = new Array<any>();

    @ViewChild('moduleModal') moduleModal;
    @ViewChild('functModal') functModal;
    @ViewChild('confirmModal') confirmModal;
    @ViewChild('functConfirmModal') functConfirmModal;

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Confirmación de baja",
        "confirmModal",
        true,
        true,
        "Aceptar",
        "Cancelar"
    );

    public confirmFunctModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Confirmación de baja",
        "functConfirmModal",
        true,
        true,
        "Aceptar",
        "Cancelar"
    );

    public moduleModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Asignar Módulos", //title
        "moduleModal", //id
        true,          //Accept Button
        true,          //Cancel Button
        "Aceptar",     //Accept Button Text
        "Cancelar");   //Cancel Button Text

    public functModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Asignar Funcionalidades", //title
        "functModal", //id
        true,          //Accept Button
        true,          //Cancel Button
        "Aceptar",     //Accept Button Text
        "Cancelar");   //Cancel Button Text

    constructor(
        private service: RoleService, 
        private moduleService: ModuleService,
        private activatedRoute: ActivatedRoute, 
        private router: Router,
        private messageService: MessageService,
        private functionalityService: FunctionalityService,
        private errorHandlerService: ErrorHandlerService) { 
    }

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getDetails();
            this.getAllModules();
            this.getAllFunctionalities();
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
        this.getSubscrip = this.service.getDetail(this.id).subscribe((data) => {
        this.role = data;
        });
    }
 
    onSubmit(form){
        if(!form.invalid){
        var model = {
            id: this.role.id,
            description: this.role.description,
            active: this.role.active
        }

        this.editSubscrip = this.service.edit(model).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.router.navigate(["/admin/roles"])
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }

    addFunctionality(moduleId, moduleDescription){
        this.moduleSelected = moduleDescription;
        this.moduleId = moduleId;

        this.functionalitiesToAdd = new Array<Option>();
        var functionalities = new Array<Option>();
        var moduleIndex = this.role.modules.findIndex(m => m.id == moduleId);
        var index = 0;

        for(var i: number = 0; i<this.allFunctionalities.length; i++){

            if(!this.isOptionInArray(this.role.modules[moduleIndex].functionalities, this.allFunctionalities[i]) ){
                this.allFunctionalities[i].included = false;
                this.allFunctionalities[i].index = index;
                functionalities.push(this.allFunctionalities[i]);
                index++;
            }
        }
        
        this.functionalitiesToAdd = functionalities;
        this.functModal.show();
    }

    getAllModules(){
        this.modulesSubscrip = this.moduleService.getOptions().subscribe(data => {
            this.allModules = data;
        });
    }

    getAllFunctionalities(){
        this.functionalitiesToAddSubscrip = this.functionalityService.getOptions().subscribe(data => {
            this.allFunctionalities = data;
        });
    }

    //si me viene moduleId cargo solo ese modulo
    //si no me viene cargo todos los que no esten asociados al rol
    selectModules(moduleId: number = null){
        this.modulesToAdd = new Array<any>();
        var localModules = new Array<Option>();
        var index = 0;

        for(var i: number = 0; i < this.allModules.length; i++){

            if(!this.isOptionInArray(this.role.modules, this.allModules[i]) ){
                this.allModules[i].included = false;
                this.allModules[i].index = index;
                localModules.push(this.allModules[i]);
                index++;
            }
        }

        this.modulesToAdd = localModules;
    }

    addModule(){
        this.selectModules();
        this.editMode = false;
        this.moduleModal.show();
    }

    private isOptionInArray(arr: any[], option: Option): boolean{
        var esta: boolean = false;

        for(var i: number = 0; i<arr.length; i++){
            if(arr[i]["id"].toString() == option.value ){
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

    openConfirmFunctModel(moduleId, functId){
        this.moduleId = moduleId;
        this.functId = functId;
        this.functConfirmModal.show();
    }

    unAssignFunct(){
   

        this.moduleService.unAssignFunctionality(this.moduleId, this.functId).subscribe(
            data => {
                this.functConfirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
            },
            err => {
                this.functConfirmModal.hide();
                this.errorHandlerService.handleErrors(err);
            }
        );
    }

    unAssignModule(moduleId: number){
        this.service.unAssignModule(this.id, moduleId).subscribe(
            data => {
                this.confirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
            },
            err => {
                this.confirmModal.hide();
                this.errorHandlerService.handleErrors(err);
            }
        );
    }

    assignModules(){
        var arrGroupsToAdd = this.modulesToAdd.filter((el)=> el.included).map((item) => {
            return item.value
        });

        this.service.assignModules(this.id, arrGroupsToAdd).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
                this.moduleModal.hide();
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    assignFunctionalities(){
        var arrFunctsToAdd = this.functionalitiesToAdd.filter((el)=> el.included).map((item) => {
            return item.value
        });

        this.moduleService.assignFunctionalities(this.moduleId , arrFunctsToAdd).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
                this.functModal.hide();
            },
            err => this.errorHandlerService.handleErrors(err));
    }
}
