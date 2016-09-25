import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { AssignmentComponent } from './assignment';
import { GuestComponent } from './guest';
import { RoomComponent } from './room';
import { AppComponent } from './search';

import { routing } from './child-checkin.routes';

@NgModule({
  declarations: [
    AssignmentComponent,
    GuestComponent,
    RoomComponent
  ],
  imports: [
    CommonModule,
    routing
  ],
})

export class ChildCheckinModule { }
