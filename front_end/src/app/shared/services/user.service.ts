import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService, CookieOptions } from 'angular2-cookie/core';
import { User } from '../models';
import { LoginRedirectService } from './login-redirect.service';
import { Observable, Subscription } from 'rxjs';

import * as moment from 'moment';
@Injectable()
export class UserService {
  private refreshTimeout: Subscription;
  private SessionLengthMilliseconds = 1800000;

  constructor(private cookie: CookieService, private loginRedirectService: LoginRedirectService, private router: Router) { }

  public getUser(): User {
    let user: User;

    if (!this.cookie.getObject('user')) {
      this.cookie.putObject('user', new User());
    }

    user = Object.create(User.prototype);
    Object.assign(user, this.cookie.getObject('user'));

    return user;
  }

  public setUser(user: User): void {
    let cookieOptions = new CookieOptions();
    cookieOptions.expires = moment().add(this.SessionLengthMilliseconds, 'milliseconds').toDate();

    this.cookie.putObject('user', user, cookieOptions);
    if (this.refreshTimeout) {
      this.refreshTimeout.unsubscribe();
      this.refreshTimeout = undefined;
    }

    if (user.isLoggedIn()) {
      this.refreshTimeout = Observable.timer(this.SessionLengthMilliseconds).subscribe(() => {
        this.loginRedirectService.redirectToLogin(this.router.routerState.snapshot.url);
      });
    }
  }

  public logOut(): void {
    let user = this.getUser();
    user.logOut();

    this.setUser(user);
  }

  public isLoggedIn(): boolean {
    return this.getUser().isLoggedIn();
  }
}
