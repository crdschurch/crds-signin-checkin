import { NgModule } from '@angular/core';
import { GuestModalComponent } from '../guest/guest-modal.component';
import { SharedModule } from '../../shared/shared.module';
import { FormsModule } from '@angular/forms';
import { MomentModule } from 'angular2-moment';

@NgModule({
  imports: [
    FormsModule,
    SharedModule
  ],
  declarations: [
    GuestModalComponent
  ],
  exports: [
    GuestModalComponent
  ]
})

export class GuestModalModule { }
