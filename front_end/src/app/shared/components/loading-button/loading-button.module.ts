import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { LoadingButtonComponent } from './loading-button.component';

@NgModule({
  imports: [CommonModule, FormsModule ],
  exports: [LoadingButtonComponent],
  declarations: [LoadingButtonComponent],
  providers: [],
})
export class LoadingButtonModule { }
