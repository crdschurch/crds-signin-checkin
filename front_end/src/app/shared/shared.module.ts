import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';

import { PhoneNumberPipe } from './pipes/phoneNumber.pipe';

import { PreloaderModule } from './preloader';

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
    FormsModule,
    Ng2BootstrapModule,
    PreloaderModule
  ],
})
export class SharedModule {
}
