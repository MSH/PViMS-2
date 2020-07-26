export interface ProductDetailWrapperModel {
    value:  ProductDetailModel[];
    recordCount: number;
}

export interface ProductDetailModel {
    id: number;
    productName: string;
    displayName: string;
    conceptName: string;
    formName: string;
    manufacturer: string;
    active: string;
}