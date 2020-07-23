import { FormValueModel } from "./formvalue.model";

export interface FormUploadModel {
    id: number;
    formIdentifier: string;
    patientIdentifier: string;
    formType: string;

    formValues: FormValueModel[];
}