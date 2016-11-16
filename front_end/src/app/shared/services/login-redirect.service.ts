import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class LoginRedirectService {
  private DefaultAuthenticatedRoute = '/admin/events';
  private SigninRoute = '/admin/sign-in';

  private originalTarget: string;

  constructor(private router: Router) { }

  public redirectToLogin(target = this.DefaultAuthenticatedRoute): void {
    this.originalTarget = target;
    this.router.navigate([this.SigninRoute]);
  }

  public redirectToTarget(target = this.DefaultAuthenticatedRoute): void {
    if (this.originalTarget) {
      this.router.navigate([this.originalTarget]);
    } else {
      this.router.navigate([target]);
    }
  }
}
