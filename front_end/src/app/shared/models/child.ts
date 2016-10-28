export class Child {
  ParticipantId: number;
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  FirstName: string;
  LastName: string;
  DateOfBirth: Date;
  signIn: boolean;

  name(): string {
    return `${this.FirstName} ${this.LastName}`;
  }
}
