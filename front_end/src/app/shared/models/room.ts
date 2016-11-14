import { Group } from '.';

export class Room {
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
    room.Label = json.Label;
    room.BumpingRuleId = json.BumpingRuleId;
    room.BumpingRulePriority = json.BumpingRulePriority;
    room.AssignedGroups = json.AssignedGroups !== undefined && json.AssignedGroups !== null && json.AssignedGroups.length !== 0
                            ? json.AssignedGroups.map((g) => Group.fromJson(g))
                            : [];
    return room;
  }

  getSelectionDescription(): string {
    if (this.AssignedGroups === undefined || this.AssignedGroups === null || this.AssignedGroups.length === 0) {
      return '';
    }

    let selected = this.AssignedGroups.map((g) => { return g.getSelectionDescription(); }).filter((g) => { return g !== null; } );
    return selected.join('; ');
  }

  getRoomNumber() {
    return this.RoomNumber !== undefined && this.RoomNumber !== null && this.RoomNumber.length > 0 ? this.RoomNumber : this.RoomName;
  }

  isBumpingRoom() {
    return this.BumpingRulePriority !== undefined && this.BumpingRulePriority !== null;
  }
}
