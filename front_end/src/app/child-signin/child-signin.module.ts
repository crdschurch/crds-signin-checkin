import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { ChildSigninComponent } from './child-signin.component';
import { SearchModule } from './search';
import { AvailableChildrenComponent } from './available-children';
import { GuestComponent } from './guest';
import { AssignmentComponent } from './assignment';
import { childSigninRouting } from './child-signin.routes';

@NgModule({
  declarations: [
    ChildSigninComponent,
    AvailableChildrenComponent,
    GuestComponent,
    AssignmentComponent,
  ],
  imports: [
    SharedModule,
    SearchModule,
    childSigninRouting
  ],
})

export class ChildSigninModule { }
