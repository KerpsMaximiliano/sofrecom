import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";

@Component({
    selector: 'providers',
    templateUrl: './providers.html'
})

export class ProvidersComponent {

    states = [
        { id: 0, text: "Todos" },
        { id: 1, text: "Activo (Default)" },
        { id: 0, text: "Inactivo" },
    ];
    stateId: number;
    businessName: string;
    areas = [
        //Combo de ProvidersAreas con Active = true
        { id: 0, text: "Rubro 1" },
        { id: 1, text: "Rubro 2" },
        { id: 2, text: "Rubro 3" },
        { id: 3, text: "Rubro 4" },
    ];
    areaId: number;
    data = [
        {
            id: 1,
            businessName: "Test",
            area: "Test",
            cuit: "Test",
            income: "Test",
            iva: "Test"
        },
        {
            id: 2,
            businessName: "Test1",
            area: "Test1",
            cuit: "Test1",
            income: "Test1",
            iva: "Test1"
        },
        {
            id: 3,
            businessName: "Test2",
            area: "Test2",
            cuit: "Test2",
            income: "Test2",
            iva: "Test2"
        }
    ]

    constructor(
        private router: Router,
        private providersService: ProvidersService,
    ) {}

    view(id) {
        this.providersService.setMode("View");
        this.router.navigate([`providers/providers/edit/${id}`]);
    }

    edit(id) {
        this.providersService.setMode("Edit");
        this.router.navigate([`providers/providers/edit/${id}`])
    }

    refreshSearch() {
        this.stateId = null;
        this.businessName = null;
        this.areaId = null
    }

    search() {
        console.log(this.stateId);
        console.log(this.businessName);
        console.log(this.areaId)
    }
}