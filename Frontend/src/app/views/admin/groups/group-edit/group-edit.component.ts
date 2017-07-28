import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { GroupService } from 'app/services/group.service';
import { Group } from 'models/Group';
import { Component, OnInit, OnDestroy } from '@angular/core';
declare var $: any;

@Component({
  selector: 'app-group-edit',
  templateUrl: './group-edit.component.html',
  styleUrls: ['./group-edit.component.css']
})
export class GroupEditComponent implements OnInit, OnDestroy {

  public entity: Group = <Group>{};

  private id: number;
  
  private paramsSubscrip: Subscription;
  private getSubscrip: Subscription;
  private editSubscrip: Subscription;

  constructor(
    private service: GroupService, 
    private activatedRoute: ActivatedRoute, 
    private router: Router,
    private messageService: MessageService) { 
    
  }

  ngOnInit() {
    this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.id = params['id'];
        this.getEntity(this.id);
    });
  }

  ngOnDestroy(){
    if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    if(this.getSubscrip) this.getSubscrip.unsubscribe();
    if(this.editSubscrip) this.editSubscrip.unsubscribe();
  }

  getEntity(id: number){
    this.getSubscrip = this.service.get(id).subscribe((data) => {
      this.entity = data;
    });
  }

  onSubmit(form){
    if(!form.invalid){
      this.editSubscrip = this.service.edit(this.entity).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);
          this.router.navigate(["/admin/groups"])
        },
        err => {
          var json = JSON.parse(err._body)
          if(json.messages) this.messageService.showMessages(json.messages);
        }
      );
    }
    
  }

  onActiveClick(active){
    if(typeof active == 'boolean'){
      this.entity.active = active;
    }
  }

}
