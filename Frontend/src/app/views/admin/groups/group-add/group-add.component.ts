import { Router } from '@angular/router';
import { Group } from 'models/group';
import { Component, OnInit } from '@angular/core';
import { MessageService } from 'app/services/common/message.service';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { GroupService } from "app/services/admin/group.service";

@Component({
  selector: 'app-group-add',
  templateUrl: './group-add.component.html',
  styleUrls: ['./group-add.component.css']
})
export class GroupAddComponent implements OnInit {

  public group: Group = <Group>{};

  constructor(private service: GroupService, 
    private messageService: MessageService,
    private router: Router,
    private errorHandlerService: ErrorHandlerService) { }

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
        err => this.errorHandlerService.handleErrors(err));
    }
  }
}
