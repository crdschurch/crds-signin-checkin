export class NewParent {

  private static GENDER = {
    MALE: 1,
    FEMALE: 2
  };

  FirstName: string;
  LastName: string;
  PhoneNumber: string;
  EmailAddress: string;
  GenderId: number = NewParent.GENDER.FEMALE;
  CongregationId: number;

  public static genderIdMale(): number {
    return NewParent.GENDER.MALE;
  }

  public static genderIdFemale(): number {
    return NewParent.GENDER.FEMALE;
  }

 static fromJson(json: any): NewParent {
    let c = new NewParent();
    if (json) {
      Object.assign(c, json);
    }
    return c;
  }

  public isMale(): boolean {
    return this.GenderId === NewParent.GENDER.MALE;
  }

  public isFemale(): boolean {
    return this.GenderId === NewParent.GENDER.FEMALE;
  }
}
