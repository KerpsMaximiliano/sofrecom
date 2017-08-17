import { Module } from 'models/module';
import { ModuleService } from 'app/services/module.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Functionality } from 'models/functionality';
import { FunctionalityService } from 'app/services/functionality.service';
import { Option } from 'models/option';
import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RoleService } from 'app/services/role.service';
import { Role } from 'models/role';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
declare var $: any;

@Component({
  selector: 'app-rol-edit',
  templateUrl: './rol-edit.component.html',
  styleUrls: ['./rol-edit.component.css']
})
export class RolEditComponent implements OnInit, OnDestroy {
  public functionalityId: number  = 0;
  public moduleId: number = 0;

  public role: Role = <Role>{};

  private id: number;
  
  private paramsSubscrip: Subscription;
  private getSubscrip: Subscription;
  private editSubscrip: Subscription;
  private modulesSubscrip: Subscription;
  private funcSubscrip: Subscription;

  //EntitiesService
  public editMode: boolean = false;
  public checkAtLeft:boolean = true;
  public cboModuleId: number;
  public cboModuleText: string;
  public modules: Option[];
  public funcsToAdd: Option[];
  private allModules: any[];
  private allFunctionalities: any[];

  @ViewChild('funcModal') funcModal;
  @ViewChild('confirmModal') confirmModal;

  private modalMessage: string;

  public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
    "Confirmación de baja",
    "confirmModal",
    true,
    true,
    "Aceptar",
    "Cancelar"
  );

  public funcModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
    "Asignar Módulo / Funcionalidades", //title
    "funcModal", //id
    true,          //Accept Button
    true,          //Cancel Button
    "Aceptar",     //Accept Button Text
    "Cancelar");   //Cancel Button Text

  constructor(
    private service: RoleService, 
    private moduleService: ModuleService,
    private funcService: FunctionalityService,
    private activatedRoute: ActivatedRoute, 
    private router: Router,
    private messageService: MessageService) { 
    
  }

  ngOnInit() {
    this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.id = params['id'];
        this.getDetails();
    });
  }

  ngOnDestroy(){
    if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    if(this.getSubscrip) this.getSubscrip.unsubscribe();
    if(this.editSubscrip) this.editSubscrip.unsubscribe();
    if(this.modulesSubscrip) this.modulesSubscrip.unsubscribe();
    if(this.funcSubscrip) this.funcSubscrip.unsubscribe();
    
  }

  getDetails(){
    this.getSubscrip = this.service.getDetail(this.id).subscribe((data) => {
      this.role = data;
      this.getAllModules();
      this.getAllFunctionalities();
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
        err => {
          var json = JSON.parse(err._body)
          if(json.messages) this.messageService.showMessages(json.messages);
        }
      );
    }
    
  }

  /*onActiveClick(active){
    if(typeof active == 'boolean'){
      this.entity.active = active;
    }
  }*/

    getAllModules(){
        this.modulesSubscrip = this.moduleService.getOptions().subscribe(data => {
            this.allModules = data;
        });
    }

    getAllFunctionalities(){
        this.funcSubscrip = this.funcService.getOptions().subscribe(data => {
            this.allFunctionalities = data;
        });
    }

    //si me viene moduleId cargo solo ese modulo
    //si no me viene cargo todos los que no esten asociados al rol
    selectModules(moduleId: number = null){
        this.modules = null;
        var localModules = new Array<Option>();
        var index = 0;

        if(moduleId){
            var modIndex = this.allModules.findIndex(x => x.value == moduleId);
            localModules.push(this.allModules[modIndex]);
        }else {
            for(var i: number = 0; i < this.allModules.length; i++){

                if(!this.isOptionInArray(this.role.modules, this.allModules[i]) ){
                    this.allModules[i].included = false;
                    this.allModules[i].index = index;
                    localModules.push(this.allModules[i]);
                    index++;
                }
            }
        }

        if (localModules.length > 0){
            this.cboModuleId = parseInt(localModules[0].value);
            this.cboModuleText = localModules[0].text;
        }

        this.modules = localModules;
    }

    addModule(){
        this.selectModules();
        this.selectFunctionalities();
        this.editMode = false;
        this.funcModal.show();
    }

    addFunctionalities(moduleId: number){
        this.selectModules(moduleId);
        this.selectFunctionalities(moduleId);
        this.editMode = true;
        this.funcModal.show();
    }

    selectFunctionalities(moduleId: number = null){
        this.funcsToAdd = null;
        var funcs = new Array<Option>();

        if (moduleId){
            var moduleIndex = this.role.modules.findIndex(x => x.id == moduleId);
            var index = 0;
            for(var i: number = 0; i<this.allFunctionalities.length; i++){

                if(!this.isOptionInArray(this.role.modules[moduleIndex].functionalities, this.allFunctionalities[i]) ){
                    this.allFunctionalities[i].included = false;
                    this.allFunctionalities[i].index = index;
                    funcs.push(this.allFunctionalities[i]);
                    index++;
                }
                
            }
        } else {
            funcs = this.allFunctionalities;
        }

        this.funcsToAdd = funcs;
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

    assignFunctionalities(){

        var arrFuncsToAdd = this.funcsToAdd.filter((el)=> el.included).map((item) => {
            return item.value
        });

        var objToSend = {
            functionalitiesToAdd: arrFuncsToAdd,
            functionalitiesToRemove: [],
            moduleId: this.cboModuleId
        };

        this.service.assignFunctionalities(this.id, objToSend).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
                this.funcModal.hide();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

    confirm(moduleId: number, functionalityId: number){ }

    removeFunctionality(moduleId: number, functionalityId: number){
        this.moduleId = moduleId;
        this.functionalityId = functionalityId;
        this.confirm = this.unAssignFunctionality;
        this.modalMessage = "¿Esta seguro que desea eliminar la funcionalidad?"
        this.confirmModal.show();
    }

    removeFunctionalities(moduleId: number){
        this.moduleId = moduleId;
        this.confirm = this.unAssignFunctionalities;
        this.modalMessage = "¿Esta seguro que desea eliminar el modulo con sus funcionalidades?"
        this.confirmModal.show();
    }

    unAssignFunctionalities(moduleId: number){
        var moduleIndex = this.role.modules.findIndex(m => m.id == moduleId);

        var arrFuncsToRem = this.role.modules[moduleIndex].functionalities
                                .map(x => x.id);

        var objToSend = {
            functionalitiesToAdd: [],
            functionalitiesToRemove: arrFuncsToRem,
            moduleId: moduleId
        };

        this.confirmModal.hide();

        this.service.unAssignFunctionalities(this.id, objToSend).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

    unAssignFunctionality(moduleId: number, functionalityId: number){
        this.confirmModal.hide();

        this.service.unAssignFunctionality(this.role.id, moduleId, functionalityId).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }


/*
    assignFuncs(entityId: number){

        var arrFuncsToAdd = this.funcsToAdd.filter((el)=> el.included).map((item) => {
            return item.value
        });

        var objToSend = {
            functionalitiesToAdd: arrFuncsToAdd,
            functionalitiesToRemove: []
        };

        this.service.assignFunctionalities(this.id, objToSend).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
                this.funcModal.hide();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

    unassignFunc(entityId: number, funcId: number){
        this.service.unAssignFunctionality(this.role.id, entityId, funcId).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }*/
}
