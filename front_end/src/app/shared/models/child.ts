import { Constants } from '../constants';

export class Child {
  EventParticipantId: number;
  ParticipantId: number;
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  FirstName: string;
  LastName: string;
  DateOfBirth: Date;
  Selected: boolean;
  ParticipationStatusId: number;
  AssignedRoomId: number;
  AssignedRoomName: string;
  AssignedSecondaryRoomId: number;
  AssignedSecondaryRoomName: string;
  CallNumber: string;

  static fromJson(json: any): Child {
    let c = new Child();
    c.EventParticipantId = json.EventParticipantId;
    c.ParticipantId = json.ParticipantId;
    c.ContactId = json.ContactId;
    c.HouseholdId = json.HouseholdId;
    c.HouseholdPositionId = json.HouseholdPositionId;
    c.FirstName = json.FirstName;
    c.LastName = json.LastName;
    c.DateOfBirth = json.DateOfBirth;
    c.Selected = json.Selected;
    c.ParticipationStatusId = json.ParticipationStatusId;
    c.AssignedRoomId = json.AssignedRoomId;
    c.AssignedRoomName = json.AssignedRoomName;
    c.AssignedSecondaryRoomId = json.AssignedSecondaryRoomId;
    c.AssignedSecondaryRoomName = json.AssignedSecondaryRoomName;
    c.CallNumber = json.CallNumber;
    return c;
  }

  name(): string {
    return `${this.FirstName} ${this.LastName}`;
  }

  selected(): boolean {
    return Boolean(this.Selected).valueOf();
  }

  checkedIn(): boolean {
    return this.ParticipationStatusId === Constants.CheckedInParticipationStatusId;
  }

  toggleCheckIn(): void {
    if (this.ParticipationStatusId === Constants.CheckedInParticipationStatusId) {
      this.ParticipationStatusId = Constants.SignedInParticipationStatusId;
    } else {
      this.ParticipationStatusId = Constants.CheckedInParticipationStatusId;
    }
  }
}
