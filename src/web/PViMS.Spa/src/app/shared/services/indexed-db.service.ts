import { Injectable } from "@angular/core";
import { DatePipe } from "@angular/common";
import { db, Form } from "../indexed-db/appdb";
import { FormModel } from "../models/form/form.model";

@Injectable( { providedIn: 'root' } )
export class IndexedDBService {

    constructor(
        protected datePipe: DatePipe
    ) { }

    async getFilteredFormsByType(type: string, synchForms: boolean, compForms: boolean): Promise<FormModel[]>
    {
        let synchStatus = synchForms ? 'Synched' : 'Not Synched';
        let compStatus = compForms ? 'Complete' : 'Not Complete';

        // For atomicity and speed, use a single transaction for the
        // queries to make:    
        return await db.transaction('r', [db.forms, db.formValues], async()=>{

            if(synchForms) {
              let forms = await db.forms
                  .where('synchStatus').equals(synchStatus)
                  .and(function(item) { return item.formType == type })
                  .sortBy('id');

              return forms;
            }
            else {
              let forms = await db.forms
                  .where('completeStatus').equals(compStatus)
                  .and(function(item) { return item.formType == type })
                  .and(function(item) { return item.synchStatus == 'Not Synched' })
                  .sortBy('id');

              return forms;
            }
        });
    }

    async getAllFormsForType(type: string): Promise<FormModel[]>
    {
        // For atomicity and speed, use a single transaction for the
        // queries to make:    
        return await db.transaction('r', [db.forms, db.formValues], async()=>{

            // Query some forms
            let forms = await db.forms
                .where('formType').equals(type)
                .sortBy('id');

            return forms;
        });
    }
    
    async searchFormsForType(type: string, searchTerm: string): Promise<FormModel[]>
    {
      // For atomicity and speed, use a single transaction for the
      // queries to make:    
      return await db.transaction('r', [db.forms, db.formValues], async()=>{

          // Query some forms
          let forms = await db.forms
              .where('formType').equals(type)
              .and(function(item) { return item.patientIdentifier.includes(searchTerm) || item.patientName.toLowerCase().includes(searchTerm.toLowerCase()) || item.formIdentifier.includes(searchTerm)})
              .sortBy('id');

          return forms;
      });      
    }      

    async addNewForm(type: string, modelForm: any, patientForm: any, models: any[]) : Promise<number>
    {
      let created = new Date();
      let createdDisplay = this.datePipe.transform(created, 'yyyy-MM-dd');
      let patientIdentifier = patientForm.patientIdentifier;
      let patientName =  `${patientForm.patientFirstName} ${patientForm.patientLastName}`;
      let completeStatus = modelForm.formCompleted ? 'Complete' : 'Not Complete';
      let formIdentifier = type.substring(4) + `-${this.datePipe.transform(created, 'MMdd')}-${this.datePipe.transform(created, 'HHmmss')}`;
      let formid = 0;

      await db.transaction('rw', db.forms, db.formValues, async function () {
          // Populate a new form
          formid = await db.forms.add(new Form(
              createdDisplay, 
              formIdentifier, 
              patientIdentifier, 
              patientName, 
              completeStatus, 
              type,
              null,
              false,
              null,
              false
          ));

          // Populate form values for this form
          db.formValues.add({ formid: formid, formUniqueIdentifier: formIdentifier, formControlKey: modelForm.constructor.name, formControlValue: JSON.stringify(modelForm) });
          db.formValues.add({ formid: formid, formUniqueIdentifier: formIdentifier, formControlKey: patientForm.constructor.name, formControlValue: JSON.stringify(patientForm) });
          models.forEach(model => {
            db.formValues.add({ formid: formid, formUniqueIdentifier: formIdentifier, formControlKey: model.constructor.name, formControlValue: JSON.stringify(model) });
          });
      });
    
      return new Promise<number>((resolve) => {
          resolve(formid);
      });       
    }

    async getForm(id: number): Promise<Form>
    {
        let form = await db.forms.get(id);
        await form.loadNavigationProperties();
        return form;
    }

    async updateForm(id: number, modelForm: any, patientForm: any, models: any[])
    {
        let form = await db.forms.get(id);
        await form.loadNavigationProperties();

        form.patientIdentifier = patientForm.patientIdentifier;
        form.patientName = `${patientForm.patientFirstName} ${patientForm.patientLastName}`;
        form.completeStatus = modelForm.formCompleted ? 'Complete' : 'Not Complete';

        form.formValues[0].formControlValue = JSON.stringify(modelForm);
        form.formValues[1].formControlValue = JSON.stringify(patientForm);

        let index = 1;
        models.forEach(model => {
            index ++;
            form.formValues[index].formControlValue = JSON.stringify(model);
        });

        await form.save();
    }

    async deleteForm(id: number)
    {
        await db.forms.delete(id);
    }

    async updateAttachment(id: number, imagebin: any, index: number)
    {
        let form = await db.forms.get(id);
        await form.loadNavigationProperties();

        if(index == 1) {
          form.attachment = imagebin;
          form.hasAttachment = true;
  
          await form.save();
  
        } 
        else if (index == 2) {
          form.attachment_2 = imagebin;
          form.hasSecondAttachment = true;
  
          await form.save();
        }
    }        

    async deleteAttachment(id: number, index: number)
    {
        let form = await db.forms.get(id);
        await form.loadNavigationProperties();

        if(index == 1) {
          form.attachment = null;
          form.hasAttachment = false;
  
          await form.save();
        }
        else if (index == 2) {
          form.attachment_2 = null;
          form.hasSecondAttachment = false;
  
          await form.save();
        }
    }

    async markFormAsSynched(id: number)
    {
        let form = await db.forms.get(id);
        await form.loadNavigationProperties();

        form.synchStatus = 'Synched';
        
        await form.save();
    }

    async markFormAsCompleted(id: number)
    {
        let form = await db.forms.get(id);
        await form.loadNavigationProperties();

        form.completeStatus = 'Complete';
        
        await form.save();
    }    
}