import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ReportInstanceMedicationDetailModel } from 'app/shared/models/report-instance/report-instance-medication.detail.model';

@Component({
  selector: 'medicationlist-popup',
  styleUrls: ['./medicationlist.popup.component.scss'],
  templateUrl: './medicationlist.popup.component.html'
})
export class MedicationListPopupComponent implements OnInit {
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: MedicationListPopupData,
    public dialogRef: MatDialogRef<MedicationListPopupComponent>,
  ) { }

  ngOnInit(): void {
  }
}

export interface MedicationListPopupData {
  medications: ReportInstanceMedicationDetailModel[];
  title: string;
}