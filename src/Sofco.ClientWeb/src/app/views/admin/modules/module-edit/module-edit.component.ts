import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/common/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Option } from 'app/models/option';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { FunctionalityService } from "app/services/admin/functionality.service";
import { ModuleService } from "app/services/admin/module.service";
import { Module } from "app/models/admin/module";
import { I18nService } from 'app/services/common/i18n.service';

@Component({
  selector: 'app-module-edit',
  templateUrl: './module-edit.component.html'
})
export class ModuleEditComponent implements OnInit, OnDestroy {

    public module: Module;
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
      "ACTIONS.confirmDelete",
      "confirmModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
    );

    constructor(
        private service: ModuleService, 
        private activatedRoute: ActivatedRoute, 
        private router: Router,
        private messageService: MessageService,
        private functionalityService: FunctionalityService,
        private errorHandlerService: ErrorHandlerService,
        private i18nService: I18nService) { 
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
        this.getSubscrip = this.service.get(id).subscribe(
          data => {
            this.module = data;
          },
          err => this.errorHandlerService.handleErrors(err));      
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
        this.confirmModalConfig.acceptButtonText = "ACTIONS.disable";
        this.confirmModalConfig.title = "ACTIONS.confirmDelete";
        this.modalMessage = this.i18nService.translateByKey("ACTIONS.areYouSureConfirmDelete") + obj.description + "?";
        this.confirmModal.show();
      } else {
        this.confirm = this.activate;
        this.confirmModalConfig.acceptButtonText = "ACTIONS.enable"
        this.confirmModalConfig.title = "ACTIONS.confirmAdd";
        this.modalMessage = this.i18nService.translateByKey("ACTIONS.areYouSureConfirmAdd") + obj.description + "?";
        this.confirmModal.show();
      }
  }

  confirm() {}
}