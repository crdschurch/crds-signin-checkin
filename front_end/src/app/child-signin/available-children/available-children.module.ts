import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { ChildSigninModule } from '../../shared/components/child-signin';

import { AvailableChildrenComponent } from './available-children.component';
import { availableChildrenRouting } from './available-children.routes';


@NgModule({
  imports: [
    ChildSigninModule,
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
