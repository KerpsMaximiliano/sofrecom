import { DatatablesLocationTexts } from './../../../components/datatables/datatables.location-texts';
import { RoleService } from './../../../services/role.service';
import { Role } from 'models/role';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { DatatablesEditionType } from "app/components/datatables/datatables.edition-type";
import { DatatablesColumn } from "app/components/datatables/datatables.columns";
import { Subscription } from "rxjs/Subscription";
import { DatatablesOptions } from "app/components/datatables/datatables.options";
import { DatatablesDataType } from "app/components/datatables/datatables.datatype";
import { DatatablesAlignment } from "app/components/datatables/datatables.alignment";

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit, OnDestroy {

  data;
  getAllSubscrip: Subscription;
  editionType = DatatablesEditionType.ButtonsAtTheEndOfTheRow;
  locationTexts = new DatatablesLocationTexts("Assign");

  options = new DatatablesOptions(
    true,  //edit
    true,  //delete
    false,  //view
    true,  //other
    false, //other2
    false, //other3
    "fa-compress",     //other1Icon
    "fa-check",      //other2Icon
    "fa-cogs"        //other3Icon
    ); 

  private dataTypeEnum = DatatablesDataType;
  private alignmentEnum = DatatablesAlignment;

  columns: DatatablesColumn[] = 
  [
    new DatatablesColumn(
      "id",  //name
      "Id",  //title
      "",    //width
      1,     //visibility
      this.dataTypeEnum.number,  //dataType
      this.alignmentEnum.left
    ),
    new DatatablesColumn(
      "description",  //name
      "Description",  //title
      "",    //width
      1,     //visibility
      this.dataTypeEnum.string,  //dataType
      this.alignmentEnum.left
    ),
    new DatatablesColumn(
      "active",  //name
      "Active",  //title
      "",    //width
      1,     //visibility
      this.dataTypeEnum.boolean,  //dataType
      this.alignmentEnum.center
    ),
    new DatatablesColumn(
      "startDate",  //name
      "Start Date",  //title
      "30px",    //width
      1,     //visibility
      this.dataTypeEnum.date,  //dataType
      this.alignmentEnum.right
    ),
    new DatatablesColumn(
      "endDate",  //name
      "End Date",  //title
      "30px",    //width
      1,     //visibility
      this.dataTypeEnum.date,  //dataType
      this.alignmentEnum.right
    ),
  ];

  constructor(
      private router: Router,
      private route: ActivatedRoute,
      private service: RoleService) { }

  ngOnInit() {
    this.getAll();
  }

  ngOnDestroy(){
    this.getAllSubscrip.unsubscribe();
  }

  getAll(){
    this.getAllSubscrip = this.service.getAll().subscribe(d => {
      //d[1].active = true;
      //d[1].startDate = new Date();
      this.data = d;
    });
  }

  editClick(id: number){
    console.log("Edit ID: " + id);
    this.router.navigate(['/admin/roles/edit/'+id]);
  }

  deleteClick(id: number){
    console.log("Delete ID: " + id);
    //this.router.navigate(['/admin/roles/delete/'+id]);
  }

  assginFeatures(id: number){
    console.log("Assign ID: " + id);
    //this.router.navigate(['/admin/roles/assign/'+id]);
  }

}
