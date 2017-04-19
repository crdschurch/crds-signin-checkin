import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ServingToggleModule } from '../serving/serving-toggle.module';
import { ChildSigninCardModule } from '../../shared/components/child-signin-card';
import { MomentModule } from 'angular2-moment';
import { AvailableChildrenComponent } from './available-children.component';
import { availableChildrenRouting } from './available-children.routes';


@NgModule({
  declarations: [
    AvailableChildrenComponent,
  ],
  imports: [
    ChildSigninCardModule,
    SharedModule,
    MomentModule,
    ServingToggleModule,
    availableChildrenRouting
  ],
  exports: [
    AvailableChildrenComponent
  ]
})

export class AvailableChildrenModule { }
