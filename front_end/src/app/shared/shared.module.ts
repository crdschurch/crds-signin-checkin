import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';

import { PhoneNumberPipe } from './pipes/phoneNumber.pipe';

import { PreloaderModule } from './preloader';
import { CrdsDatePickerModule, LoadingButtonModule } from './components';
import { CanActivateIfLoggedInGuard } from './guards';

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
    PreloaderModule,
    CrdsDatePickerModule,
    LoadingButtonModule
  ],
  providers: [
    CanActivateIfLoggedInGuard
  ]
})
export class SharedModule {
}
