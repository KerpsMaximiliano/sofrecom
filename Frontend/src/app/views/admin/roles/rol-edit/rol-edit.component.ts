import { RoleService } from 'app/services/role.service';
import { Role } from 'models/role';
import { Component, OnInit } from '@angular/core';
declare var $: any;

@Component({
  selector: 'app-rol-edit',
  templateUrl: './rol-edit.component.html',
  styleUrls: ['./rol-edit.component.css']
})
export class RolEditComponent implements OnInit {

  public rol: Role = <Role>{};

  constructor(private service: RoleService) { 

  }

  ngOnInit() {
  }

  onSubmit(form){
    if(!form.invalid){
      this.service.edit(this.rol).subscribe(
        data => {
          console.log(data);
        },
        err => {
          console.log(err);
        }
      );
    }
    
  }

  onActiveClick(active){
    this.rol.active = active;
  }

}
