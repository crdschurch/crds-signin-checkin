import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ChildSigninComponent } from './child-signin.component';
import { AvailableChildrenComponent } from './available-children';
import { GuestComponent } from './guest';
import { AssignmentComponent } from './assignment';
import { searchRoutes } from './search/search.routes';

const childSigninRoutes: Routes = [
  {
    path: 'child-signin',
    component: ChildSigninComponent,
    children: [
      {
        path: 'search',
        children: [...searchRoutes]
      },
      {
        path: 'available-children',
        component: AvailableChildrenComponent
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
  }, 
];

export const childSigninRouting: ModuleWithProviders = RouterModule.forChild(childSigninRoutes);
