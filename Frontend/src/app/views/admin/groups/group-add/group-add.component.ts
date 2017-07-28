import { Router } from '@angular/router';
import { Group } from 'models/group';
import { GroupService } from 'app/services/group.service';
import { Component, OnInit } from '@angular/core';
import { MessageService } from './../../../../services/message.service';

@Component({
  selector: 'app-group-add',
  templateUrl: './group-add.component.html',
  styleUrls: ['./group-add.component.css']
})
export class GroupAddComponent implements OnInit {

  public group: Group = <Group>{};

  constructor(private service: GroupService, private messageService: MessageService,private router: Router) { }

  ngOnInit() {
  }

  onSubmit(form){
    if(!form.invalid){
      this.group.active = true;
      this.service.add(this.group).subscribe(
        data => {
          console.log(data);
          if(data.messages) this.messageService.showMessages(data.messages);
          this.router.navigate(["/admin/groups"]);
        },
        err => {
          var json = JSON.parse(err._body)
          if(json.messages) this.messageService.showMessages(json.messages);
        }
      );
    }
  }
}
