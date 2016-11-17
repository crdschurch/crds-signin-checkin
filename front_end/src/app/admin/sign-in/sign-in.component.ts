import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { SignInService } from './sign-in.service';
import { LoginRedirectService } from '../../shared/services/login-redirect.service';

@Component({
  templateUrl: 'sign-in.component.html',
  providers: [ SignInService ]
})
export class SignInComponent {
  private success: boolean = true;
  private user: any = { username: '', password: '' };
  processing: boolean = false;

  constructor(private signInService: SignInService, private router: Router, private redirectService: LoginRedirectService) { }

  onSubmit() {
    this.processing = true;
    this.signInService.logIn(this.user.username, this.user.password).subscribe(
      resp => {
        this.redirectService.redirectToTarget('/admin/dashboard');
      },
      error => {
        this.processing = false;
        this.success = false;
      });
  }
}
