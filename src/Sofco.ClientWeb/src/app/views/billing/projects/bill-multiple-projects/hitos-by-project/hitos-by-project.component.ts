import { Component, OnInit, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { Subscription } from "rxjs";
import { ProjectService } from "../../../../../services/billing/project.service";

@Component({
  selector: 'hitos-by-project',
  templateUrl: './hitos-by-project.component.html'
})
export class HitosByProjectComponent implements OnInit, OnDestroy {

    hitos: any[] = new Array();
    hitosSelected: any[] = new Array();
    id: string;
    loading: boolean = false;

    @Input() projectId: string;
    @Input() index: number;
    @Output() selectHitoEvent: EventEmitter<any> = new EventEmitter();

    getAllSubscrip: Subscription;

    constructor(
        private projectService: ProjectService) { }

    ngOnInit() {
        this.id = "hito" + this.index;
        this.loading = true;

        this.getAllSubscrip = this.projectService.getHitos(this.projectId).subscribe(d => {
            this.hitos = d.filter(item => {
                item.projectId = this.projectId;
                item.selected = false;

                if(item.statusCode == "1" && item.billed == false){
                    return item;
                }

                return null;
            });

            this.loading = false;
        },
        () => {
                this.loading = false;
            });        
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    getCurrencySymbol(currency){
      switch(currency){
        case "Peso": { return "$"; }
        case "Dolar": { return "U$D"; }
        case "Euro": { return "€"; }
      }
    }

    selectHito(hito){
        hito.selected = !hito.selected;
    
        if(hito.selected == false){
            var index = this.hitosSelected.map(e => e.id).indexOf(hito.id);
            this.hitosSelected.splice(index, 1);
        }
        else{
            this.hitosSelected.push(hito);
        }

        var json = {
            id: this.id,
            hitos: this.hitosSelected
        }

        this.selectHitoEvent.emit(json);
    }
}
