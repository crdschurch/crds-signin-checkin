import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
// import { AvailableChildModule } from '../../shared/components/available-child';

import { AvailableChildrenComponent } from './available-children.component';
import { availableChildrenRouting } from './available-children.routes';


@NgModule({
  declarations: [
    AvailableChildrenComponent
  ],
  imports: [
    // AvailableChildModule,
    SharedModule,
    MomentModule,
    availableChildrenRouting
  ],
  exports: [
    AvailableChildrenComponent,
  ]
})

export class AvailableChildrenModule { }
