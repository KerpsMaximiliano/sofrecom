import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { MessageService } from '../../../../services/common/message.service';
import { GroupService } from "../../../../services/admin/group.service";
import { Subscription } from "rxjs";
import { RoleService } from "../../../../services/admin/role.service";
import { Group } from "../../../../models/admin/group";

@Component({
  selector: 'app-group-add',
  templateUrl: './group-add.component.html',
  styleUrls: ['./group-add.component.css']
})
export class GroupAddComponent implements OnInit, OnDestroy {

  public group: Group = new Group();
  private getRolesSubscrip: Subscription;
  public roles: any[] = new Array();

  constructor(private service: GroupService, 
    private messageService: MessageService,
    private roleService: RoleService,
    private router: Router) { }

  ngOnInit() {
    this.getAllRoles();
  }

  onSubmit(){
    this.messageService.showLoading();

    this.group.active = true;
    this.service.add(this.group).subscribe(
      data => {
        this.messageService.closeLoading();
        
        this.router.navigate(["/admin/groups"]);
      },
      err => this.messageService.closeLoading());
  }

  ngOnDestroy(){
    if(this.getRolesSubscrip) this.getRolesSubscrip.unsubscribe();
  }

  getAllRoles(){
    this.getRolesSubscrip = this.roleService.getOptions().subscribe(
      d => {
        this.roles = d;
      });
  }

  goToGroups(){
    this.router.navigate(['/admin/groups']);
  }
}
