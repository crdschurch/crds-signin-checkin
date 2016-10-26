export class Child {
  ParticipantId: number;
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  FirstName: string;
  LastName: string;
  DateOfBirth: Date;

  name(): string {
    return `${this.FirstName} ${this.LastName}`
  }
}
