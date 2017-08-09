import { NgModule } from '@angular/core';

import { SharedModule } from '../../../../shared/shared.module';
import { VerificationModalComponent } from './verification-modal.component';

@NgModule({
  imports: [
    SharedModule
  ],
  declarations: [
    VerificationModalComponent
  ],
  exports: [
    VerificationModalComponent
  ]
})

export class VerificationModalModule { }
