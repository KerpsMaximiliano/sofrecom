import { Option } from 'app/models/option';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { MessageService } from 'app/services/common/message.service';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { RoleService } from "app/services/admin/role.service";
import { Role } from "app/models/admin/role";

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
                private messageService: MessageService,private router: Router,
                private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() { }

    onSubmit(form){
      if(!form.invalid){
        this.messageService.showLoading();

        this.rol.active = true;
        this.service.add(this.rol).subscribe(
          data => {
            this.messageService.closeLoading();
            if(data.messages) this.messageService.showMessages(data.messages);

            this.router.navigate(["/admin/roles"]);
          },
          err => this.errorHandlerService.handleErrors(err));
      }
    }
}
