import { Injectable } from '@angular/core';
import { CookieService } from 'angular2-cookie/core';
import { User } from '../models';


@Injectable()
export class UserService {

  constructor(private cookie: CookieService) { }

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
    this.cookie.putObject('user', user);
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
