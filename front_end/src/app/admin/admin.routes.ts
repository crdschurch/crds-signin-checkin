import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AdminComponent } from './admin.component';
import { SignInComponent } from './sign-in';
import { EventsComponent } from './events';
import { RoomsComponent } from './rooms';
import { RoomComponent } from './room';

const adminRoutes: Routes = [
  {
    path: 'admin', component: AdminComponent,
    children: [
      { path: 'sign-in', component: SignInComponent },
      { path: 'events', component: EventsComponent },
      { path: 'events/:eventId/rooms', component: RoomsComponent },
      { path: 'events/:eventId/rooms/:roomId', component: RoomComponent }
    ]
  }
];

export const adminRouting: ModuleWithProviders = RouterModule.forChild(adminRoutes);
