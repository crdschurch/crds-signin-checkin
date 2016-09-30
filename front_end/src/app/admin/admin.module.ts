import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { AdminComponent } from './admin.component';
import { DashboardComponent } from './dashboard';
import { adminRouting } from './admin.routes';

@NgModule({
  declarations: [
    AdminComponent,
    DashboardComponent
  ],
  imports: [
    SharedModule,
    adminRouting
  ],
})

export class AdminModule { }
