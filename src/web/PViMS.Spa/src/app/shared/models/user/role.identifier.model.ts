export interface RoleIdentifierWrapperModel {
    value:  RoleIdentifierModel[];
    recordCount: number;    
}

export interface RoleIdentifierModel {
    id: number;
    name: string
    key: string
}