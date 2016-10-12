import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { AdminComponent } from './admin.component';
import { SignInComponent } from './sign-in';
import { EventsComponent } from './events';
import { RoomsComponent } from './rooms';
import { adminRouting } from './admin.routes';

@NgModule({
  imports: [
    SharedModule,
    adminRouting
  ],
  declarations: [
    AdminComponent,
    SignInComponent,
    EventsComponent,
    RoomsComponent
  ],
})

export class AdminModule { }
