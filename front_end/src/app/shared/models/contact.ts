export class Contact {
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  HomePhone: string;
  MobilePhone: string;
  Nickname: string;
  LastName: string;

 static fromJson(json: any): Contact {
    let c = new Contact();
    if (json) {
      Object.assign(c, json);
    }
    return c;
  }
}
