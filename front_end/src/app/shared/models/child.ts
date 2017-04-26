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
  GenderId: number;
  YearGrade: number;
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
  KCSortOrder: number;
  _CanCheckIn: boolean;

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
    c.GenderId = json.GenderId;
    c.YearGrade = json.YearGrade;
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
    c.KCSortOrder = json.KCSortOrder;

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

  KCEvent(eventType: number): boolean {
    return eventType !== Constants.ChildCareEventType && eventType !== Constants.BigEventType &&
      eventType !== Constants.StudentMinistry6through8EventType && eventType !== Constants.StudentMinistry9through12EventType;
  }

  ChildCareEvent(eventType: number): boolean {
    return eventType === Constants.ChildCareEventType;
  }

  StudentMinistry6through8Event(eventType: number): boolean {
    return eventType === Constants.StudentMinistry6through8EventType;
  }

  StudentMinistry9through12Event(eventType: number): boolean {
    return eventType === Constants.StudentMinistry9through12EventType;
  }

  BigEvent(eventType: number): boolean {
    return eventType === Constants.BigEventType;
  }

  KCGroup(): boolean {
    return !this.BigGroup();
  }

  StudentMinistry6through8GradeGroup(): boolean {
    return this.GroupId === Constants.MsmSixth || this.GroupId === Constants.MsmSeventh || this.GroupId === Constants.MsmEighth;
  }

  StudentMinistry9through12GradeGroup(): boolean {
    return this.GroupId === Constants.HighSchoolNinth || this.GroupId === Constants.HighSchoolTenth ||
      this.GroupId === Constants.HighSchoolEleventh || this.GroupId === Constants.HighSchoolTwelfth;
  }

  BigGroup(): boolean {
    return this.StudentMinistry6through8GradeGroup() || this.StudentMinistry9through12GradeGroup();
  }

  canCheckIn(eventType: number): boolean {
    this._CanCheckIn = (this.ChildCareEvent(eventType) && this.KCGroup()) || // Child Care Event and Kids Club Group
      (this.KCEvent(eventType) && this.KCGroup()) || // or Kids Club Event and Kids Club Group
      (this.StudentMinistry6through8Event(eventType) && this.StudentMinistry6through8GradeGroup()) || // or Student 6-8 Event and group
      (this.StudentMinistry9through12Event(eventType) && this.StudentMinistry9through12GradeGroup()) || // or Student 9-12 Event and group
      (this.BigEvent(eventType) && this.BigGroup()); // or Student 6-12 Big Event and group 6-12

    return this._CanCheckIn;
  }

  get CanCheckIn(): boolean {
    return this._CanCheckIn;
  }
}
