import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Functionality } from 'models/functionality';
import { FunctionalityService } from 'app/services/functionality.service';
import { Option } from 'models/option';
import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RoleService } from 'app/services/role.service';
import { Role } from 'models/role';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
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
  public  funcsToAddSubscrip: Subscription;

  //FuncsService
  public checkAtLeft:boolean = true;
  public funcsToAdd: Option[];
  @ViewChild('modalFuncs') modalFuncs;
  public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
    "Asignar Funcionalidades", //title
    "modalGroups", //id
    true,          //Accept Button
    false,          //Cancel Button
    "Aceptar",     //Accept Button Text
    "Cancelar");   //Cancel Button Text

  constructor(
    private service: RoleService, 
    private funcService: FunctionalityService,
    private activatedRoute: ActivatedRoute, 
    private router: Router,
    private messageService: MessageService) { 
    
  }

  ngOnInit() {
    this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.id = params['id'];
        this.getDetails();
    });
  }

  ngOnDestroy(){
    if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    if(this.getSubscrip) this.getSubscrip.unsubscribe();
    if(this.editSubscrip) this.editSubscrip.unsubscribe();
    if(this.funcsToAddSubscrip) this.funcsToAddSubscrip.unsubscribe();
  }

  getDetails(){
    this.getSubscrip = this.service.getDetail(this.id).subscribe((data) => {
      this.entity = data;

      this.getAllFuncs();
    });
  }

  onSubmit(form){
    if(!form.invalid){
      var model = {
        id: this.entity.id,
        description: this.entity.description,
        active: this.entity.active
      }

      this.editSubscrip = this.service.edit(model).subscribe(
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

  /*onActiveClick(active){
    if(typeof active == 'boolean'){
      this.entity.active = active;
    }
  }*/

  
    getAllFuncs(){
        this.funcsToAddSubscrip = this.funcService.getOptions().subscribe(data => {
            this.funcsToAdd = null;
            var funcs = new Array<Option>();
            var index = 0;
            for(var i: number = 0; i<data.length; i++){

                if(!this.isOptionInArray(this.entity.functionalities, data[i]) ){
                    data[i].included = false;
                    data[i].index = index;
                    funcs.push(data[i]);
                    index++;
                }
                
            }

            this.funcsToAdd = funcs;
        });
    }

    private isOptionInArray(arrFunc: Functionality[], option: Option): boolean{
        var esta: boolean = false;

        for(var i: number = 0; i<arrFunc.length; i++){
            if(arrFunc[i].id.toString() == option.value ){
                esta = true;
                break;
            }
        }

        return esta;
    }

    assignFuncs(){

        var arrFuncsToAdd = this.funcsToAdd.filter((el)=> el.included).map((item) => {
            return item.value
        });

        var objToSend = {
            FunctionalitiesToAdd: arrFuncsToAdd,
            functionalitiesToRemove: []
        };

        this.service.assignFunctionalities(this.id, objToSend).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
                this.modalFuncs.hide();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

    unassignFunc(funcId: number){
        this.service.unassignFunctionality(this.entity.id, funcId).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }


}
