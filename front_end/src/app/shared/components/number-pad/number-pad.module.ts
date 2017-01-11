import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { NumberPadComponent } from './number-pad.component';

@NgModule({
  imports: [ CommonModule],
  exports: [ NumberPadComponent ],
  declarations: [ NumberPadComponent ],
  providers: [],
})
export class NumberPadModule { }
