import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/common/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { GroupService } from "app/services/admin/group.service";
import { RoleService } from "app/services/admin/role.service";
import { Group } from "app/models/admin/group";
import { Role } from "app/models/admin/role";
declare var $: any;

@Component({
  selector: 'app-group-edit',
  templateUrl: './group-edit.component.html',
  styleUrls: ['./group-edit.component.css']
})
export class GroupEditComponent implements OnInit, OnDestroy {

  public module: Group;

  private id: number;
  
  private paramsSubscrip: Subscription;
  private getSubscrip: Subscription;
  private editSubscrip: Subscription;
  private getRolesSubscrip: Subscription;

  public roles;

  constructor(
    private service: GroupService, 
    private roleService: RoleService,
    private activatedRoute: ActivatedRoute, 
    private router: Router,
    private messageService: MessageService,
    private errorHandlerService: ErrorHandlerService) { 
  }

  ngOnInit() {
    this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.id = params['id'];
        this.getEntity(this.id);
        this.getAllRoles();
    });
  }

  ngOnDestroy(){
    if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    if(this.getSubscrip) this.getSubscrip.unsubscribe();
    if(this.editSubscrip) this.editSubscrip.unsubscribe();
    if(this.getRolesSubscrip) this.getRolesSubscrip.unsubscribe();
  }

  getEntity(id: number){
    this.getSubscrip = this.service.get(id).subscribe(
      data => {
        this.module = data;

        if(!data.role){
          this.module.role = <Role>{};
        }
      },
      err => this.errorHandlerService.handleErrors(err));
  }

  getAllRoles(){
    this.getRolesSubscrip = this.roleService.getOptions().subscribe(
      d => {
        this.roles = d;
      },
      err => this.errorHandlerService.handleErrors(err));
  }

  onSubmit(form){
    if(!form.invalid){

      var json = {
        id: this.module.id,
        description: this.module.description,
        active: this.module.active,
        role: {
          id: this.module.role.id,
          description: "TODO: borrar"
        }
      }

      this.editSubscrip = this.service.edit(json).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);
          this.router.navigate(["/admin/groups"])
        },
        err => this.errorHandlerService.handleErrors(err));
    }
  }
}
