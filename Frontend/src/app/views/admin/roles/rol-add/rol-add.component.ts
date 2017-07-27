import { Role } from './../../../../../models/role';
import { RoleService } from './../../../../services/role.service';
import { Component, OnInit } from '@angular/core';
import { MessageService } from './../../../../services/message.service';

@Component({
  selector: 'app-rol-add',
  templateUrl: './rol-add.component.html',
  styleUrls: ['./rol-add.component.css']
})
export class RolAddComponent implements OnInit {

  public rol: Role = <Role>{};

  constructor(private service: RoleService, private messageService: MessageService) { }

  ngOnInit() {
  }

  onSubmit(form){
    if(!form.invalid){
      this.rol.active = true;
      this.service.add(this.rol).subscribe(
        data => {
          console.log(data);
        },
        err => {
          var json = JSON.parse(err._body)
          if(json.messages) this.messageService.showMessages(json.messages);
        }
      );
    }
    
  }

}
