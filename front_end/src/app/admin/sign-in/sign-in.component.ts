import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { SignInService } from './sign-in.service';

@Component({
  templateUrl: 'sign-in.component.html',
  providers: [ SignInService ]
})
export class SignInComponent {
  private success: boolean = true;
  private user: any = { username: '', password: '' };

  constructor(private signInService: SignInService, private router: Router) { }

  onSubmit() {
    this.signInService.logIn(this.user.username, this.user.password).subscribe(
      resp => this.router.navigate(['/admin/dashboard']),
      error =>  this.success = false);
  }
}
