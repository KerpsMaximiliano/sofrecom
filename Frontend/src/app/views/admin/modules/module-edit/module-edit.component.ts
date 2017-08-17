import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ModuleService } from 'app/services/module.service';
import { Module } from 'models/Module';

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

    constructor(
        private service: ModuleService, 
        private activatedRoute: ActivatedRoute, 
        private router: Router,
        private messageService: MessageService) { 
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
}