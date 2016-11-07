import { Constants } from '../constants';

export class Child {
  ParticipantId: number;
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  FirstName: string;
  LastName: string;
  DateOfBirth: Date;
  Selected: boolean;
  ParticipationStatusId: number;

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
    c.ParticipationStatusId = json.ParticipationStatusId;
    return c;
  }

  name(): string {
    return `${this.FirstName} ${this.LastName}`;
  }

  selected(): boolean {
    return Boolean(this.Selected).valueOf();
  }

  checkedIn(): boolean {
    return this.ParticipantId === Constants.CheckedInParticipationStatusId;
  }

  toggleCheckIn(): void {
    if (this.ParticipationStatusId === Constants.CheckedInParticipationStatusId) {
      this.ParticipationStatusId = Constants.SignedInParticipationStatusId;
    } else {
      this.ParticipationStatusId = Constants.CheckedInParticipationStatusId;
    }
  }
}
