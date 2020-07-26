export interface ConceptDetailWrapperModel {
    value:  ConceptDetailModel[];
    recordCount: number;
}

export interface ConceptDetailModel {
    id: number;
    conceptName: string;
    displayName: string;
    formName: string;
    active: string;
}