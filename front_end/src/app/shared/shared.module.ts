import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';

import { HttpClient } from './services/http-client.service';
import { PhoneNumberPipe } from './pipes/phoneNumber.pipe';

@NgModule({
  imports: [
    CommonModule,
    HttpClient
  ],
  providers: [
    HttpClient,
  ],
  declarations: [
    PhoneNumberPipe
  ],
  exports: [
    PhoneNumberPipe,
    CommonModule,
    FormsModule,
    Ng2BootstrapModule
  ],
})
export class SharedModule {
}
