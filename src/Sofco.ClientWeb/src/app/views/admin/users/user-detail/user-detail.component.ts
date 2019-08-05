import { Subscription } from 'rxjs';
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { GroupService } from "../../../../services/admin/group.service";
import { UserService } from "../../../../services/admin/user.service";
import { UserDetail } from "../../../../models/admin/userDetail";
import { MenuService } from '../../../../services/admin/menu.service';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit, OnDestroy {
    private id;
    public user = <UserDetail>{};
    private groupId: any;

    @ViewChild('modalGroups') modalGroups;
    public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ADMIN.USERS.addGroup", //title
        "modalGroups", //id
        true,          //Accept Button
        true,          //Cancel Button
        "ACTIONS.ACCEPT",     //Accept Button Text
        "ACTIONS.cancel");   //Cancel Button Text

    @ViewChild('confirmModal') confirmModal;
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
    public groupsToAdd: any[] = new Array();
    public groupsToAddSubscrip: Subscription;
    
    constructor(
        private service: UserService, 
        private activatedRoute: ActivatedRoute, 
        private menuService: MenuService,
        private groupService: GroupService) { }

    ngOnInit() {
        this.routeSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getDetails();
        });
    }

    canAddGroup(){
        return this.menuService.hasFunctionality("USR", "ADGRP");
    }
 
    getDetails(){
        this.detailsSubscrip = this.service.getDetail(this.id).subscribe(
            user => {
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
        this.groupsToAddSubscrip = this.groupService.getOptions().subscribe(
            data => {
                this.groupsToAdd = new Array();
                var groups = new Array();
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

    private isOptionInArray(arrGroup, option: any): boolean{
        var esta: boolean = false;

        for(var i: number = 0; i<arrGroup.length; i++){
            if(arrGroup[i].id == option.id ){
                esta = true;
                break;
            }
        }

        return esta;
    }

    assignGroups(){

        var arrGroupsToAdd = this.groupsToAdd.filter((el)=> el.included).map((item) => {
            return item.id
        });

        var objToSend = {
            groupsToAdd: arrGroupsToAdd,
            groupsToRemove: []
        };

        this.service.assignGroups(this.id, objToSend).subscribe(
            () => {
                this.modalGroups.hide();
                this.getDetails();
            });
    }

    unassignGroup(){
        this.service.unassignGroup(this.user.id, this.groupId).subscribe(
            () => {
                this.confirmModal.hide();
                this.getDetails();
            },
            () => this.confirmModal.hide());
    }

    openConfirmModal(groupId){
        this.groupId = groupId;
        this.confirmModal.show();
    }
}
