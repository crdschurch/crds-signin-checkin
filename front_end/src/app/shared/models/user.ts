export class User {
  token: string;
  refreshToken: string;
  roles: Array<string>;

  isLoggedIn(): boolean {
    return this.token.length > 0;
  }

  logOut(): void {
    this.token = undefined;
  }
}
