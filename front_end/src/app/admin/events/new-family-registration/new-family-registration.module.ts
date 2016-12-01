import { NgModule } from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { ReactiveFormsModule } from '@angular/forms';

import { HeaderModule } from '../../header';
import { NewParentComponent } from './new-parent/new-parent.component';
import { NewChildComponent } from './new-child/new-child.component';
import { NewFamilyRegistrationComponent } from './new-family-registration.component';
import { newFamilyRegistrationRouting } from './new-family-registration.routes';

@NgModule({
  declarations: [
    NewParentComponent,
    NewChildComponent,
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
