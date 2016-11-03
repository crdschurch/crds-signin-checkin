import { ModuleWithProviders }  from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

export const routes: Routes = [
  { path: '', loadChildren: 'app/home/home.module#HomeModule'},
  { path: 'child-checkin/search', loadChildren: 'app/child-checkin/child-checkin.module#ChildCheckinModule' },
  { path: 'child-signin/search', loadChildren: 'app/child-signin/child-signin.module#ChildSigninModule' },
  { path: 'admin', loadChildren: 'app/admin/admin.module#AdminModule' },
  { path: 'setup', loadChildren: 'app/setup/setup.module#SetupModule' },
];

export const routing: ModuleWithProviders = RouterModule.forRoot(routes);
