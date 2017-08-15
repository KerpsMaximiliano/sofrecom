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

  public role: Role = <Role>{};

  private id: number;
  
  private paramsSubscrip: Subscription;
  private getSubscrip: Subscription;
  private editSubscrip: Subscription;
  public  entsToAddSubscrip: Subscription;

  //EntitiesService
  public checkAtLeft:boolean = true;
  public entitiesToAdd: Option[];
  public funcsToAdd: Option[];
  @ViewChild('modalEntities') modalEntities;
  @ViewChild('modalFunc') modalFunc;
  public entitiesModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
    "Asignar Entidades", //title
    "modalGroups", //id
    true,          //Accept Button
    false,          //Cancel Button
    "Aceptar",     //Accept Button Text
    "Cancelar");   //Cancel Button Text

  public funcModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
    "Asignar Funcionalidades", //title
    "modalFunc",   //id
    true,          //Accept Button
    false,          //Cancel Button
    "Aceptar",     //Accept Button Text
    "Cancelar");   //Cancel Button Text

  constructor(
    private service: RoleService, 
    private entityService: ModuleService,
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
    if(this.entsToAddSubscrip) this.entsToAddSubscrip.unsubscribe();
  }

  getDetails(){
    this.getSubscrip = this.service.getDetail(this.id).subscribe((data) => {
      this.role = data;

      this.getAllEntities();
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

  
    getAllEntities(){
        this.entsToAddSubscrip = this.entityService.getOptions().subscribe(data => {
            this.entitiesToAdd = null;
            var entities = new Array<Option>();
            var index = 0;
            for(var i: number = 0; i<data.length; i++){

                if(!this.isOptionInArray(this.role.modules, data[i]) ){
                    data[i].included = false;
                    data[i].index = index;
                    entities.push(data[i]);
                    index++;
                }
                
            }

            this.entitiesToAdd = entities;
        });
    }

    addFunctionalities(entityId: number){
        this.getAllFunctionalities(entityId);
        this.modalFunc.show();
    }

    getAllFunctionalities(entityId: number){
        this.entsToAddSubscrip = this.funcService.getOptions().subscribe(data => {
            this.funcsToAdd = null;
            var funcs = new Array<Option>();
            var entityIndex = this.role.modules.findIndex(x => x.id == entityId);
            var index = 0;
            for(var i: number = 0; i<data.length; i++){

                if(!this.isOptionInArray(this.role.modules[entityIndex].functionalities, data[i]) ){
                    data[i].included = false;
                    data[i].index = index;
                    funcs.push(data[i]);
                    index++;
                }
                
            }

            this.funcsToAdd = funcs;
        });
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

    assignEntities(){

        var arrEntitiesToAdd = this.entitiesToAdd.filter((el)=> el.included).map((item) => {
            return item.value
        });

        var objToSend = {
            entitiesToAdd: arrEntitiesToAdd,
            entitiesToRemove: []
        };

        this.service.assignEntities(this.id, objToSend).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
                this.modalEntities.hide();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

    unassignEntity(entityId: number){
        this.service.unassignEntity(this.role.id, entityId).subscribe(
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



    assignFuncs(entityId: number){

        var arrFuncsToAdd = this.funcsToAdd.filter((el)=> el.included).map((item) => {
            return item.value
        });

        var objToSend = {
            functionalitiesToAdd: arrFuncsToAdd,
            functionalitiesToRemove: []
        };

        this.service.assignFunctionalities(this.id, entityId, objToSend).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
                this.modalFunc.hide();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

    unassignFunc(entityId: number, funcId: number){
        this.service.unassignFunctionality(this.role.id, entityId, funcId).subscribe(
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
}
