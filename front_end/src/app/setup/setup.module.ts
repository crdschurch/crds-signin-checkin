import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { MomentModule } from 'angular2-moment';

import { SetupComponent } from './setup.component';
import { setupRouting } from './setup.routes';

@NgModule({
  declarations: [
    SetupComponent
  ],
  imports: [
    setupRouting,
    SharedModule,
    MomentModule
  ],
  exports: [],
  providers: []
})

export class SetupModule { }
