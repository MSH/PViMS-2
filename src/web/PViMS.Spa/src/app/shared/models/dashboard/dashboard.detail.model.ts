import { LinkModel } from "../link.model";

export interface DashboardDetailWrapperModel {
    value:  DashboardDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface DashboardDetailModel {
    id: number;
    uid: string;
    name: string;
    shortName: string;
    longName: string;
    frequency: string;
    active: string;
    createdDetail: string;
    updatedDetail: string;
    icon: string;
}