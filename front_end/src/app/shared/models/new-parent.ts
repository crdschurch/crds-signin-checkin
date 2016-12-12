export class NewParent {
  FirstName: string;
  LastName: string;
  PhoneNumber: string;

 static fromJson(json: any): NewParent {
    let c = new NewParent();
    if (json) {
      Object.assign(c, json);
    }
    return c;
  }
}
