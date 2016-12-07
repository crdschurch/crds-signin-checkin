import { NgModule } from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { ReactiveFormsModule } from '@angular/forms';

import { HeaderModule } from '../../header';
import { NewFamilyRegistrationComponent } from './new-family-registration.component';
import { newFamilyRegistrationRouting } from './new-family-registration.routes';

@NgModule({
  declarations: [
    NewFamilyRegistrationComponent,
  ],
  imports: [
    newFamilyRegistrationRouting,
    SharedModule,
    MomentModule,
    HeaderModule,
    ReactiveFormsModule
  ],
  exports: [ ]
})

export class NewFamilyRegistrationModule { }