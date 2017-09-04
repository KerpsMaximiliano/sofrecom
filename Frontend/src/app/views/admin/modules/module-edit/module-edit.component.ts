import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/common/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Module } from 'models/Module';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Option } from 'models/option';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { FunctionalityService } from "app/services/admin/functionality.service";
import { ModuleService } from "app/services/admin/module.service";

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
    private activateSubscrip: Subscription;
    private deactivateSubscrip: Subscription;
    public functionalityId: number = 0;
    public allFunctionalities: any[] = new Array<any>();
    public functsToAdd: any[];
    public checkAtLeft:boolean = true;
    private functionalitySelected: any;
    public modalMessage: string = "";
 
    @ViewChild('confirmModal') confirmModal;

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "Confirmación de baja",
      "confirmModal",
      true,
      true,
      "Aceptar",
      "Cancelar"
    );

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
        if(this.activateSubscrip) this.activateSubscrip.unsubscribe();
        if(this.deactivateSubscrip) this.deactivateSubscrip.unsubscribe();
    }

    getEntity(id: number){
        this.getSubscrip = this.service.get(id).subscribe((data) => {
            this.module = data;
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

    deactivate(){
        this.confirmModal.hide();

        this.deactivateSubscrip = this.functionalityService.deactivate(this.functionalitySelected.id).subscribe(
            data => {
                this.functionalitySelected.active = !this.functionalitySelected.active;
                if(data.messages) this.messageService.showMessages(data.messages);
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    activate(){
        this.confirmModal.hide();

        this.activateSubscrip = this.functionalityService.activate(this.functionalitySelected.id).subscribe(
            data => {
                this.functionalitySelected.active = !this.functionalitySelected.active;
                if(data.messages) this.messageService.showMessages(data.messages);
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    habInhabClick(obj){
      this.functionalitySelected = obj;

      if (obj.active){
        this.confirm = this.deactivate;
        this.confirmModalConfig.acceptButtonText = "Deshabilitar";
        this.confirmModalConfig.title = "Confirmación de baja";
        this.modalMessage = "Está seguro de dar de baja " + obj.description + "?";
        this.confirmModal.show();
      } else {
        this.confirm = this.activate;
        this.confirmModalConfig.acceptButtonText = "Habilitar"
        this.confirmModalConfig.title = "Confirmación de alta";
        this.modalMessage = "Está seguro de dar de alta " + obj.description + "?";
        this.confirmModal.show();
      }
  }

  confirm() {}
}