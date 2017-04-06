export class Contact {
  private static GENDER = {
    MALE: 1,
    FEMALE: 2
  };
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  HomePhone: string;
  MobilePhone: string;
  Nickname: string;
  FirstName: string;
  LastName: string;
  Address: string;
  DateOfBirth: Date;
  GenderId: number;
  YearGrade: number;
  IsSpecialNeeds: boolean;
  CongregationName: string;

  public static genderIdMale(): number {
    return Contact.GENDER.MALE;
  }

  public static genderIdFemale(): number {
    return Contact.GENDER.FEMALE;
  }

 static fromJson(json: any): Contact {
    let c = new Contact();
    if (json) {
      Object.assign(c, json);
    }
    return c;
  }

  public isMale(): boolean {
    return this.GenderId === Contact.GENDER.MALE;
  }

  public isFemale(): boolean {
    return this.GenderId === Contact.GENDER.FEMALE;
  }
}
