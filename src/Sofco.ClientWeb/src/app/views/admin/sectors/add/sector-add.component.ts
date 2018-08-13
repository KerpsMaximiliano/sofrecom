import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { UserService } from "../../../../services/admin/user.service";
import { SectorService } from "../../../../services/admin/sector.service";

@Component({
    selector: 'app-sector-add',
    templateUrl: './sector-add.component.html'
})
export class SectorAddComponent implements OnInit, OnDestroy {

    public text: string;
    public responsableId: number = 0;

    public responsables: any[] = new Array();

    public addSubscript: Subscription;
    public responsablesSubscript: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private sectorService: SectorService,
                private userService: UserService) { }

    ngOnInit(): void {
        this.messageService.showLoading();

        this.responsablesSubscript = this.userService.getOptions().subscribe(response => {
            this.messageService.closeLoading();
            this.responsables = response; 
        }, 
        error => this.messageService.closeLoading());
    }

    ngOnDestroy(): void {
        if(this.addSubscript) this.addSubscript.unsubscribe();
        if(this.responsablesSubscript) this.responsablesSubscript.unsubscribe();
    }

    goBack(){
        this.router.navigate(["/admin/sectors"]);  
    }

    save(){
        this.messageService.showLoading();

        this.addSubscript = this.sectorService.add(this.text, this.responsableId).subscribe(response => {
            this.messageService.closeLoading();

            this.goBack();   
        }, 
        error => this.messageService.closeLoading());
    }
}