import { NgModule } from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { InputTrimDirective } from '../../../shared/directives/input-trim.directive';
import { TextMaskModule } from 'angular2-text-mask';
import { MomentModule } from 'angular2-moment';
import { ReactiveFormsModule } from '@angular/forms';

import { HeaderModule } from '../../header';
import { NewFamilyRegistrationComponent } from './new-family-registration.component';
import { newFamilyRegistrationRouting } from './new-family-registration.routes';

@NgModule({
  declarations: [
    NewFamilyRegistrationComponent,
    InputTrimDirective
  ],
  imports: [
    newFamilyRegistrationRouting,
    SharedModule,
    MomentModule,
    HeaderModule,
    ReactiveFormsModule,
    TextMaskModule
  ],
  exports: [ ]
})

export class NewFamilyRegistrationModule { }
