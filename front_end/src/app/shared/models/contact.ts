export class Contact {
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  HomePhone: string;
  MobilePhone: string;
  Nickname: string;
  LastName: string;
  Address: string;
  DateOfBirth: Date;
  GenderId: number;
  IsSpecialNeeds: boolean;
  CongregationName: string;

  private GENDER = {
    MALE: 1,
    FEMALE: 2
  };

 static fromJson(json: any): Contact {
    let c = new Contact();
    if (json) {
      Object.assign(c, json);
    }
    return c;
  }
}
