import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { UserService } from '../services/user.service';

@Injectable()
export class CanActivateIfLoggedInGuard implements CanActivate {

  constructor(private userService: UserService) { }

  canActivate(): boolean {
    return this.userService.isLoggedIn();
  }
}
