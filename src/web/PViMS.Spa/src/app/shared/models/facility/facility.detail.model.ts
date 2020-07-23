export interface FacilityDetailWrapperModel {
    value:  FacilityDetailModel[];
    recordCount: number;
}

export interface FacilityDetailModel {
    id: number;
    facilityName: string
    facilityType: string
    facilityCode: string
    contactNumber: string
    mobileNumber: string
    faxNumber: string
}