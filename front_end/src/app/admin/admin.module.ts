import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { AdminComponent } from './admin.component';
import { DashboardComponent } from './dashboard';
import { SignInComponent } from './sign-in';
import { adminRouting } from './admin.routes';

@NgModule({
  imports: [
    SharedModule,
    adminRouting
  ],
  declarations: [
    AdminComponent,
    DashboardComponent,
    SignInComponent,
  ],
})

export class AdminModule { }
