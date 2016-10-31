import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';

import { AvailableChildrenComponent } from './available-children.component';
import { AvailableChildComponent } from './available-child';
import { availableChildrenRouting } from './available-children.routes';


@NgModule({
  declarations: [
    AvailableChildrenComponent,
    AvailableChildComponent
  ],
  imports: [
    SharedModule,
    MomentModule,
    availableChildrenRouting
  ],
  exports: [
    AvailableChildrenComponent,
  ]
})

export class AvailableChildrenModule { }
