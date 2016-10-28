import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SearchComponent } from './search.component';

export const searchRoutes: Routes = [
  { path: '', component: SearchComponent }
];

export const searchRouting: ModuleWithProviders = RouterModule.forChild(searchRoutes);
