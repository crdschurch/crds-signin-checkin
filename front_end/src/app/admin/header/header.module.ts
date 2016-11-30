import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MomentModule } from 'angular2-moment';
import { SharedModule } from '../../shared/shared.module';

import { HeaderComponent } from './header.component';
import { HeaderService } from './header.service';

@NgModule({
  declarations: [
    HeaderComponent,
  ],
  imports: [
    SharedModule,
    RouterModule,
    MomentModule,
  ],
  exports: [
    HeaderComponent,
  ],
  providers: [
    HeaderService,
  ],
})

export class HeaderModule { }
