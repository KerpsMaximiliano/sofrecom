import { ActivatedRoute, Router } from '@angular/router';
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

  id: string;
  public rol: Role = <Role>{};

  constructor(private service: RoleService, private activatedRoute: ActivatedRoute, private router: Router) { 
    
  }

  ngOnInit() {
    this.activatedRoute.params.subscribe(params => {
        this.id = params['id'];
        this.getRol(this.id);
    });
  }

  getRol(id: string){
    this.service.get(id).subscribe((data) => {
      this.rol = data.data;
    });
  }

  onSubmit(form){
    if(!form.invalid){
      this.service.edit(this.rol).subscribe(
        data => {
          console.log(data);
          this.router.navigate(["/admin/roles"])
        },
        err => {
          console.log(err);
        }
      );
    }
    
  }

  onActiveClick(active){
    if(typeof active == 'boolean'){
      this.rol.active = active;
    }
  }

}
