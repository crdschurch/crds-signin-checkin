import { Component, ViewEncapsulation } from '@angular/core';
import { HeaderService } from './header/header.service';
import { Event } from './events/event';

@Component({
  selector: 'admin',
  templateUrl: 'admin.component.html',
  styleUrls: ['admin.component.scss'],
  providers: [ HeaderService ],
  encapsulation: ViewEncapsulation.None
})
export class AdminComponent {}
