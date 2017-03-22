export class Congregation {
  CongregationId: number;
  CongregationName: string;

 static fromJson(json: any): Congregation {
    let c = new Congregation();
    if (json) {
      Object.assign(c, json);
    }
    return c;
  }
}
