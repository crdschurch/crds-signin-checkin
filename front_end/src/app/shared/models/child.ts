export class Child {
  ParticipantId: number;
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  FirstName: string;
  LastName: string;
  DateOfBirth: Date;
  signIn: boolean;

  constructor() {
    this.signIn = this.signIn === undefined ? false : this.signIn;
  }

  name(): string {
    return `${this.FirstName} ${this.LastName}`;
  }
}
