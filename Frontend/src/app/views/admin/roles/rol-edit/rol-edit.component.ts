import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RoleService } from 'app/services/role.service';
import { Role } from 'models/role';
import { Component, OnInit, OnDestroy } from '@angular/core';
declare var $: any;

@Component({
  selector: 'app-rol-edit',
  templateUrl: './rol-edit.component.html',
  styleUrls: ['./rol-edit.component.css']
})
export class RolEditComponent implements OnInit, OnDestroy {

  public entity: Role = <Role>{};

  private id: number;
  
  private paramsSubscrip: Subscription;
  private getSubscrip: Subscription;
  private editSubscrip: Subscription;

  constructor(
    private service: RoleService, 
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
      this.entity = data;
    });
  }

  onSubmit(form){
    if(!form.invalid){
      this.editSubscrip = this.service.edit(this.entity).subscribe(
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

  onActiveClick(active){
    if(typeof active == 'boolean'){
      this.entity.active = active;
    }
  }

}
