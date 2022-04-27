import { LinkModel } from "../link.model";

export interface CohortGroupDetailWrapperModel {
    value:  CohortGroupDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface CohortGroupDetailModel {
    id: number;
    cohortName: string;
    cohortCode: string;
    startDate: any;
    finishDate: any;
    conditionName: string;
    enrolmentCount: number;
}