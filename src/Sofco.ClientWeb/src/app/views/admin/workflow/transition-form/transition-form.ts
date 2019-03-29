import { Component, OnInit, OnDestroy, ViewChild, EventEmitter, Output, Input } from "@angular/core";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { Subscription } from "rxjs";
import { GroupService } from "app/services/admin/group.service";
import { UserService } from "app/services/admin/user.service";
import { UtilsService } from "app/services/common/utils.service";

@Component({
    selector: 'workflow-transition-form',
    templateUrl: './transition-form.html'
})
export class WorkflowTransitionFormComponent implements OnInit, OnDestroy {

    getStatesSubscrip: Subscription;
    getUsersSubscrip: Subscription;
    getGroupsSubscrip: Subscription;
    postSubscrip: Subscription;

    public states: any[] = new Array();
    public users: any[] = new Array();
    public groups: any[] = new Array();

    public model: any = {
        actualWorkflowStateId: null,
        nextWorkflowStateId: null,
        notificationCode: "",
        conditionCode: "",
        validationCode: "",
        parameterCode: "",
        userApplicantHasAccess: false,
        managerHasAccess: false,
        usersHasAccess: [],
        usersDenyAccess: [],
        groupsHasAccess: [],
        sectorHasAccess: false,
        notifyToUserApplicant: false,
        notifyToManager: false,
        notifyToUsers: [],
        notifyToGroups: [],
        notifyToSector: false,
        id: null
    }

    constructor(private groupService: GroupService,
                private userService: UserService,
                private utilsService: UtilsService,
                private workflowService: WorkflowService){
    }

    ngOnInit(): void {
        this.getUsers();
        this.getGroups();
        this.getStates();
    }    
    
    ngOnDestroy(): void {
        if(this.getStatesSubscrip) this.getStatesSubscrip.unsubscribe();
        if(this.getUsersSubscrip) this.getUsersSubscrip.unsubscribe();
        if(this.getGroupsSubscrip) this.getGroupsSubscrip.unsubscribe();
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    getUsers() {
        this.getUsersSubscrip = this.userService.getOptions().subscribe(res => {
            this.users = res.map(item => {
                item.id = parseInt(item.id);
                return item;
            });
        });
    }

    getGroups() {
        this.getUsersSubscrip = this.groupService.getOptions().subscribe(res => {
            this.groups = res;
        });
    }

    getStates() {
        this.getUsersSubscrip = this.workflowService.getStates().subscribe(res => {
            this.states = res.data;
        });
    }
}