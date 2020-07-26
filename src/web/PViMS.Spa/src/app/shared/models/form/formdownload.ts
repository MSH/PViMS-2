import { SynchMessageModel } from "./synchmessage.model";

export interface FormDownloadModel {
    id: number;
    formIdentifier: string;

    validationMessages: SynchMessageModel[];
    commentMessages: SynchMessageModel[];
}