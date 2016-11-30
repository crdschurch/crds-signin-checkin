import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MomentModule } from 'angular2-moment';
import { Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';

import { CrdsDatePickerComponent } from './crds-datepicker.component';

@NgModule({
  imports: [CommonModule, FormsModule, MomentModule, Ng2BootstrapModule],
  exports: [CrdsDatePickerComponent],
  declarations: [CrdsDatePickerComponent],
  providers: [],
})
export class CrdsDatePickerModule { }
