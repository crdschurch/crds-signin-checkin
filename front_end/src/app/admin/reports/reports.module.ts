import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { ReactiveFormsModule } from '@angular/forms';

import { HeaderModule } from '../header';


@NgModule({
  declarations: [
    ReportsComponent,
  ],
  imports: [

    HeaderModule,
  ],
  exports: [ ]
})

export class ReportsModule { }
