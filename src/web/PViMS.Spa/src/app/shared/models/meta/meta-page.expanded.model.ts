import { LinkModel } from "../link.model";
import { MetaWidgetDetailModel } from "./meta-widget.detail.model";

export interface MetaPageExpandedWrapperModel {
    value:  MetaPageExpandedModel[];
    recordCount: number;
}

export interface MetaPageExpandedModel {
    id: number;
    metaPageGuid: string;
    pageName: string;
    pageDefinition: string;
    metaDefinition: string;
    breadCrumb: string;
    system: string;
    visible: string;
    widgets: MetaWidgetDetailModel[];
}