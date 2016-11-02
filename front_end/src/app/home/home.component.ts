import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'home',
  templateUrl: 'home.component.html',
})
export class HomeComponent {

  constructor(private router: Router) { }

  goToChildCheckin() {
    this.router.navigate(['/child-checkin/room']);
  }

  goToChildSignin() {
    this.router.navigate(['/child-signin/search']);
  }

  goToAdminTools() {
    this.router.navigate(['/admin/sign-in']);
  }
}
