export class Household {
  HouseholdId: number;
  HomePhone: string;
  HouseholdName: string;
  CongregationId: number;
  HouseholdSourceId: number;
  AddressLine1: string;
  AddressLine2: string;
  City: string;
  State: string;
  ZipCode: string;
  County: string;
  CountryCode: string;

 static fromJson(json: any): Household {
    let h = new Household();
    if (json) {
      Object.assign(h, json);
    }
    return h;
  }
}
