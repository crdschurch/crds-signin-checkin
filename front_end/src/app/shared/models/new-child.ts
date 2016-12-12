export class NewChild {
  FirstName: string;
  LastName: string;
  DateOfBirth: Date;
  YearGrade: number;

  static fromJson(json: any): NewChild {
    let c = new NewChild();
    if (json) {
      Object.assign(c, json);
    }
    return c;
  }

  name(): string {
    return `${this.FirstName} ${this.LastName}`;
  }
}
