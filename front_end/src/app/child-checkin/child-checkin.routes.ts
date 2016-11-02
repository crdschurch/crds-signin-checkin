import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ChildCheckinComponent } from './child-checkin.component';
import { RoomComponent } from './room';

const childCheckinRoutes: Routes = [
  {
    path: 'child-checkin',
    component: ChildCheckinComponent,
    children: [
      {
        path: 'room',
        component: RoomComponent
      },
    ]
  }
];

export const childCheckinRouting: ModuleWithProviders = RouterModule.forChild(childCheckinRoutes);
