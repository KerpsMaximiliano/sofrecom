import { Subscription } from 'rxjs';
import { MessageService } from '../../../../services/common/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { GroupService } from "../../../../services/admin/group.service";
import { RoleService } from "../../../../services/admin/role.service";
import { Group } from "../../../../models/admin/group";
import { Role } from "../../../../models/admin/role";

@Component({
  selector: 'app-group-edit',
  templateUrl: './group-edit.component.html',
  styleUrls: ['./group-edit.component.css']
})
export class GroupEditComponent implements OnInit, OnDestroy {

  public module: Group = new Group();

  private id: number;
  
  private paramsSubscrip: Subscription;
  private getSubscrip: Subscription;
  private editSubscrip: Subscription;
  private getRolesSubscrip: Subscription;

  public roles: any[] = new Array();

  constructor(
    private service: GroupService, 
    private roleService: RoleService,
    private activatedRoute: ActivatedRoute, 
    private router: Router,
    private messageService: MessageService) { 
  }

  ngOnInit() {
    this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.id = params['id'];
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
      });
  }

  getAllRoles(){
    this.messageService.showLoading();

    this.getRolesSubscrip = this.roleService.getOptions().subscribe(
      d => {
        this.roles = d;

        this.getEntity(this.id);

        this.messageService.closeLoading();
      },
      err => this.messageService.closeLoading());
  }

  goToGroups(){
    this.router.navigate(['/admin/groups']);
  }

  onSubmit(form){
    this.editSubscrip = this.service.edit(this.module).subscribe(
      data => {
        this.router.navigate(["/admin/groups"])
      });
  }
}
