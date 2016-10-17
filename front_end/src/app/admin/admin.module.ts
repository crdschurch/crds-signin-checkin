import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { MomentModule } from 'angular2-moment';

import { AdminComponent } from './admin.component';
import { SignInComponent } from './sign-in';
import { EventsComponent } from './events';
import { RoomsComponent } from './rooms';
import { RoomComponent } from './room';
import { adminRouting } from './admin.routes';

@NgModule({
  imports: [
    SharedModule,
    adminRouting,
    MomentModule
  ],
  declarations: [
    AdminComponent,
    SignInComponent,
    EventsComponent,
    RoomsComponent,
    RoomComponent
  ],
})

export class AdminModule { }
