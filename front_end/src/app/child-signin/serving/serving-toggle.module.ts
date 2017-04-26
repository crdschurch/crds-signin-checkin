import { NgModule } from '@angular/core';
import { ServingToggleComponent } from '../serving/serving-toggle.component';
import { SharedModule } from '../../shared/shared.module';
import { FormsModule } from '@angular/forms';
import { MomentModule } from 'angular2-moment';

@NgModule({
  imports: [
    FormsModule,
    SharedModule
  ],
  declarations: [
    ServingToggleComponent
  ],
  exports: [
    ServingToggleComponent
  ]
})

export class ServingToggleModule { }
