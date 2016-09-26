import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { PhoneNumberPipe } from './phoneNumber.pipe';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    PhoneNumberPipe
  ],
  exports: [
    PhoneNumberPipe,
    CommonModule,
    FormsModule
  ],
})
export class SharedModule {
}
