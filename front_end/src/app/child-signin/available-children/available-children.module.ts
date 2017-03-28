import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { AvailableChildModule } from '../../shared/components/available-child/available-child.module';

import { AvailableChildrenComponent } from './available-children.component';
import { availableChildrenRouting } from './available-children.routes';


@NgModule({
  imports: [
    AvailableChildModule,
    SharedModule,
    MomentModule,
    availableChildrenRouting
  ],
  declarations: [
    AvailableChildrenComponent
  ],
  exports: [
    AvailableChildrenComponent
  ]
})

export class AvailableChildrenModule { }
