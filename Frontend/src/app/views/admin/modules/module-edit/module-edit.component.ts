import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ModuleService } from 'app/services/module.service';
import { FunctionalityService } from 'app/services/functionality.service';
import { Module } from 'models/Module';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Option } from 'models/option';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";

@Component({
  selector: 'app-module-edit',
  templateUrl: './module-edit.component.html'
})
export class ModuleEditComponent implements OnInit, OnDestroy {

    public module: Module = <Module>{};
    private id: number;
    private paramsSubscrip: Subscription;
    private getSubscrip: Subscription;
    private editSubscrip: Subscription;
    private functSubscrip: Subscription;
    public functionalityId: number = 0;
    public allFunctionalities: any[] = new Array<any>();
    public functsToAdd: any[];
    public checkAtLeft:boolean = true;
 
    @ViewChild('functionalityModal') functionalityModal;
    @ViewChild('confirmModal') confirmModal;

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "Confirmación de baja",
      "confirmModal",
      true,
      true,
      "Aceptar",
      "Cancelar"
    );

    public functionalityModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "Asignar Funcionalidades", //title
      "functionalityModal", //id
      true,          //Accept Button
      true,          //Cancel Button
      "Aceptar",     //Accept Button Text
      "Cancelar");   //Cancel Button Text

    constructor(
        private service: ModuleService, 
        private activatedRoute: ActivatedRoute, 
        private router: Router,
        private messageService: MessageService,
        private functionalityService: FunctionalityService,
        private errorHandlerService: ErrorHandlerService) { 
    }

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getEntity(this.id);
        });
    }

    ngOnDestroy(){
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.editSubscrip) this.editSubscrip.unsubscribe();
        if(this.functSubscrip) this.functSubscrip.unsubscribe();
    }

    getEntity(id: number){
        this.getSubscrip = this.service.get(id).subscribe((data) => {
            this.module = data;
            this.getAllFunctionalities();
        });
    }

    onSubmit(form){
      if(!form.invalid){

        var json = {
          id: this.module.id,
          description: this.module.description,
          active: this.module.active,
          code: this.module.code
        }

        this.editSubscrip = this.service.edit(json).subscribe(
          data => {
            if(data.messages) this.messageService.showMessages(data.messages);
            this.router.navigate(["/admin/entities"])
          },
          err => {
            var json = JSON.parse(err._body)
            if(json.messages) this.messageService.showMessages(json.messages);
          }
        );
      }
    }

    getAllFunctionalities(){
        this.functSubscrip = this.functionalityService.getOptions().subscribe(data => {
            this.allFunctionalities = data;
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

    selectFunctionalities(){
      this.functsToAdd = null;
      var localFunct = new Array<Option>();
      var index = 0;

      for(var i: number = 0; i < this.allFunctionalities.length; i++){

          if(!this.isOptionInArray(this.module.functionalities, this.allFunctionalities[i]) ){
              this.allFunctionalities[i].included = false;
              this.allFunctionalities[i].index = index;
              localFunct.push(this.allFunctionalities[i]);
              index++;
          }
      }

      this.functsToAdd = localFunct;
    }

    addFunctionality(){
      this.selectFunctionalities();
      this.functionalityModal.show();
    }

    openConfirmModal(functionalityId){
        this.functionalityId = functionalityId;
        this.confirmModal.show();
    }

    unAssignFunctionality(functionalityId: number){
        var moduleIndex = this.module.functionalities.findIndex(m => m.id == functionalityId);

        this.confirmModal.hide();

        this.service.unAssignFunctionality(this.id, functionalityId).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getEntity(this.id);
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    assignFunctionalities(){

        var arrFunctToAdd = this.functsToAdd.filter((el)=> el.included).map((item) => {
            return item.value
        });

        this.service.assignFunctionalities(this.id, arrFunctToAdd).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getEntity(this.id);
                this.functionalityModal.hide();
            },
            err => this.errorHandlerService.handleErrors(err));
    }
}