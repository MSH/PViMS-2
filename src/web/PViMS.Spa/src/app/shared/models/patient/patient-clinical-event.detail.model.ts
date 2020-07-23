import { AttributeValueModel } from "../attributevalue.model";

export interface PatientClinicalEventDetailModel {
    id: number;
    patientClinicalEventGuid: string;
    sourceDescription: string;
    onsetDate: any;
    reportDate: any;
    resolutionDate: any;
    isSerious: string;
    clinicalEventAttributes: AttributeValueModel[];
}