import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { ChildSigninComponent } from './child-signin.component';
import { SearchModule } from './search';
import { AvailableChildrenModule } from './available-children';
import { AssignmentComponent } from './assignment';
import { childSigninRouting } from './child-signin.routes';
import { MomentModule } from 'angular2-moment';

@NgModule({
  declarations: [
    ChildSigninComponent,
    AssignmentComponent,
  ],
  imports: [
    SharedModule,
    SearchModule,
    MomentModule,
    AvailableChildrenModule,
    childSigninRouting
  ],
})

export class ChildSigninModule { }
