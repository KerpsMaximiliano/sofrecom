import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";

@Component({
    selector: 'providers',
    templateUrl: './providers.html'
})

export class ProvidersComponent implements OnInit{

    states = [
        { id: 0, text: "Todos" },
        { id: 1, text: "Activo" },
        { id: 2, text: "Inactivo" },
    ];
    stateId: number = 1;
    businessName: string;
    areas = [];
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
        private providersArea: ProvidersAreaService
    ) {}

    ngOnInit(): void {
        this.providersArea.getAll().subscribe(d => {
            d.data.forEach(providerArea => {
                if(providerArea.active) {
                    this.areas.push(providerArea);
                    this.areas = [...this.areas]
                }
            })
        });
    }

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