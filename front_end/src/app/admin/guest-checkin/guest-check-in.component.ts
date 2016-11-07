import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AdminService } from '../admin.service';

@Component({
  templateUrl: 'guest-check-in.component.html'
})
export class GuestCheckInComponent {
  constructor(
    private adminService: AdminService) {
  }
}
