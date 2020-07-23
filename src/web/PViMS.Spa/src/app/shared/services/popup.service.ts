import { Injectable } from '@angular/core';
import { MatDialog, MatSnackBar, MatDialogRef } from '@angular/material';
import { InfoPopupComponent } from '../components/popup/info.popup.component';
import { ErrorPopupComponent } from '../components/popup/error.popup.component';
import { ConfirmPopupComponent } from '../components/popup/confirm.popup.component';

@Injectable({ providedIn: 'root' })
export class PopupService {

    constructor(
        private dialog: MatDialog,
        private snackBar: MatSnackBar) {
    }

    showInfoMessage(message: string, title: string = "Info") {
        let dialogRef: MatDialogRef<InfoPopupComponent>;
        dialogRef = this.dialog.open(InfoPopupComponent, {
            autoFocus: true,
            width: '500px',
            disableClose: true,
            data: { title: title, message: message }
        });
    }

    showErrorMessage(errorMessage: any, title: string = "Error") {
        let dialogRef: MatDialogRef<ErrorPopupComponent>;
        dialogRef = this.dialog.open(ErrorPopupComponent, {
            autoFocus: true,
            width: '500px',
            disableClose: true,
            data: { title: title, message: errorMessage }
        });
    }

    showConfirmMessage(message: any, title: string = "Confirm") {
        let dialogRef: MatDialogRef<ConfirmPopupComponent>;
        dialogRef = this.dialog.open(ConfirmPopupComponent, {
            autoFocus: true,
            width: '500px',
            disableClose: true,
            data: { title: title, message: message }
        });
    }

    notify(message: string, action: string) {
        this.snackBar.open(message, action, {
            duration: 3000,
            verticalPosition: 'top', // 'top' | 'bottom'
            horizontalPosition: 'center', //'start' | 'center' | 'end' | 'left' | 'right'
            panelClass: ['blue-snackbar'],            
        });
    }
}
