import { AttributeValueModel } from "../attributevalue.model";

export interface PatientDetailWrapperModel {
    value:  PatientDetailModel[];
    recordCount: number;
}

export interface PatientDetailModel {
    id: number;
    patientGuid: string;
    facilityName: string;
    firstName: string;
    middleName: string;
    lastName: string;
    dateOfBirth: any;
    age: number;
    ageGroup: string;
    createdDetail: string;
    updatedDetail: string;
    latestEncounterDate: any;
    currentStatus: string;
    medicalRecordNumber: string;
    patientAttributes: AttributeValueModel[];
}