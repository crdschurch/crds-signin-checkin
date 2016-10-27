import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AvailableChildrenComponent } from './available-children.component';

export const availableChildrenRoutes: Routes = [
  { path: '', component: AvailableChildrenComponent }
];

export const availableChildrenRouting: ModuleWithProviders = RouterModule.forChild(availableChildrenRoutes);
