import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  templateUrl: 'reports.component.html',
  providers: [  ]
})
export class ReportsComponent {
  private success = true;
  private user: any = { username: '', password: '' };
  processing = false;

  constructor() {
  }
}
