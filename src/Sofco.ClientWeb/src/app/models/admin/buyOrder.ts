export interface BuyOrderSearchFilters {
    id?: number,
    requestNoteId?: number,
    fromDate?: Date | string,
    toDate?: Date | string,
    providerId?: number,
    statusId?: number,
    number?: string
}