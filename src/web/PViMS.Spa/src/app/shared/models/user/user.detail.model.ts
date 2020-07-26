export interface UserDetailModel {
    id: number;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    allowDatasetDownload: string;
    active: string;
    roles: string[];
    facilities: string[];
}