import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ChildCheckinComponent } from './child-checkin.component';
import { ResultsComponent } from './results';
import { GuestComponent } from './guest';
import { AssignmentComponent } from './assignment';

const childCheckinRoutes: Routes = [
  // {
    // path: 'child-checkin',
    // component: ChildCheckinComponent,
    // children: [
    //   {
    //     path: 'results',
    //     component: ResultsComponent
    //   },
    //   {
    //     path: 'guest',
    //     component: GuestComponent
    //   },
    //   {
    //     path: 'assignment',
    //     component: AssignmentComponent
    //   }
    // ]
  // }, 
  { path: 'child-checkin', loadChildren: 'app/child-checkin/search/search.module#SearchModule' }
];

export const childCheckinRouting: ModuleWithProviders = RouterModule.forChild(childCheckinRoutes);
