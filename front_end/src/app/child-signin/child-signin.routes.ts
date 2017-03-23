import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ChildSigninComponent } from './child-signin.component';
import { AssignmentComponent } from './assignment';
import { searchRoutes } from './search/search.routes';
import { availableChildrenRoutes } from './available-children/available-children.routes';

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
        path: 'available-children/:phoneNumber',
        children: [...availableChildrenRoutes]
      },
      {
        path: 'assignment',
        component: AssignmentComponent
      }
    ]
  }
];

export const childSigninRouting: ModuleWithProviders = RouterModule.forChild(childSigninRoutes);
