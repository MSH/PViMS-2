import { Location } from '../../../../node_modules/@angular/common';
import { environment } from '../../../environments/environment';
import { PopupService } from '../services/popup.service';
import { AccountService } from '../services/account.service';
import { FormGroup, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';

export class BasePopupComponent {

    protected busy: boolean = false;

    constructor(
        protected _router: Router,
        protected _location: Location,
        protected popupService: PopupService,
        protected accountService: AccountService) {
    }

    public CLog(object: any, title: string = undefined) {
        if (!environment.production) {
            console.log({ title: title, object });
        }
    }

    public CLogFormErrors(viewModelForm: FormGroup) {
      Object.keys(viewModelForm.controls).forEach(key => {

        const controlErrors: ValidationErrors = viewModelForm.get(key).errors;
        if (controlErrors != null) {
          Object.keys(controlErrors).forEach(keyError => {
            console.log('Key control: ' + key + ', keyError: ' + keyError + ', err value: ', controlErrors[keyError]);
          });
        }
      })
    }

    public isBusy(): boolean {
        return this.busy;
    }

    public setBusy(value: boolean): void {
        setTimeout(() => { this.busy = value; });
    }

    protected showError(errorMessage: any, title: string = "Error") {
        this.popupService.showErrorMessage(errorMessage, title);
    }

    protected showInfo(message: string, title: string = "Info") {
        this.popupService.showInfoMessage(message, title);
    }

    protected notify(message: string, action: string) {
        return this.popupService.notify(message, action);
    }

    protected showConfirm(message: string, title: string = "Confirm") {
        this.popupService.showConfirmMessage(message, title);
    }

    protected throwError(errorObject: any, title: string = "Exception") {
        if (errorObject.status == 401) {
            this.showError(errorObject.error.message, errorObject.error.statusCodeType);
        } else {
            this.showError(errorObject.message, title);
        }
    }

    protected handleError(errorObject: any, title: string = "Exception")
    {
      console.log(errorObject);

      let message = '';
      if(errorObject.message) {
        if(Array.isArray(errorObject.message)) {
          message = errorObject.message[0];
        }
        else {
          message = errorObject.message;
        }
      }
      else {
        message = "Unknown error experienced. Please contact your system administrator. ";
      }

      if(errorObject.ReferenceCode) {
        message += `Reference Code: ${errorObject.ReferenceCode}`;
      }

      this.CLog(errorObject, title);
      this.showError(message, title);
    }

    public setForm(form: FormGroup, value: any): void {
        form.setValue(value);
    }

    public updateForm(form: FormGroup, value: any): void {
        form.patchValue(value);
    }

    public resetForm(form: FormGroup): void {
        form.reset();
    }
}
