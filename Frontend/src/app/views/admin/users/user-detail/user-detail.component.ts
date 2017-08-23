import { MessageService } from 'app/services/message.service';
import { Option } from 'models/option';
import { Group } from 'models/group';
import { GroupService } from 'app/services/group.service';
import { Subscription } from 'rxjs/Subscription';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { User } from 'models/user';
import { UserService } from 'app/services/user.service';
import { UserDetail } from "models/userDetail";

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit, OnDestroy {
    private id;
    public user = <UserDetail>{};
    private groupId: any;

    public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Asignar Grupos", //title
        "modalGroups", //id
        true,          //Accept Button
        false,          //Cancel Button
        "Aceptar",     //Accept Button Text
        "Cancelar");   //Cancel Button Text

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "Confirmación de baja",
      "confirmModal",
      true,
      true,
      "Aceptar",
      "Cancelar"
    );

    private routeSubscrip: Subscription;
    private detailsSubscrip: Subscription;

    //GroupService
    public checkAtLeft:boolean = true;
    public groupsToAdd: Option[] = new Array<Option>();
    public groupsToAddSubscrip: Subscription;
    @ViewChild('modalGroups') modalGroups;
    @ViewChild('confirmModal') confirmModal;
    
    constructor(
        private service: UserService, 
        private activatedRoute: ActivatedRoute, 
        private router: Router,
        private groupService: GroupService,
        private messageService: MessageService) { }

    ngOnInit() {
        this.routeSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getDetails();
        });
    }
 
    getDetails(){
        this.detailsSubscrip = this.service.getDetail(this.id).subscribe(user => {
            this.user = user;
            this.getAllGroups();
        });
    }

    ngOnDestroy(){
        if(this.routeSubscrip) this.routeSubscrip.unsubscribe();
        if(this.detailsSubscrip) this.detailsSubscrip.unsubscribe();
        if(this.groupsToAddSubscrip) this.groupsToAddSubscrip.unsubscribe();
    }

    getAllGroups(){
        this.groupsToAddSubscrip = this.groupService.getOptions().subscribe(data => {
            this.groupsToAdd = new Array<Option>();
            var groups = new Array<Option>();
            var index = 0;
            for(var i: number = 0; i<data.length; i++){

                if(!this.isOptionInArray(this.user.groups, data[i]) ){
                    data[i].included = false;
                    data[i].index = index;
                    groups.push(data[i]);
                    index++;
                }
                
            }

            this.groupsToAdd = groups;
        });
    }

    private isOptionInArray(arrGroup, option: Option): boolean{
        var esta: boolean = false;

        for(var i: number = 0; i<arrGroup.length; i++){
            if(arrGroup[i].value.toString() == option.value ){
                esta = true;
                break;
            }
        }

        return esta;
    }

    assignGroups(){

        var arrGroupsToAdd = this.groupsToAdd.filter((el)=> el.included).map((item) => {
            return item.value
        });

        var objToSend = {
            groupsToAdd: arrGroupsToAdd,
            groupsToRemove: []
        };

        this.service.assignGroups(this.id, objToSend).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
                this.modalGroups.hide();
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

    unassignGroup(){
        this.service.unassignGroup(this.user.id, this.groupId).subscribe(
            data => {
                this.confirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.getDetails();
            },
            err => {
                this.confirmModal.hide();
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

    openConfirmModal(groupId){
        this.groupId = groupId;
        this.confirmModal.show();
    }
}
