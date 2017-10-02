import { MessageService } from 'app/services/common/message.service';
import { Option } from 'app/models/option';
import { Subscription } from 'rxjs/Subscription';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { GroupService } from "app/services/admin/group.service";
import { UserService } from "app/services/admin/user.service";
import { UserDetail } from "app/models/admin/userDetail";

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
        "ADMIN.USERS.addGroup", //title
        "modalGroups", //id
        true,          //Accept Button
        false,          //Cancel Button
        "ACTIONS.ACCEPT",     //Accept Button Text
        "ACTIONS.cancel");   //Cancel Button Text

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmDelete",
      "confirmModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
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
        private messageService: MessageService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
        this.routeSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getDetails();
        });
    }
 
    getDetails(){
        this.detailsSubscrip = this.service.getDetail(this.id).subscribe(
            user => {
                this.user = user;
                this.getAllGroups();
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    ngOnDestroy(){
        if(this.routeSubscrip) this.routeSubscrip.unsubscribe();
        if(this.detailsSubscrip) this.detailsSubscrip.unsubscribe();
        if(this.groupsToAddSubscrip) this.groupsToAddSubscrip.unsubscribe();
    }

    getAllGroups(){
        this.groupsToAddSubscrip = this.groupService.getOptions().subscribe(
            data => {
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
            },
            err => this.errorHandlerService.handleErrors(err));
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
            err => this.errorHandlerService.handleErrors(err));
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
                this.errorHandlerService.handleErrors(err);
            }
        );
    }

    openConfirmModal(groupId){
        this.groupId = groupId;
        this.confirmModal.show();
    }
}
