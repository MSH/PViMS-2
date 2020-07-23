export interface MetaTableDetailWrapperModel {
    value:  MetaTableDetailModel[];
    recordCount: number;
}

export interface MetaTableDetailModel {
    id: number;
    metaTableGuid: string;
    tableName: string;
    friendlyName: string;
    friendlyDescription: string;
    tableType: string;
}