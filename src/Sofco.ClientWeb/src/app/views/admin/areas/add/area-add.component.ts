import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { AreaService } from "../../../../services/admin/area.service";
import { UserService } from "../../../../services/admin/user.service";

@Component({
    selector: 'app-area-add',
    templateUrl: './area-add.component.html'
})
export class AreaAddComponent implements OnInit, OnDestroy {

    public text: string;
    public responsableId: number;

    public responsables: any[] = new Array();

    public addSubscript: Subscription;
    public responsablesSubscript: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private areaService: AreaService,
                private userService: UserService) { }

    ngOnInit(): void {
        this.messageService.showLoading();

        this.responsablesSubscript = this.userService.getOptions().subscribe(response => {
            this.messageService.closeLoading();
            this.responsables = response; 
        }, 
        error => {
            this.messageService.closeLoading();
        });
    }

    ngOnDestroy(): void {
        if(this.addSubscript) this.addSubscript.unsubscribe();
        if(this.responsablesSubscript) this.responsablesSubscript.unsubscribe();
    }

    goBack(){
        this.router.navigate(["/admin/areas"]);  
    }

    save(){
        this.messageService.showLoading();

        this.addSubscript = this.areaService.add(this.text, this.responsableId).subscribe(response => {
            this.messageService.closeLoading();
            this.goBack();   
        }, 
        error => { 
            this.messageService.closeLoading();
        });
    }
}