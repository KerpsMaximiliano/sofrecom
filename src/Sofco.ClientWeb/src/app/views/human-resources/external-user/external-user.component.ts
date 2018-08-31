import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { UserService } from "../../../services/admin/user.service";
import { Subscription } from "rxjs";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { MessageService } from "../../../services/common/message.service";

@Component({
    selector: 'external-user',
    templateUrl: './external-user.component.html'
})
export class ExternalUserComponent implements OnInit, OnDestroy {

    getUsersSubscript: Subscription;
    getManagersSubscript: Subscription;
    addSubscript: Subscription;

    public users: any[] = new Array();
    public managers: any[] = new Array();

    @ViewChild('externalModal') externalModal;
    public externalModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "allocationManagement.resources.hoursByContract",
        "externalModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    ); 

    public model = {
        userId: 0,
        managerId: 0,
        countryCode: 0,
        areaCode: 0,
        hours: 0,
        phone: ""
    }

    constructor(private userService: UserService,
                private messageService: MessageService,
                private employeeService: EmployeeService) {}

    ngOnInit(): void {
        this.getUsers();
        this.getManagers();
    }    
    
    ngOnDestroy(): void {
        if(this.getUsersSubscript) this.getUsersSubscript.unsubscribe();
        if(this.getManagersSubscript) this.getManagersSubscript.unsubscribe();
        if(this.addSubscript) this.addSubscript.unsubscribe();
    }

    getUsers(){
        this.getUsersSubscript = this.userService.getUsersExternalFreeOptions().subscribe(response => {
            this.users = response; 
        });
    }

    getManagers(){
        this.getManagersSubscript = this.userService.getOptions().subscribe(response => {
            this.managers = response; 
        });
    }

    add(){
        this.getUsersSubscript = this.employeeService.addExternal(this.model).subscribe(response => {
            this.externalModal.hide();
        },
        error => this.externalModal.resetButtons());
    }
}