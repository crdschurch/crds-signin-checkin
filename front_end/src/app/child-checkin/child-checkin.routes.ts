import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ChildCheckinComponent } from './child-checkin.component';
import { ResultsComponent } from './results';
import { GuestComponent } from './guest';
import { AssignmentComponent } from './assignment';
import { searchRoutes } from './search/search.routes';

const childCheckinRoutes: Routes = [
  {
    path: 'child-checkin',
    component: ChildCheckinComponent,
    children: [
      {
        path: 'search',
        children: [...searchRoutes]
      },
      {
        path: 'results',
        component: ResultsComponent
      },
      {
        path: 'guest',
        component: GuestComponent
      },
      {
        path: 'assignment',
        component: AssignmentComponent
      }
    ]
  }
];

export const childCheckinRouting: ModuleWithProviders = RouterModule.forChild(childCheckinRoutes);
