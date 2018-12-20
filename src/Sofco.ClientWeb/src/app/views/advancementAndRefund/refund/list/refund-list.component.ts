import { Component, OnInit } from "@angular/core";

declare var $: any;

@Component({
    selector: 'refund-list',
    templateUrl: './refund-list.component.html',
    styleUrls: ['./refund-list.component.scss']
})
export class RefundListComponent implements OnInit {

    public tabInProcess = true;

    constructor(){}

    ngOnInit(): void {
    }
}
