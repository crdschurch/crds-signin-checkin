import { Component, ViewEncapsulation } from '@angular/core';
import { HeaderService } from './header/header.service';
import { AdminService } from './admin.service';

@Component({
  selector: 'admin',
  templateUrl: 'admin.component.html',
  styleUrls: ['admin.component.scss'],
  providers: [ AdminService, HeaderService ],
  encapsulation: ViewEncapsulation.None
})
export class AdminComponent {}
