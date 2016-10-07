import { ModuleWithProviders }  from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

export const routes: Routes = [
  { path: '', loadChildren: 'app/home/home.module#HomeModule'},
  { path: 'child-checkin', loadChildren: 'app/child-checkin/child-checkin.module#ChildCheckinModule' },
  { path: 'admin', loadChildren: 'app/admin/admin.module#AdminModule' },
];

export const routing: ModuleWithProviders = RouterModule.forRoot(routes);
