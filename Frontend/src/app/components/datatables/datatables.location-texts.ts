export class DatatablesLocationTexts{

    constructor(
        public other1Text: string = "",
        public other2Text: string = "",
        public other3Text: string = "",
        public editText: string = "Editar",
        public deleteText: string = "Eliminar",
        public searchText: string = "Buscar",
        public habilitateText: string = "Habilitar",
        public inHabilitateText: string = "Inhabilitar",
        public deleteQuestion: string = 'Está seguro de eliminar "¶" ?', //¶ : placeholder para ubicar la entidad a eliminar
        public deleteQuestionTitle: string = 'Confirmar Eliminación'
    ){}
}