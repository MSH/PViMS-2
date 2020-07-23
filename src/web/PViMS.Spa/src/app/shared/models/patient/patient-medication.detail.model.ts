import { AttributeValueModel } from "../attributevalue.model";

export interface PatientMedicationDetailModel {
    id: number;
    patientMedicationGuid: string;
    sourceDescription: string;
    conceptId: number;
    productId: number;
    medication: string;
    dose: string;
    doseUnit: string;
    dosefrequency: string;
    startDate: any;
    endDate: any;
    indicationType: string;
    medicationAttributes: AttributeValueModel[];
}