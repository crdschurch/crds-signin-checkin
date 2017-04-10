import { Address } from './address';

export class Household {
  HouseholdId: number;
  HomePhone: string;
  HouseholdName: string;
  CongregationId: number;
  HouseholdSourceId: number;
  AddressId: Address;

 static fromJson(json: any): Household {
    let h = new Household();
    if (json) {
      Object.assign(h, json);
    }
    return h;
  }
}
