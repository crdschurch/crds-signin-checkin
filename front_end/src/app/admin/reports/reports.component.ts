import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { HeaderService } from '../header/header.service';

@Component({
  templateUrl: 'reports.component.html',
  providers: [  ]
})
export class ReportsComponent {
  private success = true;
  private user: any = { username: '', password: '' };
  processing = false;

  constructor(private headerService: HeaderService) {
  }
}
