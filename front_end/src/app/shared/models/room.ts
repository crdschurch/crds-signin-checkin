import { Group } from '.';

export class Room {
  static BUMPING_TYPE = {
    PRIORITY: 1,
    VACANCY: 2
  };
  EventRoomId: string;
  RoomId: string;
  EventId: string;
  RoomName: string;
  RoomNumber: string;
  AllowSignIn: boolean;
  Volunteers: number;
  Capacity: number;
  SignedIn: number;
  CheckedIn: number;
  Label: string;
  AssignedGroups: Group[];
  BumpingRuleId: number;
  BumpingRulePriority: number;
  BumpingRuleTypeId: number;
  AdventureClub: boolean;
  KcSortOrder: number;

  static fromJsons(jsons: any): Room[] {
    let rooms: Room[] = [];
    for (let json of jsons) {
        rooms.push(Room.fromJson(json));
    }
    return rooms;
  }

  static fromJson(json: any): Room {
    let room = new Room();
    if (!json) {
      return room;
    }

    room.EventRoomId = json.EventRoomId;
    room.RoomId = json.RoomId;
    room.EventId = json.EventId;
    room.RoomName = json.RoomName;
    room.RoomNumber = json.RoomNumber;
    room.AllowSignIn = json.AllowSignIn;
    room.Volunteers = json.Volunteers;
    room.Capacity = json.Capacity;
    room.SignedIn = json.SignedIn;
    room.CheckedIn = json.CheckedIn;
    room.Label = json.Label;
    room.AdventureClub = json.AdventureClub;
    room.KcSortOrder = json.KcSortOrder;
    room.BumpingRuleId = json.BumpingRuleId;
    room.BumpingRulePriority = json.BumpingRulePriority;
    room.BumpingRuleTypeId = json.BumpingRuleTypeId;
    room.AssignedGroups = json.AssignedGroups !== undefined && json.AssignedGroups !== null && json.AssignedGroups.length !== 0
                            ? json.AssignedGroups.map((g) => Group.fromJson(g))
                            : [];
    return room;
  }

  getSelectionDescription(split = true): any {
    if (this.AssignedGroups === undefined || this.AssignedGroups === null || this.AssignedGroups.length === 0) {
      return '';
    }

    let selected = this.AssignedGroups.map((g) => { return g.getSelectionDescription(); }).filter((g) => { return g !== null; } );

    if (split) {
      return selected.join('; ');
    }

    return selected;
  }

  getRoomNumber() {
    return this.RoomNumber !== undefined && this.RoomNumber !== null && this.RoomNumber.length > 0 ? this.RoomNumber : this.RoomName;
  }

  isBumpingRoom() {
    return this.BumpingRulePriority !== undefined && this.BumpingRulePriority !== null;
  }

  isTypePriority() {
    return this.BumpingRuleTypeId === Room.BUMPING_TYPE.PRIORITY;
  }
  isTypeVacancy() {
    return this.BumpingRuleTypeId === Room.BUMPING_TYPE.VACANCY;
  }
}
