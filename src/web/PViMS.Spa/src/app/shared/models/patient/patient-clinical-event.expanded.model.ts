import { AttributeValueModel } from "../attributevalue.model";
import { ReportInstanceMedicationDetailModel } from "../report-instance/report-instance-medication.detail.model";
import { ActivityExecutionStatusEventModel } from "../activity/activity-execution-status-event.model";

export interface PatientClinicalEventExpandedModel {
    id: number;
    patientClinicalEventGuid: string;
    sourceDescription: string;
    onsetDate: any;
    reportDate: any;
    resolutionDate: any;
    isSerious: string;
    clinicalEventAttributes: AttributeValueModel[];
    setMedDraTerm: string;
    medications: ReportInstanceMedicationDetailModel[];
    activity: ActivityExecutionStatusEventModel[];
}