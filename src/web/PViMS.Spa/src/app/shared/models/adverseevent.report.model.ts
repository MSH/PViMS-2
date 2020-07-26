export interface AdverseEventReportWrapperModel {
    value:  AdverseEventReportModel[];
    recordCount: number;
}

export interface AdverseEventReportModel {
    adverseEvent: string;
    criteria: string;
    serious: string;
    patientCount: number;
}