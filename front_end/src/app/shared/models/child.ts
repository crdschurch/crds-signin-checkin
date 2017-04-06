import { Constants } from '../constants';
import { Contact } from '.';

export class Child {
  EventParticipantId: number;
  EventId: number;
  EventIdSecondary: number;
  ParticipantId: number;
  ContactId: number;
  HouseholdId: number;
  HouseholdPositionId: number;
  Nickname: string;
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
  SignInErrorMessage: string;
  CheckinPhone: string;
  GroupId: number;
  GroupName: string;
  TimeIn: Date;
  TimeConfirmed: Date;
  HeadsOfHousehold: Array<Contact> = [];

  static fromJson(json: any): Child {
    let c = new Child();
    c.EventParticipantId = json.EventParticipantId;
    c.EventId = json.EventId;
    c.EventIdSecondary = json.EventIdSecondary;
    c.ParticipantId = json.ParticipantId;
    c.ContactId = json.ContactId;
    c.HouseholdId = json.HouseholdId;
    c.HouseholdPositionId = json.HouseholdPositionId;
    c.FirstName = json.FirstName;
    c.LastName = json.LastName;
    c.Nickname = json.Nickname;
    c.DateOfBirth = json.DateOfBirth;
    c.Selected = json.Selected;
    c.ParticipationStatusId = json.ParticipationStatusId;
    c.AssignedRoomId = json.AssignedRoomId;
    c.AssignedRoomName = json.AssignedRoomName;
    c.AssignedSecondaryRoomId = json.AssignedSecondaryRoomId;
    c.AssignedSecondaryRoomName = json.AssignedSecondaryRoomName;
    c.CallNumber = json.CallNumber;
    c.CheckinPhone = json.CheckinPhone;
    c.SignInErrorMessage = json.SignInErrorMessage;
    c.GroupId = json.GroupId;
    c.GroupName = json.GroupName;
    c.TimeIn = json.TimeIn;
    c.TimeConfirmed = json.TimeConfirmed;

    if (json.HeadsOfHousehold !== null && json.HeadsOfHousehold !== undefined) {
      c.HeadsOfHousehold = (<Contact[]>json.HeadsOfHousehold).map(r => Contact.fromJson(r));
    }

    return c;
  }

  name(): string {
    return `${this.Nickname || this.FirstName} ${this.LastName}`;
  }

  assignedRoom() {
    return this.AssignedRoomName ? this.AssignedRoomName : 'Error';
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

  guest(): boolean {
    return Constants.GuestHouseholdId === this.HouseholdId;
  }

  uniqueIdentifier() {
    return this.ContactId;
  }
}
