import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { HomeComponent } from './home.component';
import { homeRouting } from './home.routes';

@NgModule({
  declarations: [
    HomeComponent
  ],
  imports: [
    SharedModule,
    homeRouting
  ],
})

export class HomeModule { }
