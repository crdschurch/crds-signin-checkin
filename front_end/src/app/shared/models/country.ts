export class Country {
  CountryId: number;
  Country: string;
  Code3: string;

  static fromJson(json: any): Country {
    let c = new Country();
    c.CountryId = json.CountryId;
    c.Country = json.Country;
    c.Code3 = json.Code3;

    return c;
  }
}
