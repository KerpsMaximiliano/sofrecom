import { Injectable } from '@angular/core';
declare var $:any;

@Injectable()
export class DataTableService {
    
    constructor() { }

    destroy(selector){
        $(selector).DataTable().destroy();
    }

    init(selector, scroll){
        setTimeout(()=>{
            $( document ).ready(function() {

                var options = {
                    oSearch: { "bSmart": false, "bRegex": true },
                    // scrollX: false,
                    responsive: true,
                    language:
                    {
                        "sProcessing": "Procesando...",
                        "sLengthMenu": "Mostrar _MENU_ registros",
                        "sZeroRecords": "No se encontraron resultados",
                        "sEmptyTable": "Sin información disponible para esta tabla",
                        "sInfo": "Mostrando del registro _START_ al _END_ de un total de _TOTAL_",
                        "sInfoEmpty": "Mostrando del registro 0 al 0 de un total de 0",
                        "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
                        "sInfoPostFix": "",
                        "sSearch": "Buscar:",
                        "sUrl": "",
                        "sInfoThousands": ",",
                        "sLoadingRecords": "Cargando...",
                        "oPaginate": {
                            "sFirst": "Primero",
                            "sLast": "Último",
                            "sNext": "Siguiente",
                            "sPrevious": "Anterior"
                        },
                        "oAria": {
                            "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                            "sSortDescending": ": Activar para ordenar la columna de manera descendente"
                        }
                    }
                };

                // if(scroll) options.scrollX = true;

                this.tableRef = $(selector).DataTable(options);
                
            });
        });
    }

    adjustColumns(){
        setTimeout(()=>{
            $('.dataTables_scrollHeadInner').css("width", "100%");
            $('.dataTables_scrollHeadInner table').css("width", "100%");
        }, 1000);
    }
}