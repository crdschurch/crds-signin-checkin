import { Component, ViewEncapsulation } from '@angular/core';
import { AdminService } from './admin.service';
import { ChildSigninService } from '../child-signin/child-signin.service';

@Component({
  selector: 'admin',
  templateUrl: 'admin.component.html',
  styleUrls: ['admin.component.scss'],
  providers: [ AdminService, ChildSigninService ],
  encapsulation: ViewEncapsulation.None
})
export class AdminComponent {}
