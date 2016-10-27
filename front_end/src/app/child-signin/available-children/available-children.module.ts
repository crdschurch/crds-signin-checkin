import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';

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
    availableChildrenRouting
  ],
  exports: [
    AvailableChildrenComponent,
  ]
})

export class AvailableChildrenModule { }
