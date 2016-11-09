import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';
import { PhoneNumberPipe } from './pipes/phoneNumber.pipe';
import { OneBasedIndexPipe } from './pipes/one-based-index.pipe';
import { ApiService } from './services';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    PhoneNumberPipe,
    OneBasedIndexPipe
  ],
  exports: [
    PhoneNumberPipe,
    OneBasedIndexPipe,
    CommonModule,
    FormsModule,
    Ng2BootstrapModule
  ],
  providers:  [
    ApiService
  ]
})
export class SharedModule {
}
