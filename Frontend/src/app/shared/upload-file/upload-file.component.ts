import { Configuration } from 'app/services/configuration';
import { FileProperties } from './FileProperties';
import { RequestOptions } from '@angular/http';
import { Observable } from 'rxjs';
import { Component, OnInit, Input } from '@angular/core';
import { Http, Response, Headers, URLSearchParams } from '@angular/http';
import 'rxjs/add/operator/map';
declare var require: any;

@Component({
  selector: 'upload-file',
  templateUrl: './upload-file.component.html',
  styleUrls: ['./upload-file.component.scss']
})
export class UploadFileComponent implements OnInit {

  //@Input() url = "http://localhost:59605/api/UploadFile";
  //@Input() url = "http://localhost:9696/api/UploadFile";
  @Input() url = "http://sofrelab-iis1.cloudapp.net:9696/api/UploadFile"; //uat iis
  //@Input() url = "http://sofrelab-iis1.cloudapp.net:9000/api/UploadFile"; //uat
  
  @Input() authorization: string = '29c05029-ddf4-4e5e-8a91-62c5af6ae294';
  @Input() idCustomer: number;
  @Input() idService: number;
  @Input() idProject: number;
  @Input() idHito: number;

  private headers : Headers;

  

  public FileArray: FileProperties[];

  constructor(
    private http: Http
  ) {
    this.FileArray = new Array<FileProperties>();
  }

  ngOnInit() {
    if (this.headers == null){
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Authorization', this.authorization);
    }
  }

  fileChange(event) {
      //var preview = document.querySelector('img');
      //var file    = document.querySelector('input[type=file]')['files'][0];
      var files = document.querySelector('#btnUpload')['files'];
      var file    = files[files.length-1];
      var reader  = new FileReader();
      var that = this;
      reader.addEventListener("load", function () {
        var extension = file.name.split(".").pop();
        that.loadFile(reader.result, file.name, extension);
      }, false);

      if (file) {
        reader.readAsDataURL(file);
      }
  }

  loadFile(file: any, fileName: string, extension: string){

    var imgPath = "./images/" + extension + ".png";

    var img:any;
    
    try{
      img = require("./images/" + extension + ".png");
    }
    catch(e){
      img = require("./images/other.png");
    }
    
    var fileProperties = <FileProperties>{
      File: file,
      FileName: fileName,
      Extension: extension,
      Path: img,
      Customer: this.idCustomer,
      Service: this.idService,
      Project: this.idProject,
      Hito: this.idHito
    };

    this.FileArray.push(fileProperties);

  }

  public deleteFile(index: number){
    this.FileArray.splice(index, 1);
  }

  public loadFilesFromBackend(){

      var that = this;
      let params: URLSearchParams = new URLSearchParams();
      params.set('customer', this.idCustomer.toString());
      params.set('service', this.idService.toString());
      params.set('project', this.idProject.toString());
      params.set('hito', this.idHito.toString());

      let options = new RequestOptions({ headers: this.headers });  
      options.search = params;
 
      this.http.get(this.url, options)  
        .map(res => res.json())  
        .catch(error => Observable.throw(error))  
        .subscribe(  
          data => {
            data.forEach((d) => {
              that.loadFile(d.File, d.FileName, d.FileName.split('.').pop());
            });
            console.log('success')
          },  
          error => console.log(error)  
        )  
  }

  uploadFiles(){
     /*this.FileArray.forEach(e => {
       this.uploadFile(e.File, e.FileName, e.Customer, e.Service, e.Project, e.Hito)
      });*/

      var fileData = new Array<any>();

      this.FileArray.forEach(e => {
        fileData.push({
          File: e.File,
          FileName: e.FileName,
          Customer: e.Customer,
          Service: e.Service,
          Project: e.Project,
          Hito: e.Hito
        });
      });

      let options = new RequestOptions({ headers: this.headers });  
 
      this.http.post(this.url, fileData, options)  
        .map(res => res.json())  
        .catch(error => Observable.throw(error))  
        .subscribe(  
          data => console.log('success'),  
          error => console.log(error)  
        );  
  }


  
  /*uploadFile(file: any, fileName: string, customer: number, service: number, project: number, hito: number) {  

      var fileData = {
        File: file,
        FileName: fileName,
        Customer: customer,
        Service: service,
        Project: project,
        Hito: hito
      };

      let options = new RequestOptions({ headers: this.headers });  
 
      this.http.post(this.url, fileData, options)  
        .map(res => res.json())  
        .catch(error => Observable.throw(error))  
        .subscribe(  
          data => console.log('success'),  
          error => console.log(error)  
        )  
  }  */

}
