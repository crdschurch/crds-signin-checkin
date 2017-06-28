export class User {
  token: string;
  refreshToken: string;
  roles: Array<string>;

  static fromJson(json: any): User {
     let u = new User();
     if (json) {
       Object.assign(u, json);
     }
     return u;
   }

  isLoggedIn(): boolean {
    return this.token !== null && this.token !== undefined && this.token.length > 0;
  }

  hasRefreshToken(): boolean {
    return this.refreshToken && this.refreshToken.length > 0;
  }

  logOut(): void {
    this.token = undefined;
    this.refreshToken = undefined;
  }
}
