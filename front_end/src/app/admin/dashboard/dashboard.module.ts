import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardComponent }   from './dashboard.component';
import { dashboardRouting } from './dashboard.routes';

@NgModule({
    imports: [ CommonModule, dashboardRouting ],
    declarations: [ DashboardComponent ]
})
export class DashboardModule { }
