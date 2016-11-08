//import { Group } from './group';

export class BumpingRule {
  BumpingRuleId: string;
  FromEventRoomId: string;
  RoomName: string;
  ToEventRoomId: string;
  PriorityOrder: string;
  BumpingRuleTypeId: string;
  BumpingRuleTypeDescription: string;
  FromRoomName: string;
  FromRoomNumber: string;
  ToRoomName: string;
  ToRoomNumber: string;

  static fromJson(json: any): BumpingRule {
    let bumpingRule = new BumpingRule();
    if (!json) {
      return bumpingRule;
    }

    bumpingRule.BumpingRuleId = json.BumpingRuleId;
    bumpingRule.FromEventRoomId = json.FromEventRoomId;
    bumpingRule.RoomName = json.RoomName;
    bumpingRule.ToEventRoomId = json.ToEventRoomId;
    bumpingRule.PriorityOrder = json.PriorityOrder;
    bumpingRule.BumpingRuleTypeId = json.BumpingRuleTypeId;
    bumpingRule.BumpingRuleTypeDescription = json.BumpingRuleTypeDescription;
    bumpingRule.FromRoomName = json.FromRoomName;
    bumpingRule.FromRoomNumber = json.FromRoomNumber;
    bumpingRule.ToRoomName = json.ToRoomName;
    bumpingRule.ToRoomNumber = json.ToRoomNumber;

    return bumpingRule;
  }

  // TODO: Add increment and decrement code


  // getSelectionDescription(): string {
  //   if (this.AssignedGroups === undefined || this.AssignedGroups === null || this.AssignedGroups.length === 0) {
  //     return '';
  //   }

  //   let selected = this.AssignedGroups.map((g) => { return g.getSelectionDescription(); }).filter((g) => { return g !== null; } );
  //   return selected.join('; ');
  // }

  // getRoomNumber() {
  //   return this.RoomNumber !== undefined && this.RoomNumber !== null && this.RoomNumber.length > 0 ? this.RoomNumber : this.RoomName;
  // }

  
}
