import { Injectable } from '@angular/core';
import { Configuration } from './configuration';
declare var $: any;

@Injectable()
export class DataTableService {

    constructor(private config: Configuration) { }

    destroy(selector){
        $(selector).DataTable().destroy();
    }

    initialize(params){
        var lang = {};
        if(this.config.currLang == "es" || this.config.currLang == "fr"){
          lang = this.getLanguageEs();
        }

        setTimeout(()=>{
            $( document ).ready(function() {
                var options: any = {
                    oSearch: { "bSmart": false, "bRegex": true },
                    responsive: true, 
                    language: lang,
                }

                if(params.scrollX && params.scrollX == true){
                    options.scrollX = true;
                }

                if(params.columnDefs){
                    options.aoColumnDefs = params.columnDefs;
                }

                if(params.order){
                    options.order = params.order;
                }

                if(params.withExport){
                    const excelExport: any = {
                        extend: 'excelHtml5',
                        title: params.title,
                        exportOptions: {
                            columns: params.columns
                        }
                    };

                    if(params.customizeExcelExport){
                        excelExport.customize = params.customizeExcelExport;
                    }

                    if(params.customizeExcelExportData){
                        excelExport.customizeData = params.customizeExcelExportData;
                    }

                    options.dom = '<"html5buttons"B>lTfgitp';
                    options.buttons = [ excelExport ];
                }

                $(params.selector).DataTable(options);
            });
        });
    }

    // adjustColumns(){
    //     setTimeout(()=>{
    //         $('.dataTables_scrollHeadInner').css("width", "100%");
    //         $('.dataTables_scrollHeadInner table').css("width", "100%");
    //     }, 1000);
    // }

    getLanguageEs(){
       var language =
        {
            sProcessing: "Procesando...",
            sLengthMenu: "Mostrar _MENU_ registros",
            sZeroRecords: "No se encontraron resultados",
            sEmptyTable: "Sin información disponible para esta tabla",
            sInfo: "Mostrando del registro _START_ al _END_ de un total de _TOTAL_",
            sInfoEmpty: "Mostrando del registro 0 al 0 de un total de 0",
            sInfoFiltered: "(filtrado de un total de _MAX_ registros)",
            sInfoPostFix: "",
            sSearch: "Buscar:",
            sUrl: "",
            sInfoThousands: ",",
            sLoadingRecords: "Cargando...",
            oPaginate: {
                sFirst: "Primero",
                sLast: "Último",
                sNext: "Siguiente",
                sPrevious: "Anterior"
            },
            oAria: {
                sSortAscending: ": Activar para ordenar la columna de manera ascendente",
                sSortDescending: ": Activar para ordenar la columna de manera descendente"
            }
        };

        return language;
    }
}