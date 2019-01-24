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
        let lang = {};

        if(this.config.currLang == "es"){
          lang = this.getLanguageEs();
        }

        if(this.config.currLang == "fr"){
            lang = this.getLanguageFr();
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

                if(params.withOutSorting){
                    options.bSort = false;
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

    getLanguageEs(): any{
        return {
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
            decimal: ",",
            thousands: ".",
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
    }

    getLanguageFr(): any {
        return {
            sProcessing: "En traitement...",
            sLengthMenu: "Afficher _MENU_ lignes",
            sZeroRecords: "Aucun résultats n'a été trouvée",
            sEmptyTable: "Aucune information n'est disponible pour cette table",
            sInfo: "Afficher la ligne _START_ a _END_ d'un total de _TOTAL_",
            sInfoEmpty: "Afficher la ligne 0 a 0 d'un total de 0",
            sInfoFiltered: "(filtré d'un total de _MAX_ lignes)",
            sInfoPostFix: "",
            sSearch: "Chercher:",
            sUrl: "",
            sInfoThousands: ",",
            sLoadingRecords: "En chargement...",
            oPaginate: {
                sFirst: "Premier",
                sLast: "Dernier",
                sNext: "Suivant",
                sPrevious: "Précédent"
            },
            oAria: {
                sSortAscending: ": Activer pour ordonner la colonne en croissant",
                sSortDescending: ": Activer pour commander la colonne dans l'ordre décroissant"
            }
        };
    }
}
