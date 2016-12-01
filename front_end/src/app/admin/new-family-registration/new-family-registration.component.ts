import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AdminService } from '../admin.service';

@Component({
  templateUrl: 'new-family-registration.component.html'
})
export class NewFamilyRegistrationComponent {
  constructor(private adminService: AdminService) {
  }
}
