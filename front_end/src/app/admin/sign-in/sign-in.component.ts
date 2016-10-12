import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { SignInService } from './sign-in.service';

@Component({
  selector: 'sign-in',
  templateUrl: 'sign-in.component.html',
  providers: [ SignInService ]
})
export class SignInComponent {
  private errorMsg: string = '';
  private user: any = { username: '', password: '' };

  constructor(private signInService: SignInService, private router: Router) { }

  onSubmit() {
    console.log("onSubmit", this)
    this.signInService.logIn(this.user.username, this.user.password).subscribe(
      resp => this.router.navigate(['/child-checkin']),
      error =>  this.errorMsg = error);
  }
}
