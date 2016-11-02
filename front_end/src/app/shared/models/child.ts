export class Child {
  ParticipantId: number;
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  FirstName: string;
  LastName: string;
  DateOfBirth: Date;
  Selected: boolean;

  static fromJson(json: any): Child {
    let c = new Child();
    c.ParticipantId = json.ParticipantId;
    c.ContactId = json.ContactId;
    c.HouseholdId = json.HouseholdId;
    c.HouseholdPositionId = json.HouseholdPositionId;
    c.FirstName = json.FirstName;
    c.LastName = json.LastName;
    c.DateOfBirth = json.DateOfBirth;
    c.Selected = json.Selected;
    return c;
  }

  name(): string {
    return `${this.FirstName} ${this.LastName}`;
  }

  selected(): boolean {
    return Boolean(this.Selected).valueOf();
  }

}
