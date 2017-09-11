import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { MessageService } from 'app/services/common/message.service';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { GroupService } from "app/services/admin/group.service";
import { Subscription } from "rxjs/Subscription";
import { RoleService } from "app/services/admin/role.service";
import { Group } from "app/models/admin/group";

@Component({
  selector: 'app-group-add',
  templateUrl: './group-add.component.html',
  styleUrls: ['./group-add.component.css']
})
export class GroupAddComponent implements OnInit, OnDestroy {

  public group: Group = <Group>{};
  private getRolesSubscrip: Subscription;
  public roles;

  constructor(private service: GroupService, 
    private messageService: MessageService,
    private roleService: RoleService,
    private router: Router,
    private errorHandlerService: ErrorHandlerService) { }

  ngOnInit() {
    this.getAllRoles();
  }

  onSubmit(form){
    if(!form.invalid){
      this.group.active = true;
      this.service.add(this.group).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);
          this.router.navigate(["/admin/groups"]);
        },
        err => this.errorHandlerService.handleErrors(err));
    }
  }

  ngOnDestroy(){
    if(this.getRolesSubscrip) this.getRolesSubscrip.unsubscribe();
  }

  getAllRoles(){
    this.getRolesSubscrip = this.roleService.getOptions().subscribe(d => {
      this.roles = d;
    });
  }
}
