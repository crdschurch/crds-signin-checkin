import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { UserService } from '../services/user.service';
import { LoginRedirectService } from '../services/login-redirect.service';

@Injectable()
export class CanActivateIfLoggedInGuard implements CanActivate {

  constructor(private userService: UserService, private redirectService: LoginRedirectService) {
    console.log(`User: ${userService.getUser()}`);
  }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (!this.userService.isLoggedIn()) {
      this.redirectService.redirectToLogin(state.url);
      return false;
    }

    return true;
  }
}
