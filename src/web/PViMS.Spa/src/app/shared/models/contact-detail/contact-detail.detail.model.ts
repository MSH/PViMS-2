export interface ContactDetailWrapperModel {
    value:  ContactDetailModel[];
    recordCount: number;
}

export interface ContactDetailModel {
    id: number;
    contactType: string;
    contactFirstName: string;
    contactLastName: string;
    organisationName: string;
    streetAddress: string;
    city: string;
    state: string;
    postCode: string;
    contactNumber: string;
    contactEmail: string;
    countryCode: string;
}