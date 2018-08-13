import { Option } from '../../../../models/option';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { MessageService } from '../../../../services/common/message.service';
import { RoleService } from "../../../../services/admin/role.service";
import { Role } from "../../../../models/admin/role";

@Component({
  selector: 'app-rol-add',
  templateUrl: './rol-add.component.html',
  styleUrls: ['./rol-add.component.css']
})
export class RolAddComponent implements OnInit {

    public rol: Role = <Role>{};

    public funcsToAdd: Option[];
    public funcsAdded: Option[];

    constructor(private service: RoleService, 
                private messageService: MessageService,private router: Router) { }

    ngOnInit() { }

    onSubmit(form){
      if(!form.invalid){
        this.messageService.showLoading();

        this.rol.active = true;
        this.service.add(this.rol).subscribe(
          data => {
            this.messageService.closeLoading();

            this.router.navigate(["/admin/roles"]);
          });
      }
    }
}
