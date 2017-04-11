export class Address {
  AddressId: number;
  AddressLine1: string;
  AddressLine2: string;
  City: string;
  State: string;
  ZipCode: string;
  County: string;
  CountryCode: string;

 static fromJson(json: any): Address {
    let a = new Address();
    if (json) {
      Object.assign(a, json);
    }
    return a;
  }
}
